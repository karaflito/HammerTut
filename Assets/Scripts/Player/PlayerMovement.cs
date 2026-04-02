using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerSensors))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerMovement : MonoBehaviour
{
    private const float InputDeadZone = 0.01f;
    private const float ClimbReentryLockDuration = 0.15f;

    [Header("Movement Settings")]
    [SerializeField] private PlayerData playerData;

    private PlayerInputHandler input;
    private PlayerSensors sensors;
    private PlayerMotor motor;

    private float coyoteTimeRemaining;
    private float dashTimeRemaining;
    private float dashCooldownRemaining;
    private float climbReentryLockRemaining;
    private Vector2 dashDirection;
    private bool stateInitialized;
    private bool suppressNextAirJump;

    public PlayerStateId CurrentStateId { get; private set; }
    public bool IsGrounded => sensors != null && sensors.IsGrounded;
    public bool IsClimbing => CurrentStateId == PlayerStateId.Climbing;
    public bool IsDashing => CurrentStateId == PlayerStateId.Dashing;
    public bool CanDash => dashCooldownRemaining <= 0f && !IsDashing;
    public Vector2 Velocity => motor != null ? motor.Velocity : Vector2.zero;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        sensors = GetComponent<PlayerSensors>();
        motor = GetComponent<PlayerMotor>();

        if (playerData == null)
        {
            Debug.LogError("PlayerMovement requires a PlayerData asset.", this);
            enabled = false;
        }
    }

    private void Start()
    {
        sensors.Refresh();
        SetState(sensors.IsGrounded ? PlayerStateId.Grounded : PlayerStateId.Airborne);
    }

    private void Update()
    {
        UpdateCoyoteTime();

        if (dashCooldownRemaining > 0f)
        {
            dashCooldownRemaining -= Time.deltaTime;
        }

        if (climbReentryLockRemaining > 0f)
        {
            climbReentryLockRemaining -= Time.deltaTime;
        }

        switch (CurrentStateId)
        {
            case PlayerStateId.Grounded:
                TickGrounded();
                break;
            case PlayerStateId.Airborne:
                TickAirborne();
                break;
            case PlayerStateId.Climbing:
                TickClimbing();
                break;
            case PlayerStateId.Dashing:
                TickDashing();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (CurrentStateId)
        {
            case PlayerStateId.Grounded:
                FixedTickGrounded();
                break;
            case PlayerStateId.Airborne:
                FixedTickAirborne();
                break;
            case PlayerStateId.Climbing:
                FixedTickClimbing();
                break;
            case PlayerStateId.Dashing:
                FixedTickDashing();
                break;
        }
    }

    private void TickGrounded()
    {
        if (!sensors.IsGrounded)
        {
            SetState(PlayerStateId.Airborne);
            return;
        }

        if (input.ConsumeDashPressed() && CanDash)
        {
            SetState(PlayerStateId.Dashing);
            return;
        }

        if (ShouldStartClimb())
        {
            SetState(PlayerStateId.Climbing);
            return;
        }

        if (input.ConsumeJumpPressed())
        {
            PerformJump();
            SetState(PlayerStateId.Airborne);
        }
    }

    private void FixedTickGrounded()
    {
        ApplyHorizontalMovement();
        motor.SetGravityScale(playerData.normalGravity);
    }

    private void TickAirborne()
    {
        if (input.ConsumeDashPressed() && CanDash)
        {
            SetState(PlayerStateId.Dashing);
            return;
        }

        if (ShouldStartClimb())
        {
            SetState(PlayerStateId.Climbing);
            return;
        }

        if (input.ConsumeJumpPressed() && coyoteTimeRemaining > 0f && !suppressNextAirJump)
        {
            PerformJump();
            coyoteTimeRemaining = 0f;
            return;
        }

        if (sensors.IsGrounded && motor.Velocity.y <= 0f)
        {
            SetState(PlayerStateId.Grounded);
        }
    }

    private void FixedTickAirborne()
    {
        ApplyHorizontalMovement();
        ApplyAirGravity();
    }

    private void TickClimbing()
    {
        if (!sensors.IsNearClimbable)
        {
            SetState(sensors.IsGrounded ? PlayerStateId.Grounded : PlayerStateId.Airborne);
            return;
        }

        if (input.ConsumeDashPressed() && CanDash)
        {
            SetState(PlayerStateId.Dashing);
            return;
        }

        if (input.ConsumeJumpPressed())
        {
            PerformJump();
            LockClimbReentry();
            suppressNextAirJump = false;
            SetState(PlayerStateId.Airborne);
        }
    }

    private void FixedTickClimbing()
    {
        motor.SetGravityScale(0f);
        motor.SetHorizontalSpeed(input.MoveInput * playerData.speed);
        motor.FaceDirection(input.MoveInput);
        motor.SetVerticalSpeed(input.ClimbInput * playerData.climbSpeed);
    }

    private void TickDashing()
    {
        dashTimeRemaining -= Time.deltaTime;

        if (dashTimeRemaining > 0f)
        {
            return;
        }

        if (ShouldStartClimb())
        {
            SetState(PlayerStateId.Climbing);
            return;
        }

        SetState(sensors.IsGrounded ? PlayerStateId.Grounded : PlayerStateId.Airborne);
    }

    private void FixedTickDashing()
    {
        motor.SetGravityScale(0f);
        motor.SetVelocity(dashDirection * playerData.dashSpeed);
    }

    private void SetState(PlayerStateId newState)
    {
        if (stateInitialized && CurrentStateId == newState)
        {
            return;
        }

        PlayerStateId previousState = CurrentStateId;
        if (stateInitialized)
        {
            ExitState(previousState);
        }

        CurrentStateId = newState;
        EnterState(stateInitialized ? previousState : newState, newState);
        stateInitialized = true;
    }

    private void EnterState(PlayerStateId previousState, PlayerStateId newState)
    {
        switch (newState)
        {
            case PlayerStateId.Grounded:
                motor.SetGravityScale(playerData.normalGravity);
                break;

            case PlayerStateId.Airborne:
                if (previousState == PlayerStateId.Climbing)
                {
                    suppressNextAirJump = true;
                    coyoteTimeRemaining = 0f;
                }
                break;

            case PlayerStateId.Climbing:
                suppressNextAirJump = false;
                motor.SetGravityScale(0f);
                motor.SetVerticalSpeed(0f);
                break;

            case PlayerStateId.Dashing:
                suppressNextAirJump = false;
                dashTimeRemaining = playerData.dashDuration;
                dashCooldownRemaining = playerData.dashCoolDown;
                dashDirection = ResolveDashDirection();
                motor.SetGravityScale(0f);
                motor.SetVelocity(dashDirection * playerData.dashSpeed);
                break;
        }
    }

    private void ExitState(PlayerStateId oldState)
    {
        if (oldState == PlayerStateId.Climbing || oldState == PlayerStateId.Dashing)
        {
            motor.SetGravityScale(playerData.normalGravity);
        }

        if (oldState == PlayerStateId.Grounded)
        {
            suppressNextAirJump = false;
        }
    }

    private void ApplyHorizontalMovement()
    {
        motor.SetHorizontalSpeed(input.MoveInput * playerData.speed);
        motor.FaceDirection(input.MoveInput);
    }

    private void ApplyAirGravity()
    {
        if (motor.Velocity.y < 0f)
        {
            motor.SetGravityScale(playerData.normalGravity * playerData.fallGravityMultiplier);
            return;
        }

        motor.SetGravityScale(playerData.normalGravity);
    }

    private void PerformJump()
    {
        motor.SetGravityScale(playerData.normalGravity);
        motor.ResetVerticalVelocity();
        motor.AddImpulse(Vector2.up * playerData.jumpForce);
        coyoteTimeRemaining = 0f;
    }

    private Vector2 ResolveDashDirection()
    {
        if (Mathf.Abs(input.MoveInput) > InputDeadZone)
        {
            float direction = Mathf.Sign(input.MoveInput);
            motor.FaceDirection(direction);
            return new Vector2(direction, 0f);
        }

        return new Vector2(motor.FacingSign, 0f);
    }

    private bool ShouldStartClimb()
    {
        return climbReentryLockRemaining <= 0f
            && sensors.IsNearClimbable
            && Mathf.Abs(input.ClimbInput) > InputDeadZone;
    }

    private void LockClimbReentry()
    {
        climbReentryLockRemaining = ClimbReentryLockDuration;
    }

    private void UpdateCoyoteTime()
    {
        if (CurrentStateId == PlayerStateId.Climbing)
        {
            return;
        }

        if (sensors.IsGrounded)
        {
            coyoteTimeRemaining = playerData.coyoteTime;
            suppressNextAirJump = false;
            return;
        }

        if (coyoteTimeRemaining > 0f)
        {
            coyoteTimeRemaining -= Time.deltaTime;
        }
    }
}

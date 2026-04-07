using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerSensors))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerMovement : MonoBehaviour
{
    private const float ClimbReentryLockDuration = 0.15f;

    [Header("Movement Settings")]
    [SerializeField] private PlayerData playerData;

    private Dictionary<PlayerStateId, PlayerState> states;
    private PlayerState currentState;
    private float climbReentryLockRemaining;
    private bool stateInitialized;

    public PlayerInputHandler Input { get; private set; }
    public PlayerSensors Sensors { get; private set; }
    public PlayerMotor Motor { get; private set; }
    public PlayerData Data => playerData;

    public PlayerStateId CurrentStateId { get; private set; }
    public bool IsGrounded => Sensors != null && Sensors.IsGrounded;
    public bool IsClimbing => CurrentStateId == PlayerStateId.Climbing;
    public bool IsDashing => CurrentStateId == PlayerStateId.Dashing;
    public bool CanDash => DashCooldownRemaining <= 0f && !IsDashing;
    public Vector2 Velocity => Motor != null ? Motor.Velocity : Vector2.zero;

    public float CoyoteTimeRemaining { get; set; }
    public float DashCooldownRemaining { get; set; }
    public bool SuppressNextAirJump { get; set; }

    private void Awake()
    {
        Input = GetComponent<PlayerInputHandler>();
        Sensors = GetComponent<PlayerSensors>();
        Motor = GetComponent<PlayerMotor>();

        if (playerData == null)
        {
            Debug.LogError("PlayerMovement requires a PlayerData asset.", this);
            enabled = false;
            return;
        }

        states = new Dictionary<PlayerStateId, PlayerState>
        {
            { PlayerStateId.Grounded, new GroundedState(this) },
            { PlayerStateId.Airborne, new AirborneState(this) },
            { PlayerStateId.Climbing, new ClimbingState(this) },
            { PlayerStateId.Dashing, new DashingState(this) }
        };
    }

    private void Start()
    {
        Sensors.Refresh();
        SetState(Sensors.IsGrounded ? PlayerStateId.Grounded : PlayerStateId.Airborne);
    }

    private void Update()
    {
        currentState?.Tick();
    }

    private void FixedUpdate()
    {
        UpdateTimers();
        currentState?.FixedTick();
    }

    public void SetState(PlayerStateId newStateId)
    {
        if (stateInitialized && CurrentStateId == newStateId)
            return;

        if (stateInitialized)
            currentState?.Exit();

        PlayerStateId previousId = CurrentStateId;
        CurrentStateId = newStateId;
        currentState = states[newStateId];
        currentState.Enter(stateInitialized ? previousId : newStateId);
        stateInitialized = true;
    }

    public void PerformJump()
    {
        Motor.SetGravityScale(playerData.normalGravity);
        Motor.SetVerticalSpeed(playerData.jumpForce);
        CoyoteTimeRemaining = 0f;
    }

    public void ApplyHorizontalMovement()
    {
        Motor.SetHorizontalSpeed(Input.MoveInput * playerData.speed);
        Motor.FaceDirection(Input.MoveInput);
    }

    public void ApplyAirGravity()
    {
        if (Motor.Velocity.y < 0f)
        {
            Motor.SetGravityScale(playerData.normalGravity * playerData.fallGravityMultiplier);
            return;
        }

        Motor.SetGravityScale(playerData.normalGravity);
    }

    public bool ShouldStartClimb()
    {
        return climbReentryLockRemaining <= 0f
            && Sensors.IsNearClimbable
            && Mathf.Abs(Input.ClimbInput) > PlayerMotor.InputDeadZone;
    }

    public void LockClimbReentry()
    {
        climbReentryLockRemaining = ClimbReentryLockDuration;
    }

    private void UpdateTimers()
    {
        float dt = Time.fixedDeltaTime;

        if (DashCooldownRemaining > 0f)
            DashCooldownRemaining -= dt;

        if (climbReentryLockRemaining > 0f)
            climbReentryLockRemaining -= dt;

        UpdateCoyoteTime(dt);
    }

    private void UpdateCoyoteTime(float dt)
    {
        if (CurrentStateId == PlayerStateId.Climbing)
            return;

        if (Sensors.IsGrounded)
        {
            CoyoteTimeRemaining = playerData.coyoteTime;
            SuppressNextAirJump = false;
            return;
        }

        if (CoyoteTimeRemaining > 0f)
            CoyoteTimeRemaining -= dt;
    }
}

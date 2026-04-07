using UnityEngine;

public class DashingState : PlayerState
{
    private float dashTimeRemaining;
    private Vector2 dashDirection;

    public DashingState(PlayerMovement ctx) : base(ctx) { }

    public override PlayerStateId Id => PlayerStateId.Dashing;

    public override void Enter(PlayerStateId previousState)
    {
        ctx.SuppressNextAirJump = false;
        dashTimeRemaining = ctx.Data.dashDuration;
        ctx.DashCooldownRemaining = ctx.Data.dashCoolDown;
        dashDirection = ResolveDashDirection();
        ctx.Motor.SetGravityScale(0f);
        ctx.Motor.SetVelocity(dashDirection * ctx.Data.dashSpeed);
    }

    public override void Exit()
    {
        ctx.Motor.SetGravityScale(ctx.Data.normalGravity);
    }

    public override void Tick()
    {
        if (dashTimeRemaining > 0f)
            return;

        if (TryClimb()) return;

        ctx.SetState(ctx.Sensors.IsGrounded ? PlayerStateId.Grounded : PlayerStateId.Airborne);
    }

    public override void FixedTick()
    {
        dashTimeRemaining -= Time.fixedDeltaTime;
        ctx.Motor.SetGravityScale(0f);
        ctx.Motor.SetVelocity(dashDirection * ctx.Data.dashSpeed);
    }

    private Vector2 ResolveDashDirection()
    {
        if (Mathf.Abs(ctx.Input.MoveInput) > PlayerMotor.InputDeadZone)
        {
            float direction = Mathf.Sign(ctx.Input.MoveInput);
            ctx.Motor.FaceDirection(direction);
            return new Vector2(direction, 0f);
        }

        return new Vector2(ctx.Motor.FacingSign, 0f);
    }
}

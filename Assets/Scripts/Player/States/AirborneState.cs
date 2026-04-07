public class AirborneState : PlayerState
{
    public AirborneState(PlayerMovement ctx) : base(ctx) { }

    public override PlayerStateId Id => PlayerStateId.Airborne;

    public override void Enter(PlayerStateId previousState)
    {
        if (previousState == PlayerStateId.Climbing)
        {
            ctx.SuppressNextAirJump = true;
            ctx.CoyoteTimeRemaining = 0f;
        }
    }

    public override void Tick()
    {
        if (TryDash()) return;
        if (TryClimb()) return;

        if (ctx.Input.ConsumeJumpPressed() && ctx.CoyoteTimeRemaining > 0f && !ctx.SuppressNextAirJump)
        {
            ctx.PerformJump();
            ctx.CoyoteTimeRemaining = 0f;
            return;
        }

        if (ctx.Sensors.IsGrounded && ctx.Motor.Velocity.y <= 0f)
        {
            ctx.SetState(PlayerStateId.Grounded);
        }
    }

    public override void FixedTick()
    {
        ctx.ApplyHorizontalMovement();
        ctx.ApplyAirGravity();
    }
}

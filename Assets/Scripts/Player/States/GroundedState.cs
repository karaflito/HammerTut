public class GroundedState : PlayerState
{
    public GroundedState(PlayerMovement ctx) : base(ctx) { }

    public override PlayerStateId Id => PlayerStateId.Grounded;

    public override void Enter(PlayerStateId previousState)
    {
        ctx.Motor.SetGravityScale(ctx.Data.normalGravity);
    }

    public override void Exit()
    {
        ctx.SuppressNextAirJump = false;
    }

    public override void Tick()
    {
        if (!ctx.Sensors.IsGrounded)
        {
            ctx.SetState(PlayerStateId.Airborne);
            return;
        }

        if (TryDash()) return;
        if (TryClimb()) return;

        if (ctx.Input.ConsumeJumpPressed())
        {
            ctx.PerformJump();
            ctx.SetState(PlayerStateId.Airborne);
        }
    }

    public override void FixedTick()
    {
        ctx.ApplyHorizontalMovement();
        ctx.Motor.SetGravityScale(ctx.Data.normalGravity);
    }
}

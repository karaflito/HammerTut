using UnityEngine;

public class ClimbingState : PlayerState
{
    public ClimbingState(PlayerMovement ctx) : base(ctx) { }

    public override PlayerStateId Id => PlayerStateId.Climbing;

    public override void Enter(PlayerStateId previousState)
    {
        ctx.SuppressNextAirJump = false;
        ctx.Motor.SetGravityScale(0f);
        ctx.Motor.SetVerticalSpeed(0f);
    }

    public override void Exit()
    {
        ctx.Motor.SetGravityScale(ctx.Data.normalGravity);
    }

    public override void Tick()
    {
        if (!ctx.Sensors.IsNearClimbable)
        {
            ctx.SetState(ctx.Sensors.IsGrounded ? PlayerStateId.Grounded : PlayerStateId.Airborne);
            return;
        }

        if (TryDash()) return;

        if (ctx.Input.ConsumeJumpPressed())
        {
            ctx.PerformJump();
            ctx.LockClimbReentry();
            ctx.SuppressNextAirJump = false;
            ctx.SetState(PlayerStateId.Airborne);
        }
    }

    public override void FixedTick()
    {
        ctx.Motor.SetGravityScale(0f);
        ctx.Motor.SetHorizontalSpeed(ctx.Input.MoveInput * ctx.Data.speed);
        ctx.Motor.FaceDirection(ctx.Input.MoveInput);
        ctx.Motor.SetVerticalSpeed(ctx.Input.ClimbInput * ctx.Data.climbSpeed);
    }
}

public abstract class PlayerState
{
    protected readonly PlayerMovement ctx;

    protected PlayerState(PlayerMovement ctx)
    {
        this.ctx = ctx;
    }

    public abstract PlayerStateId Id { get; }
    public virtual void Enter(PlayerStateId previousState) { }
    public virtual void Exit() { }
    public virtual void Tick() { }
    public virtual void FixedTick() { }

    protected bool TryDash()
    {
        if (ctx.Input.ConsumeDashPressed() && ctx.CanDash)
        {
            ctx.SetState(PlayerStateId.Dashing);
            return true;
        }
        return false;
    }

    protected bool TryClimb()
    {
        if (ctx.ShouldStartClimb())
        {
            ctx.SetState(PlayerStateId.Climbing);
            return true;
        }
        return false;
    }
}

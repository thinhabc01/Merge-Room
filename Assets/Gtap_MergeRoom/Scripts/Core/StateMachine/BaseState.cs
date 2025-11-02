public abstract class BaseState
{
    public virtual void Enter(){}
    public virtual void Exit(){}
    public virtual void Dispose(){}
}

public abstract class PayloadState<TPayload> : BaseState
{
    public abstract void Enter(TPayload payload);
}
public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void SetState(IState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}

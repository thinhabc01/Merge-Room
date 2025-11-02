using System;

public class GameManager: IGameStateChanger
{
    private readonly GameSettings _settings;
    public event Action<GameState> OnStateChanged;
    public GameState CurrentState { get; private set; }

    public GameManager(GameSettings settings)
    {
        _settings = settings;
    }
    
    public void SetState(GameState state)
    {
        CurrentState = state;

        OnStateChanged?.Invoke(state);
    }

    public void Destroy()
    {
    }
}
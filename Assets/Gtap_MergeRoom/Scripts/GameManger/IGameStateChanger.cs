using System;

public interface IGameStateChanger: IGameStateEvent
{
    void SetState(GameState state);
}

public interface IGameStateEvent
{
    GameState CurrentState { get; }
    
    event Action<GameState> OnStateChanged;
}
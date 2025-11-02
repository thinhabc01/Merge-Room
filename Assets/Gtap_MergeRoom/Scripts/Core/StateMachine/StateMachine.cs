using System;
using System.Collections.Generic;

public class StateMachine
{
    private readonly IUpdater _updater;
    
    private Dictionary<Type, BaseState> _states;
    private BaseState _activeState;

    public event Action<BaseState> StateChanged;

    public StateMachine(IUpdater updater)
    {
        _states = new Dictionary<Type, BaseState>();
        _updater = updater;
    }

    public void AddState<TState>(TState state) where TState : BaseState
    {
        _states[typeof(TState)] = state;
    }

    public void Enter<TState>() where TState : BaseState
    {
        BaseState state = ChangeState<TState>();
        state?.Enter();
    }
    
    public void Enter<TState, TPayload>(TPayload payload) where TState : PayloadState<TPayload>
    {
        TState state = ChangeState<TState>();
        state?.Enter(payload);
    }

    
    public void Disable()
    {
        if(_activeState != null)
        {
            _updater.RemoveFrom(_activeState);
        }
    }

    public void Dispose()
    {
        foreach (var state in _states.Values)
        {
            _updater.RemoveFrom(state);
            state.Dispose();
        }

        _states.Clear();
    }

    private TState ChangeState<TState>() where TState : BaseState
    {
        TState state = GetState<TState>();
        
        if(_activeState != null)
        {
            if (_activeState.Equals(state))
                return null;

            _activeState.Exit();
            _updater.RemoveFrom(_activeState);
        }

        _activeState = state;
        _updater.AddTo(state);
        StateChanged?.Invoke(state);
        
        return state;
    }

    private TState GetState<TState>() where TState : BaseState
    {
        return _states[typeof(TState)] as TState;
    }
}
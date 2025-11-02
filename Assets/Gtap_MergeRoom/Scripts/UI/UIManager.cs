using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager
{
    private readonly Canvas _canvas;
    private readonly PlayerInput _playerInput;
    private readonly GameSettings _setting;
    private readonly IGameStateChanger _stateChanger;
    private UIWindow _previousWindow, _activeWindow;

    private UIWindow[] _windows;

    public event Action<int> OnSelectRoom; 

    public UIManager(IGameStateChanger stateChanger, Canvas canvas, PlayerInput playerInput, GameSettings setting)
    {
        _stateChanger = stateChanger;
        _canvas = canvas;
        _playerInput = playerInput;
        _setting = setting;

        _stateChanger.OnStateChanged += OnGameStateChange;
        _playerInput.Player.Back.performed += ButtonBack;

        SetupWindows();
    }

    private void OnGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Setup:
                ShowMenu<LobbyWindow>();
                break;
            case GameState.Tutorial:
                ShowMenu<TutorialWindow>();
                break;
            case GameState.GameLoop:
                ShowMenu<GameLoopWindow>();
                break;
            case GameState.GameComplete:
                ShowMenu<GameCompleteWindow>();
                break;
        }
    }

    public T GetWindow<T>() where T : MonoBehaviour
    {
        foreach (var window in _windows)
        {
            if (window is T result)
            {
                return result;
            }
        }

        return null;
    }

    private void SetupWindows()
    {
        _windows = _canvas.GetComponentsInChildren<UIWindow>(true);
        foreach (var window in _windows)
        {
            window.Setup(this);
        }
    }

    private void ShowMenu<T>() where T : UIWindow
    {
        foreach (var window in _windows)
        {
            if (window is T)
            {
                if (_activeWindow)
                {
                    _previousWindow = _activeWindow;
                }

                _activeWindow = window;
                window.Show();
            }
            else
            {
                window.Hide();
            }
        }
    }

    private void ButtonBack(InputAction.CallbackContext ctx)
    {
        if (_stateChanger.CurrentState == GameState.GameLoop || _stateChanger.CurrentState == GameState.Setup)
        {
            _playerInput.Disable();
            ShowMenu<ExitWindow>();
        }
    }

    public void ButtonStart(int num)
    {
        OnSelectRoom?.Invoke(num);
        
        _stateChanger.SetState(GameState.GameLoop);
    }

    public void BackToLobby()
    {
        _stateChanger.SetState(GameState.Setup);
    }

    public void OpenSettings()
    {
        _playerInput.Disable();
        ShowMenu<SettingsWindow>();
    }

    public void ShowPreviousWindow()
    {
        _playerInput.Enable();
        foreach (var window in _windows)
        {
            window.Hide();
        }

        (_activeWindow, _previousWindow) = (_previousWindow, _activeWindow);
        _activeWindow.Show();
    }

    public bool GetSettingsValue(string key)
    {
        return _setting.Values[key];
    }

    public void ChangeSetting(string key, bool value)
    {
        _setting.Values[key] = value;
        switch (key)
        {
            case "OnSounds":
                SoundManager.Instance.IsSounds = value;
                break;
            case "OnVibration":
                if (value)
                {
                    HapticManager.Instance.PlayLightHaptic();
                }

                HapticManager.Instance.IsVibration = value;
                break;
            case "OnMusic":
                SoundManager.Instance.IsMusic = value;
                break;
        }

        _setting.SaveSettings();
    }

    public void Destroy()
    {
        _stateChanger.OnStateChanged -= OnGameStateChange;
        _playerInput.Player.Back.performed -= ButtonBack;
    }
}
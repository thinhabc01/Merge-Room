using UnityEngine;
using UnityEngine.EventSystems;

public class CamController: IWatcher, ITick
{
    private readonly IUpdater _updater;
    private readonly IGameStateEvent _stateEvent;
    private readonly CamBase[] _camBases;
    private readonly PlayerInput _input;

    private CamBase _activeCam;
    private Vector2 _startInput;
    private Vector2 _currentInput;
    private Vector3 _startPositionCam;
    private Vector3 _newPositionCam;
    private float _smoothCam;
    private float _sensitiveMoveCam;
    private float _screenSensitive;
    private bool _isTouch;
    private Quaternion _rot;

    public CamController(IUpdater updater, IGameStateEvent stateEvent, CamBase[] camBases, PlayerInput input, Vector2 settings)
    {
        _updater = updater;
        _stateEvent = stateEvent;
        _camBases = camBases;
        _input = input;
        _smoothCam = settings.x;
        _sensitiveMoveCam = settings.y;
        _screenSensitive = settings.y / Screen.height;
        
        _stateEvent.OnStateChanged += OnGameStateChange;
        
        //_updater.AddTo(this);
    }
    
    private void OnGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Setup:
                SetMain<PreviewSceneCam>();
                break;
        }
    }

    public void SetFollow<T>(Transform follow, Transform aim = null) where T : CamBase
    {
        foreach (var cam in _camBases)
        {
            if (cam is T)
                cam.SetFollow(follow, aim);
        }
    }

    public void Shake(float intensity, float duration)
    {
        _activeCam.ShakeCamera(intensity, duration);
    }

    public void SetMain<T>() where T : CamBase
    {
        foreach (var cam in _camBases)
        {
            if(cam is T)
            {
                _activeCam = cam;
                _newPositionCam = _activeCam.Position;
                _rot = Quaternion.Euler(0f, _activeCam.transform.eulerAngles.y, 0f);
                cam.SetMain();
            }
            else
            {
                cam.Hide();
            }
        }
    }

    public void Tick()
    {
        KeyboardMove();
        
        if (_input.Player.Touch.WasPressedThisFrame())
        {
            if (EventSystem.current.IsPointerOverGameObject(-1)) return;

            StartInput();
        }

        if (_input.Player.Touch.WasReleasedThisFrame())
            _isTouch = false;

        if (_isTouch)
            PerformedInput();
        
        _activeCam.Position = Vector3.Lerp(_activeCam.Position, _newPositionCam, Time.smoothDeltaTime * _smoothCam);
    }

    private void StartInput()
    {
        _isTouch = true;
        _startPositionCam = _activeCam.Position;
        _startInput = _input.Player.TouchPosition.ReadValue<Vector2>();
    }

    private void PerformedInput()
    {
        _currentInput = _input.Player.TouchPosition.ReadValue<Vector2>();

        var dif = _currentInput - _startInput;
        var offset = _rot * (new Vector3(dif.x, 0f, dif.y) * _screenSensitive);
        _newPositionCam = _startPositionCam + offset;
    }
    
    private void KeyboardMove()
    {
        var input = _input.Player.Move.ReadValue<Vector2>();

        if (input == Vector2.zero) return;
        
        var direction = _rot * (new Vector3(input.x, 0f, input.y));
        _newPositionCam = _activeCam.Position + direction * (_sensitiveMoveCam * 10 * Time.smoothDeltaTime);
    }

    public void Destroy()
    {
        //_updater.RemoveFrom(this);
        _stateEvent.OnStateChanged -= OnGameStateChange;
    }
}
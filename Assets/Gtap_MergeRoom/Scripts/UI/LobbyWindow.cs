using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyWindow : UIWindow
{
    [SerializeField] private Button _buttonOpenSettings;
    [SerializeField] private ButtonRoom[] _buttonRooms;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private bool _testMode;

    private Vector2 _defaultPosition;
    private UIManager _uiManager;
    private EventTrigger _eventTrigger;
    private Dictionary<int, ButtonState> _roomData;

    private const string _keyDataRoom = "KeyDataRoom";

    public override void Setup(UIManager uiManager)
    {
        _uiManager = uiManager;
        
        _buttonOpenSettings.onClick.AddListener(ClickButtonSettings);

        if (_testMode)
        {
            _roomData = new Dictionary<int, ButtonState>()
            {
                {0, ButtonState.Open},
                {1, ButtonState.Open},
                {2, ButtonState.Open},
                {3, ButtonState.Open},
                {4, ButtonState.Open},
                {5, ButtonState.Open},
            };
        }
        else
        {
            _roomData = ES3.Load(_keyDataRoom, new Dictionary<int, ButtonState>()
            {
                {0, ButtonState.Open},
                {1, ButtonState.Block},
                {2, ButtonState.Block},
                {3, ButtonState.Block},
                {4, ButtonState.Block},
                {5, ButtonState.Block},
            });
        }

        foreach (var button in _buttonRooms)
        {
            button.Setup(this);
            var state = _roomData[button.Number];   
            button.CurrentState = state;
        }
    }

    public void UpdateData(int num, ButtonState state)
    {
        _roomData[num] = state;

        if (state == ButtonState.Complete && _roomData.ContainsKey(num + 1) && _roomData[num + 1] == ButtonState.Block)
        {
            _roomData[num + 1] = ButtonState.Open;
            UpdateButton(num + 1, ButtonState.Open);
        }

        UpdateButton(num, state);
        
        ES3.Save(_keyDataRoom, _roomData);
    }

    private void UpdateButton(int num, ButtonState state)
    {
        foreach (var buttonRoom in _buttonRooms)
        {
            if (buttonRoom.Number == num)
                buttonRoom.CurrentState = state;
        }
    }
    
    public void ClickButtonStartRoom(int num)
    {
        UpdateData(num, ButtonState.Progress);
        
        _uiManager.ButtonStart(num);
    }

    private void ClickButtonSettings()
    {
        SoundManager.Instance.PlaySound(_audioClip);
        
        _uiManager.OpenSettings();
    }

    protected override void OnDestroy()
    {
        _buttonOpenSettings.onClick.RemoveAllListeners();
    }
}
using UnityEngine;
using UnityEngine.UI;

public class ButtonRoom : MonoBehaviour
{
    [SerializeField] private Image _imageRoom;
    [SerializeField] private Image _imageState;
    [SerializeField] private int _roomNumber;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private Sprite[] _stateSprites;
    [SerializeField] private Sprite[] _roomSprites;

    private ButtonState _currentState;
    private LobbyWindow _lobbyWindow;
    private Button _button;
    
    public int Number => _roomNumber;

    public ButtonState CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;

            switch (value)
            {
                case ButtonState.Block:
                    _imageRoom.sprite = _roomSprites[0];
                    _imageState.sprite = _stateSprites[0];
                    _button.interactable = false;
                    break;
                case ButtonState.Progress:
                    _imageRoom.sprite = _roomSprites[1];
                    _imageState.sprite = _stateSprites[1];
                    _button.interactable = true;
                    break;
                case ButtonState.Complete:
                    _imageRoom.sprite = _roomSprites[1];
                    _imageState.sprite = _stateSprites[2];
                    _button.interactable = true;
                    break;
                case ButtonState.Open:
                    _imageRoom.sprite = _roomSprites[1];
                    _imageState.sprite = _stateSprites[3];
                    _button.interactable = true;
                    break;
            }
        }
    }

    public void Setup(LobbyWindow lobbyWindow)
    {
        _lobbyWindow = lobbyWindow;
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClickButtonRoom);
    }

    private void OnClickButtonRoom()
    {
        _lobbyWindow.ClickButtonStartRoom(_roomNumber);

        SoundManager.Instance.PlaySound(_audioClip);
        HapticManager.Instance.PlayLightHaptic();
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}

public enum ButtonState
{
    Block,
    Progress,
    Complete,
    Open,
}

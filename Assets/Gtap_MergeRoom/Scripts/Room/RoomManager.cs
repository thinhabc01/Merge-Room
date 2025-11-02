using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private RoomController[] _rooms;
    
    private GridController _gridController;
    private IGameStateChanger _gameChanger;
    private UIManager _uiManager;
    private ItemController _itemController;
    private RoomController _activeRoom;
    private GameLoopWindow _loopWindow;
    private LobbyWindow _lobbyWindow;
    private GameCompleteWindow _completeWindow;
    private PlayerInput _input;
    private Camera _camera;
    
    public void Setup(IGameStateChanger gameChanger, UIManager uiManager, ItemController itemController, 
        GridController gridController, PlayerInput input)
    {
        _camera = Camera.main;
        _input = input;
        _gameChanger = gameChanger;
        _gridController = gridController;
        _itemController = itemController;
        _uiManager = uiManager;
        _loopWindow = _uiManager.GetWindow<GameLoopWindow>();
        _lobbyWindow = _uiManager.GetWindow<LobbyWindow>();
        _completeWindow = _uiManager.GetWindow<GameCompleteWindow>();
        
        _gameChanger.OnStateChanged += OnGameStateChange; 
        _uiManager.OnSelectRoom += LoadRoom;

        foreach (var room in _rooms)
        {
            room.gameObject.SetActive(false);
        }
    }

    private void OnGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Setup:
                if(_activeRoom)
                {
                    _activeRoom.Save();
                    _activeRoom.gameObject.SetActive(false);
                    _activeRoom.OnCompleteRoom -= CompleteRoom;

                    _lobbyWindow.UpdateData(_activeRoom.Num, _activeRoom.RoomComplete ? ButtonState.Complete : ButtonState.Progress);
                    _gridController.Clear();
                    _input.Enable();
                }
                break;
        }
    }
    
    private void LoadRoom(int num)
    {
        _input.Disable();
        for (int i = 0; i < _rooms.Length; i++)
        {
            _rooms[i].gameObject.SetActive(i == num);
        }

        _activeRoom = _rooms[num];
        
        _itemController.ActiveRoom = _activeRoom;
        _completeWindow.CurrentRoomSprite = _activeRoom.CompleteSprite;
        _camera.backgroundColor = _activeRoom.Back;
        _camera.orthographicSize = _activeRoom.FOV;
        
        _activeRoom.Load(_itemController, _gridController, _loopWindow, _input, num);
        _activeRoom.OnCompleteRoom += CompleteRoom;
    }

    private void CompleteRoom()
    {
        _gameChanger.SetState(GameState.GameComplete);
    }
    
    private void OnDestroy()
    {
        if (_activeRoom)
        {
            _activeRoom.OnCompleteRoom -= CompleteRoom;
            _activeRoom.Save();
        }

        _gameChanger.OnStateChanged -= OnGameStateChange;
        _uiManager.OnSelectRoom -= LoadRoom;
    }
    
#if !UNITY_EDITOR
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus == false && _activeRoom)
        {
            _activeRoom.Save();
        }
    }
#endif
}

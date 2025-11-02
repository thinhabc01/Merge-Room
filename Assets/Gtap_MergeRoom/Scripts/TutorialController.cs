using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private RoomController _firstRoom;
    [SerializeField] private GameSettings _settings;
    [SerializeField] private EItem _currentItem;
    [SerializeField] private Transform _finger;
    [SerializeField] private float _duration = 2f;
    [SerializeField] private RoomObject _targetObject;
    [SerializeField] private Transform _fakeTarget;
    
    private IGameStateEvent _gameEvent;
    private Transform _first, _second;
    private ItemController _itemController;
    private GridController _gridController;
    private float _interpolator;
    
    public void Setup(IGameStateEvent gameEvent, GridController gridController, ItemController itemController)
    {
        _gameEvent = gameEvent;
        _itemController = itemController;
        _gridController = gridController;
        
        _firstRoom.OnFillGrid += StartTutorial;
        _itemController.OnSpawnAfterMerge += CheckMerge;
        _targetObject.OnComletedRoom += FinishTutorial;
        _gameEvent.OnStateChanged += OnGameStateChange;
        
        _finger.gameObject.SetActive(true);
    }

    private void OnGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Setup:
                FinishTutorial(_targetObject);
                break;
        }
    }
    
    private void StartTutorial()
    {
        _firstRoom.OnFillGrid -= StartTutorial;
        _settings.Values["Tutorial"] = false;
        _settings.SaveSettings();

        FindMerge();
    }

    private void CheckMerge(EItem current, EItem next)
    {
        if (_currentItem == current)
        {
            _currentItem = next;

            FindMerge();
        }
    }
    
    private void FindMerge()
    {
        _first = null;
        _second = null;
        foreach (var cell in _gridController.Cells)
        {
            var eItem = cell.Item.EItem;
            if (_currentItem == eItem)
            {
                if(_first == null)
                    _first = cell.Item.transform;
                else if (_second == null)
                    _second = cell.Item.transform;
            }
        }

        if(_second == null)
            _second = _fakeTarget;

        _finger.position = _first.position;
    }

    private void Update()
    {
        if(_first == null || _second == null) return;
        
        if (_interpolator > _duration)
        {
            _interpolator = 0f;
            _finger.position = _first.position;
        }

        _interpolator += Time.deltaTime;

        _finger.position = Vector3.Lerp(_first.position, _second.position, _interpolator / _duration);
    }

    private void Unsubscribe()
    {
        _targetObject.OnComletedRoom -= FinishTutorial;
        _firstRoom.OnFillGrid -= StartTutorial;
        
        if(_gameEvent != null)
            _gameEvent.OnStateChanged -= OnGameStateChange;
        
        if(_itemController != null)
            _itemController.OnSpawnAfterMerge -= CheckMerge;
    }
    
    private void FinishTutorial(RoomObject roomObject)
    {
        _finger.gameObject.SetActive(false);
        _first = null;

        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}

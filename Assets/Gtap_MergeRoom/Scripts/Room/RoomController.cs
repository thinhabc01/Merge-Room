using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private string _roomNameKey = "Room_00";
    [SerializeField] private float _delaySpawn = 0.1f;
    [SerializeField] private Sprite _completeSprite;
    [SerializeField] private Color _background;
    [SerializeField] private float _fov;
    [SerializeField] private GameSettings _settings;

    [Space(20)]
    [SerializeField] private StageRoom[] _stage;

    private List<EItem> _itemsGrid;
    private List<int> _roomObjectsComplete;
    private List<EItem> _queue;
    private int _currentNumQueue;
    private int _currentNumStage;
    private int _countRoomObjects;
    private int _progress;
    private bool _firstTime;
    
    private PlayerInput _input;
    private GridController _gridController;
    private ItemController _itemController;
    private StageRoom _activeStage;
    private GameLoopWindow _loopWindow;
    private WaitForSeconds _delay;
    private AudioClip _openObjectClip;

    public Sprite CompleteSprite => _completeSprite;
    public Color Back => _background;
    public float FOV => _fov;
    public bool RoomComplete => _currentNumStage >= _stage.Length;
    public int Num { get; private set; }
    public event Action OnCompleteRoom;
    public event Action OnFillGrid;

    private int Progress
    {
        get  => _progress;
        set
        {
            _progress = value;

            var valueBar = Mathf.InverseLerp(0, _countRoomObjects, _progress);
            _loopWindow.SetValueProgress(valueBar, value, _countRoomObjects);
        }
    }

    public void Load(ItemController itemController, GridController gridController, GameLoopWindow loopWindow, PlayerInput input, int num)
    {
        Num = num;
        _openObjectClip = _settings.OpenObjectClip;
        _delay = new WaitForSeconds(_delaySpawn);
        _loopWindow = loopWindow;
        _input = input;
        _itemController = itemController;
        _gridController = gridController;
        
        _countRoomObjects = 0;
        Progress = 0;

        LoadSave();
        LoadStage();
        StartStage(false);
    }

    private void LoadSave()
    {
        var saveRoom = ES3.Load(_roomNameKey, new SaveRoom()
        {
            NumQueue = 0,
            NumStage = 0,
            RoomObjects = new List<int>(),
            ItemsGrid = new List<EItem>(),
            FirstTime = true,
        });

        _roomObjectsComplete = new List<int>();
        _roomObjectsComplete.AddRange(saveRoom.RoomObjects);
        
        _itemsGrid = new List<EItem>();
        _itemsGrid.AddRange(saveRoom.ItemsGrid);
        
        _currentNumStage = saveRoom.NumStage;
        _currentNumQueue = saveRoom.NumQueue;
        _firstTime = saveRoom.FirstTime;
        
        StartRoomEvent();
    }
    
    private void LoadStage()
    {
        if (RoomComplete)
            ResetRoom();
        
        for (int i = 0; i < _stage.Length; i++)
        {
            _stage[i].gameObject.SetActive(i < _currentNumStage);

            if (i < _currentNumStage)
            {
                Progress += _stage[i].RoomObjects.Length;

                foreach (var roomObject in _stage[i].RoomObjects)
                {
                    roomObject.Completed(false);
                }
            }
            
            _countRoomObjects += _stage[i].RoomObjects.Length;
        }
    }
    
    private void StartStage(bool feedback)
    {
        _input.Disable();
        
        _queue = new List<EItem>();
        _activeStage = _stage[_currentNumStage];
        _activeStage.gameObject.SetActive(true);
        _activeStage.ShowProps(feedback);
        
        foreach (var roomObject in _activeStage.RoomObjects)
        {
            if (_roomObjectsComplete.Contains(roomObject.ID))
            {
                roomObject.Completed(false);
                Progress++;
            }
            else
            {
                roomObject.OnComletedRoom += RoomObjectComplete;
            }
        }

        Progress = _progress;

        foreach (var eItem in _itemsGrid)
            _itemController.SpawnItem(eItem);

        for (int i = _currentNumQueue; i < _activeStage.Queue.Length; i++)
            _queue.Add(_activeStage.Queue[i]);

        StartCoroutine(FillGrid());
    }

    private void ResetRoom()
    {
        _currentNumStage = 0;
        foreach (var stage in _stage)
        {
            stage.ResetObject();
        }
    }
    
    private IEnumerator FillGrid()
    {
        yield return new WaitForSeconds(0.5f);
        
        var cell = _gridController.GetFreeCell();
        while (!ReferenceEquals(cell, null) && _queue.Count > 0)
        {
            var eItem = GetEItemQueue();
            _itemController.SpawnItem(eItem, cell);
            
            cell = _gridController.GetFreeCell();
            
            yield return _delay;
        }
        
        _input.Enable();
        OnFillGrid?.Invoke();
    }

    private void RoomObjectComplete(RoomObject roomObject)
    {
        roomObject.OnComletedRoom -= RoomObjectComplete;

        var eItem = GetEItemQueue();
        if (eItem != EItem.None)
            _itemController.SpawnItem(eItem);
        
        _roomObjectsComplete.Add(roomObject.ID);
        Progress++;

        SoundManager.Instance.PlaySound(_openObjectClip, volume: 0.7f);
        
        CheckStageComplete();
    }

    private void CheckStageComplete()
    {
        if (_activeStage.IsComplete())
        {
            _currentNumStage++;
            _currentNumQueue = 0;
            _itemsGrid.Clear();
            _roomObjectsComplete.Clear();
            
            if (RoomComplete)
            {
                CompleteRoomEvent();
                
                _firstTime = false;
                OnCompleteRoom?.Invoke();
            }
            else
            {
                StartStage(true);
            }
        }
    }

    private void StartRoomEvent()
    {
    }
    
    private void CompleteRoomEvent()
    {
    }
    
    public ItemData GetItemData(EItem eItem)
    {
        return _activeStage.GetItemData(eItem);
    }

    public EItem GetEItemQueue()
    {
        if (_queue.Count == 0) return EItem.None;
        
        _currentNumQueue++;
        return _queue.Dequeue();
    }
    
    public void Save()
    {
        ES3.Save(_roomNameKey, new SaveRoom()
        {
            NumQueue = _currentNumQueue,
            NumStage = _currentNumStage,
            RoomObjects = _roomObjectsComplete,
            ItemsGrid = _gridController.GetCurrentItem(),
            FirstTime = _firstTime,
        });
    }

    private void Dispose()
    {
        if(_activeStage)
            foreach (var roomObject in _activeStage.RoomObjects)
                roomObject.OnComletedRoom -= RoomObjectComplete;
        
        StopAllCoroutines();
    }
    
    private void OnDisable() => Dispose();
    private void OnDestroy() => Dispose();

    [Serializable]
    private struct SaveRoom
    {
        public int NumQueue;
        public int NumStage;
        public List<int> RoomObjects;
        public List<EItem> ItemsGrid;
        public bool FirstTime;
    }
}
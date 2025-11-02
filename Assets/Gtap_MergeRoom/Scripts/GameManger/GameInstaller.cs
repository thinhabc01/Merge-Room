using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameInstaller : MonoBehaviour, IUpdater
{
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private RoomManager _roomManager;
    [SerializeField] private TutorialController _tutorialController;

    [Space(10)]
    [SerializeField] private PreLaunchItem[] _preLaunchPrefabs;
    
    private GameManager _gameManager;
    private PlayerInput _playerInput;
    private CamController _camController;
    private GridController _gridController;
    private SelectionController _selectionController;
    private ItemController _itemController;
    private UIManager _uiManager;
    private Canvas _mainCanvas;
    private float _playLengthTime;
    
    private HashSet<ITick> _ticks = new HashSet<ITick>();
    private HashSet<ITickFixed> _ticksFixed = new HashSet<ITickFixed>();
    private HashSet<ITickLate> _ticksLate = new HashSet<ITickLate>();

    private void Awake()
    {
        _gameSettings.LoadSettings();
        
        _mainCanvas = GetComponentInChildren<Canvas>();

        var containerPoolObjects = new GameObject("ContainerPoolObjects");
        PoolManager.ClearPool();
        for (int i = 0; i < _preLaunchPrefabs.Length; i++)
        {
            PoolManager.PoolPreLaunch(_preLaunchPrefabs[i].Prefab, _preLaunchPrefabs[i].Amount, containerPoolObjects.transform);
        }

        SoundManager.Instance.Setup(_gameSettings.Values);
        HapticManager.Instance.Setup(_gameSettings.Values);
        
        _playerInput = new PlayerInput();
        _gameManager = new GameManager(_gameSettings);
        _uiManager = new UIManager(_gameManager, _mainCanvas, _playerInput, _gameSettings);
        _selectionController = new SelectionController(this, _gameManager, _playerInput, _gameSettings.GridCellsSettings);
        _gridController = new GridController(_gameSettings.GridCellsSettings, _roomManager.transform);
        _itemController = new ItemController(_gridController, _gameSettings, _selectionController, _gameSettings);
        
        _roomManager.Setup(_gameManager, _uiManager, _itemController, _gridController, _playerInput);
    }

    private void Start()
    {
        _gameManager.SetState(GameState.Setup);
        
        SoundManager.Instance.PlayMusic(_gameSettings.MusicClip, volume: 0.5f);
        _playerInput.Enable();
    }

    private void Update()
    {
        ITick[] tickArray = _ticks.ToArray();
		
        for (var i = 0; i < tickArray.Length; i++)
        {
            tickArray[i].Tick();
        }

        if (_gameManager.CurrentState == GameState.GameLoop && AdsController.Instance.CurrentTime < 0f)
        {
            AdsController.Instance.ShowInterstitialAds();
        }
    }

    private void FixedUpdate()
    {
        ITickFixed[] tickFixedArray = _ticksFixed.ToArray();
		
        for (var i = 0; i < tickFixedArray.Length; i++)
        {
            tickFixedArray[i].TickFixed();
        }
    }

    private void LateUpdate()
    {
        ITickLate[] tickLateArray = _ticksLate.ToArray();
		
        for (var i = 0; i < tickLateArray.Length; i++)
        {
            tickLateArray[i].TickLate();
        }
    }

    private void OnDestroy()
    {
        _selectionController?.Destroy();
        _uiManager?.Destroy();
        _camController?.Destroy();
        _gameManager?.Destroy();
        _itemController?.Destroy();
    }

    public void AddTo(object tick)
    {
        switch (tick)
        {
            case ITick tickDefault:
                _ticks.Add(tickDefault);
                break;
            case ITickFixed tickFixed:
                _ticksFixed.Add(tickFixed);
                break;
            case ITickLate late:
                _ticksLate.Add(late);
                break;
        }
    }

    public void RemoveFrom(object tick)
    {
        switch (tick)
        {
            case ITick tickDefault:
                _ticks.Remove(tickDefault);
                break;
            case ITickFixed tickFixed:
                _ticksFixed.Remove(tickFixed);
                break;
            case ITickLate late:
                _ticksLate.Remove(late);
                break;
        }
    }

#if UNITY_EDITOR
    [Button]
    private void LevelComplete()
    {
        _gameManager.SetState(GameState.GameComplete);
    }
#endif
}

[Serializable]
public struct PreLaunchItem
{
    public GameObject Prefab;
    public int Amount;
}
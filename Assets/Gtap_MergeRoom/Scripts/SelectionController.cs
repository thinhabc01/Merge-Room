using System;
using UnityEngine;

public class SelectionController: ITick
{
    private readonly IUpdater _updater;
    private readonly IGameStateEvent _stateEvent;
    private readonly PlayerInput _input;
    private readonly Camera _cameraGrid;
    private readonly Camera _cameraMain;
    private readonly LayerMask _layerCell;
    private readonly LayerMask _layerSlot;
    private readonly float _elevatedValue;
    
    private Vector3 _touchPosition;
    private Item _selectedItem;
    private Cell _selectedCell, _currentCell;
    private RoomObject _currentRoomObject;
    private bool _isDrag;

    public event Action<Item, Item, Cell> OnMergeEvent;

    public SelectionController(IUpdater updater, IGameStateEvent stateEvent, PlayerInput input, GridCellsSettings settings)
    {
        _updater = updater;
        _stateEvent = stateEvent;
        _input = input;

        _layerCell = settings.LayerCell;
        _layerSlot = settings.LayerSlot;
        _elevatedValue = settings.ElevatedValue;
        
        _cameraGrid = GameObject.FindGameObjectWithTag("CameraGrid").GetComponent<Camera>();
        _cameraGrid.gameObject.SetActive(false);
        
        _cameraMain = Camera.main;

        _stateEvent.OnStateChanged += OnGameStateChange;
    }

    private void OnGameStateChange(GameState state)
    {
        if (state == GameState.GameLoop)
        {
            _updater.AddTo(this);
            _cameraGrid.gameObject.SetActive(true);
        }
        else
        {
            _updater.RemoveFrom(this);
            _cameraGrid.gameObject.SetActive(false);
        }
    }
    
    public void Tick()
    {
        if (_input.Player.Touch.WasPressedThisFrame())
        {
            SelectCell();
        }

        if (_input.Player.Touch.IsPressed() && _isDrag)
        {
            DragItem();
        }

        if (_input.Player.Touch.WasReleasedThisFrame() && _isDrag)
        {
            DropItem();
        }
    }

    private void SelectCell()
    {
        var ray = _cameraGrid.ScreenPointToRay(_input.Player.TouchPosition.ReadValue<Vector2>());
        
        if (Physics.Raycast(ray, out var hit, 100, _layerCell))
        {
            _selectedCell = hit.collider.GetComponent<Cell>();
            
            if (ReferenceEquals(_selectedCell.Item, null)) return;
            
            _selectedItem = _selectedCell.Item;
            _touchPosition = _selectedItem.transform.position;
            _selectedCell.Item = null;
            _isDrag = true;
            
            if(_selectedItem.NextItem == EItem.None)
                _selectedItem.AnimationShow(true);
        }
    }

    private void DragItem()
    {
        var touch = _input.Player.TouchPosition.ReadValue<Vector2>();
        var ray = _cameraGrid.ScreenPointToRay(touch);

        MoveItemModel(touch);

        if (Physics.Raycast(ray, out var hit, 100, _layerCell))
        {
            var cell = hit.collider.GetComponent<Cell>();

            if (_currentCell == cell) return;
            
            if (_currentCell)
                _currentCell.Used = false;

            _currentCell = cell;
            _currentCell.Used = true;

            cell.ReadyMerge = ReferenceEquals(cell.Item, null) || CheckReadyMerge(cell.Item);
        }

        if (_selectedItem.NextItem == EItem.None)
            CheckSlot(touch);
    }

    private void CheckSlot(Vector2 touch)
    {
        var ray = _cameraMain.ScreenPointToRay(touch);

        if (Physics.Raycast(ray, out var hit, 100, _layerSlot))
        {
            var roomObject = hit.collider.GetComponent<RoomObject>();
            
            if (_currentRoomObject && _currentRoomObject != roomObject)
                _currentRoomObject.Highlight = false;

            _currentRoomObject = roomObject;
            
            if (_selectedItem.EItem == _currentRoomObject.EItem)
                _currentRoomObject.Highlight = true;
        }
        else
        {
            if(_currentRoomObject)
            {
                _currentRoomObject.Highlight = false;
                _currentRoomObject = null;
            }
        }
    }

    private bool CheckReadyMerge(Item itemRaycast)
    {
        if (itemRaycast.EItem != _selectedItem.EItem || itemRaycast.NextItem == EItem.None)
            return false;

        return true;
    }
    
    private void DropItem()
    {
        var ray = _cameraGrid.ScreenPointToRay(_input.Player.TouchPosition.ReadValue<Vector2>());

        if (Physics.Raycast(ray, out var hit, 100, _layerCell))
        {
            var cell = hit.collider.GetComponent<Cell>();
            
            if (cell == _selectedCell || ReferenceEquals(cell.Item, null))
            {
                cell.Item = _selectedItem;
                _selectedItem.AnimationShow(false);
                
                ResetSelect();
                return;
            }

            if (!ReferenceEquals(cell.Item, null))
            {
                if (cell.ReadyMerge == true)
                {
                    OnMergeEvent?.Invoke(cell.Item, _selectedItem, cell); 
                }
                else
                {
                    _selectedCell.Item = cell.Item;
                    cell.Item = _selectedItem;
                    _selectedItem.AnimationShow(false);
                }

                ResetSelect();
                return;
            }
        }

        if (_currentRoomObject && _currentRoomObject.Highlight)
        {
            PoolManager.SetPool(_selectedItem);
            _currentRoomObject.Completed(true);
            ResetSelect();
            return;
        }
        
        _selectedCell.Item = _selectedItem;
        _selectedItem.AnimationShow(false);
        ResetSelect();
    }

    private void ResetSelect()
    {
        _isDrag = false;
        if (_currentCell)
        {
            _currentCell.Used = false;
        }

        if (_selectedCell)
        {
            _selectedCell.Used = false;
        }

        _selectedItem = null;
        _currentCell = null;
        _selectedCell = null;
    }

    private void MoveItemModel(Vector2 touch)
    {
        var pos = _selectedItem.transform.position;
        var distance = Vector3.Distance(pos, _cameraGrid.transform.position);

        _touchPosition = _cameraGrid.ScreenToWorldPoint(new Vector3(touch.x, touch.y, distance));
        var newPos = new Vector3(_touchPosition.x, _elevatedValue, _touchPosition.z);
        _selectedItem.transform.position = Vector3.Lerp(pos, newPos, 16f * Time.deltaTime);
    }

    public void Destroy()
    {
        _stateEvent.OnStateChanged -= OnGameStateChange;

        _updater.RemoveFrom(this);
        
        ResetSelect();
    }
}
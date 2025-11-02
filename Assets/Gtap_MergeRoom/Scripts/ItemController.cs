using System;
using UnityEngine;

public class ItemController
{
    private readonly GridController _gridController;
    private readonly SelectionController _selectionController;
    private readonly ParticleController _mergeFX;
    private readonly Item _itemPrefab;
    private readonly AudioClip _mergeClip;
    
    public RoomController ActiveRoom { get; set; }

    public event Action<EItem, EItem> OnSpawnAfterMerge;

    public ItemController(GridController gridController, GameSettings gameSettings, SelectionController selectionController, GameSettings settings)
    {
        _gridController = gridController;
        _selectionController = selectionController;
        _mergeClip = settings.MergeClip;
        _itemPrefab = gameSettings.Item;
        _mergeFX = gameSettings.MergeFX;
        
        _selectionController.OnMergeEvent += MergeItem;
    }

    private void MergeItem(Item first, Item second, Cell cell)
    {
        var mergeItem = first.EItem;
        var nextItem = first.NextItem;
        
        PoolManager.SetPool(first);
        PoolManager.SetPool(second);

        PoolManager.GetPool(_mergeFX, cell.transform.position + Vector3.up);
        HapticManager.Instance.PlayLightHaptic();
        SoundManager.Instance.PlaySound(_mergeClip, volume: 0.6f);

        SpawnItem(nextItem, cell);

        var eItem = ActiveRoom.GetEItemQueue();
        if (eItem != EItem.None)
            SpawnItem(eItem);
        
        OnSpawnAfterMerge?.Invoke(mergeItem, nextItem); }
    
    public void SpawnItem(EItem eItem, Cell cell = null)
    {
        cell ??= _gridController.GetFreeCell();
        
        if (ReferenceEquals(cell, null)) return;
        
        var data = ActiveRoom.GetItemData(eItem);
        
        var item = PoolManager.GetPool(_itemPrefab, cell.transform.position);
        item.Setup(data);
        item.transform.SetParent(_gridController.Container);
        
        cell.Item = item;
    }

    public void Destroy()
    {
        _selectionController.OnMergeEvent -= MergeItem;
    }
}

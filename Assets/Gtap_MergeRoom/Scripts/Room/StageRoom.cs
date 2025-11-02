using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StageRoom : MonoBehaviour
{
    [SerializeField] private RoomObject[] _roomObjects;
    [SerializeField] private Transform[] _props;
    
    [Space(10)]
    [SerializeField] private EItem[] _queue;

    [Space(10)]
    [SerializeField] private List<ItemData> _items;

    public RoomObject[] RoomObjects => _roomObjects;
    public EItem[] Queue => _queue;
    
    public ItemData GetItemData(EItem eItem)
    {
        foreach (var data in _items)
        {
            if (data.Type == eItem)
                return data;
        }

        return new ItemData();
    }

    public bool IsComplete()
    {
        foreach (var room in _roomObjects)
        {
            if (room.IsCompleted == false)
                return false;
        }

        return true;
    }

    public void ResetObject()
    {
        foreach (var roomObject in _roomObjects)
        {
            roomObject.Reset();
        }
    }
    
    public void ShowProps(bool feedback)
    {
        if (feedback == false)
            return;
        
        foreach (var prop in _props)
        {
            var scaleTarget = prop.localScale;
            prop.localScale = Vector3.zero;
            prop
                .DOScale(scaleTarget, 0.7f)
                .SetEase(Ease.OutBack);
        }    
    }

    private void KillTweens()
    {
        foreach (var prop in _props)
        {
            prop.DOKill();
        }  
    }

    private void OnDisable() => KillTweens();
    private void OnDestroy() => KillTweens();

#if UNITY_EDITOR
    [ContextMenu("FillQueue")]
    private void FillQueue()
    {
        var queueTemp = new List<EItem>();

        foreach (var data in _items)
        {
            if (data.Next == EItem.None)
            {
                var eItem = data.Type;
                var child = eItem;

                while (child != EItem.None)
                {
                    child = FindChild(eItem);
                    
                    if(child != EItem.None)
                    {
                        queueTemp.Add(child);
                        eItem = child;
                    }
                    else
                    {
                        queueTemp.Add(eItem);
                    }
                }
            }
        }

        _queue = queueTemp.ToArray();
    }

    private EItem FindChild(EItem eItemParent)
    {
        foreach (var data in _items)
        {
            if (data.Next == eItemParent)
                return data.Type;
        }

        return EItem.None;
    }
#endif
}

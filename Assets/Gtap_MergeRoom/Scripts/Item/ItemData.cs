using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct ItemData
{
    public Sprite Sprite;
    public EItem Type;
    public EItem Next;
    [ShowIf("Next", EItem.None)] public float Scale;
}
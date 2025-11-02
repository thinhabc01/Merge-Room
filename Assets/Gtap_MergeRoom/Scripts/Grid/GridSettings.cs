using System;
using UnityEngine;

[Serializable]
public struct GridCellsSettings
{
    public Vector3 GlobalOffset;
    public Vector2Int Size;
    public Vector2 OffsetCells;
    public Cell CellPrefab;
    public LayerMask LayerCell;
    public LayerMask LayerSlot;
    public float ElevatedValue;
}

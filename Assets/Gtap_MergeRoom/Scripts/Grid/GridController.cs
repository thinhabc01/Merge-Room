using System.Collections.Generic;
using UnityEngine;

public class GridController
{
    private readonly GridCellsSettings _gridSettings;

    private List<Cell> _cells = new List<Cell>();
    private Transform _gridContainer;

    public Transform Container { get; private set; }

    public Cell[] Cells => _cells.ToArray();

    public GridController(GridCellsSettings gridCellsSettings, Transform container)
    {
        Container = container;
        _gridSettings = gridCellsSettings;
        
        CreateGridCells();
    }
    
    public Cell GetFreeCell()
    {
        foreach (var cell in _cells)
        {
            if (ReferenceEquals(cell.Item, null))
            {
                return cell;
            }
        }

        return null;
    }

    public List<EItem> GetCurrentItem()
    {
        List<EItem> items = new List<EItem>();

        foreach (var cell in _cells)
        {
            if(cell.Item)
                items.Add(cell.Item.EItem);
        }

        return items;
    }

    public void Clear()
    {
        foreach (var cell in _cells)
        {
            cell.Clear();
        }
    }
    
    private void CreateGridCells()
    {
        _gridContainer = new GameObject("GridContainer").transform;
        _gridContainer.SetParent(Container);
        var gridSize = _gridSettings.Size;
        var offsetCell = _gridSettings.OffsetCells;
        var position = new Vector3(gridSize.x * offsetCell.x * 0.5f, 0f, 0f); 
        
        for (int i = 0; i < gridSize.x; i++)
        {
            position.z = gridSize.y * offsetCell.y * 0.5f;
            
            for (int j = 0; j < gridSize.y; j++)
            {
                var cell = PoolManager.GetPool(_gridSettings.CellPrefab, position);
                _cells.Add(cell);
                cell.transform.SetParent(_gridContainer);

                position.z -= offsetCell.y;
            }

            position.x -= offsetCell.x;
        }

        _gridContainer.position = _gridSettings.GlobalOffset;
    }
}
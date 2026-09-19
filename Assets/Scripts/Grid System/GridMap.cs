using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridMapData
{
    public string nameGridMap;
    public int widthCell;
    public int heightCell;
    public float cellSize;
    public Transform origin;
    public Transform container;
    
    [SerializeField] private GridZone[] zoneData;

    public GridZone GetZone(int x, int y)
    {
        EnsureArraySize();
        if (x < 0 || y < 0 || x >= widthCell || y >= heightCell) return GridZone.None;
        return zoneData[x + y * widthCell];
    }

    public void SetZone(int x, int y, GridZone zone)
    {
        EnsureArraySize();
        if (x < 0 || y < 0 || x >= widthCell || y >= heightCell) return;
        zoneData[x + y * widthCell] = zone;
    }
    
    public bool Contains(Vector3 worldPosition)
    {
        if (origin == null) return false;
        Vector3 local = worldPosition - origin.position;
        float maxX = widthCell * cellSize;
        float maxY = heightCell * cellSize;
        return local.x >= 0 && local.y >= 0 && local.x < maxX && local.y < maxY;
    }
    
    public void EnsureArraySize()
    {
        int required = Mathf.Max(0, heightCell * widthCell);
        if (zoneData != null && zoneData.Length == required) return;
        
        GridZone[] newArr = new GridZone[required];
        if (zoneData != null)
            Array.Copy(zoneData, newArr, Mathf.Min(zoneData.Length, required));
        zoneData = newArr;
    }
}

[System.Serializable]
public class GridCellData
{
    public GridZone zone;
    public Tower occupant;
    
    public bool IsOccupied => occupant != null;
    public override string ToString() => zone.ToString();
}

public class GridMap : MonoBehaviour
{
    [SerializeField] private List<GridMapData> gridMapData = new();
    [SerializeField] private GridZone gridZone;
    
    [Header("Highlight sorting")]
    [SerializeField] private int highlightSortingOrder = 11; // above dim background
    [SerializeField] private Sprite highlightSprite;

    [SerializeField] private bool showDebugGrid;
    
    private List<Grid<GridCellData>> _grid = new();
    public List<GridMapData> GetGridMapDataList() => gridMapData;
    
    
    private void Start()
    {
        _grid.Clear();
        
        foreach (var mapData in gridMapData)
        {
            Grid<GridCellData> newGrid = new Grid<GridCellData>(mapData.widthCell, mapData.heightCell, mapData.cellSize, mapData.origin.position, mapData.container, mapData.nameGridMap, showDebugGrid);

            mapData.EnsureArraySize();
            for (int x = 0; x < mapData.widthCell; x++)
            for (int y = 0; y < mapData.heightCell; y++)
                newGrid.SetGridObject(x, y, new GridCellData { zone = mapData.GetZone(x, y) });
 
            _grid.Add(newGrid);
        }
    }
    
    private int GetMapIndex(Vector3 worldPosition)
    {
        for (int i = 0; i < gridMapData.Count; i++)
        {
            if (gridMapData[i].Contains(worldPosition))
                return i;
        }
        return -1;
    }

    #region Main Grid Map System 

    public GridCellData GetOrCreateCell(int mapIndex, Vector3 worldPosition)
    {
        var cellData = _grid[mapIndex].GetGridObject(worldPosition);
        return cellData ?? new GridCellData();
    }
    
    public void SetZone(Vector3 worldPosition, GridZone zone)
    {
        int mapIndex = GetMapIndex(worldPosition);
        if (mapIndex < 0 || mapIndex >= _grid.Count) return;
        
        var cell = GetOrCreateCell(mapIndex, worldPosition);
        cell.zone = zone;
        _grid[mapIndex].SetGridObject(worldPosition, cell);
    }
    
    public GridZone GetZoneAt(Vector3 worldPosition)
    {
        int mapIndex =  GetMapIndex(worldPosition);
        if (mapIndex < 0 || mapIndex >= _grid.Count) return GridZone.None;
        
        var cell = _grid[mapIndex].GetGridObject(worldPosition);
        return cell != null ? cell.zone : GridZone.None;
    }

    public Vector3 GetGridPosition(Vector3 worldPosition)
    {
        int mapIndex = GetMapIndex(worldPosition);
        if (mapIndex < 0 || mapIndex >= _grid.Count) return worldPosition;
        
        return _grid[mapIndex].GetGridPosition(worldPosition);
    }

    public float GetCellSizeAt(Vector3 worldPosition)
    {
        int mapIndex = GetMapIndex(worldPosition);
        if (mapIndex < 0 || mapIndex >= _grid.Count) return 0;

        Debug.Log("Cell Size : " + _grid[mapIndex].GetGridCellSize(worldPosition));
        
        return _grid[mapIndex].GetGridCellSize(worldPosition);
    }
    #endregion
    
    
    public void HighlightZone(GridZone zoneToShow, bool active)
    {
        for (int i = 0; i < gridMapData.Count; i++)
        {
            var mapData = gridMapData[i];
            for (int x = 0; x < mapData.widthCell; x++)
            for (int y = 0; y < mapData.heightCell; y++)
            {
                if (mapData.GetZone(x, y) == zoneToShow)
                    _grid[i].SetCellHighlight(x, y, active, highlightSortingOrder, highlightSprite, Color.white);
            }
        }
    }

    #region Grid Map Tower System
    
    public void SetTower(Vector3 worldPosition, Tower tower)
    {
        int mapIndex = GetMapIndex(worldPosition);
        if (mapIndex < 0 || mapIndex >= _grid.Count) return;

        var cell = GetOrCreateCell(mapIndex, worldPosition);
        cell.occupant = tower;
        _grid[mapIndex].SetGridObject(worldPosition, cell);

        SetZone(worldPosition, GridZone.Occupied);
        
    }

    public Tower GetTowerAt(Vector3 worldPosition)
    {
        int mapIndex = GetMapIndex(worldPosition);
        if (mapIndex < 0 || mapIndex >= _grid.Count) return null;

        var cell = _grid[mapIndex].GetGridObject(worldPosition);
        return cell?.occupant;
    }

    public void ClearTower(Vector3 worldPosition, GridZone? resetZone = null)
    {
        int mapIndex = GetMapIndex(worldPosition);
        if (mapIndex < 0 || mapIndex >= _grid.Count) return;
        
        var cell = GetOrCreateCell(mapIndex, worldPosition);
        cell.occupant = null;
        if (resetZone.HasValue) cell.zone = resetZone.Value;
        _grid[mapIndex].SetGridObject(worldPosition, cell);
    }

    #endregion
    
    public bool TryGetNearestZoneCell(Vector3 worldPosition, GridZone allowZone, out Vector3 result)
    {
        result = worldPosition;
        float bestSqrDist = float.MaxValue;
        bool found = false;

        for (int i = 0; i < _grid.Count; i++)
        {
            var mapData = gridMapData[i];
            var grid = _grid[i];
            
            for (int x = 0; x < mapData.widthCell; x++)
            for (int y = 0; y < mapData.heightCell; y++)
            {
                var cell = grid.GetGridObject(x, y);
                if (cell == null || cell.zone != allowZone) continue;

                Vector3 cellCenter = grid.GetGridPosition(x, y);
                // 2D distance, ignore Z
                float sqrDist = ((Vector2)cellCenter - (Vector2)worldPosition).sqrMagnitude;

                if (sqrDist < bestSqrDist)
                {
                    bestSqrDist = sqrDist;
                    result = cellCenter;
                    found = true;
                }
            }
        }
        
        return found;
    }
    
    private void OnDrawGizmos()
    {
        if (gridMapData == null) return;

        foreach (var mapData in gridMapData)
        {
            if (mapData.origin == null) continue;
            
            Vector3 startPos = mapData.origin.position;
            
            for (int x = 0; x <= mapData.widthCell; x++)
            {
                Vector3 start = startPos + new Vector3(x * mapData.cellSize, 0, 0);
                Vector3 end = start + new Vector3(0, mapData.heightCell * mapData.cellSize, 0);
                Gizmos.DrawLine(start, end);
            }
            
            for (int y = 0; y <= mapData.heightCell; y++)
            {
                Vector3 start = startPos + new Vector3(0, y * mapData.cellSize, 0);
                Vector3 end = start + new Vector3(mapData.widthCell * mapData.cellSize, 0, 0);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}

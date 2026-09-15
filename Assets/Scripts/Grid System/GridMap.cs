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

public class GridMap : MonoBehaviour
{
    [SerializeField] private List<GridMapData> gridMapData = new();
    [SerializeField] private GridZone gridZone;
    
    [Header("Highlight sorting")]
    [SerializeField] private int highlightSortingOrder = 11; // above dim background
    [SerializeField] private Sprite highlightSprite;

    [SerializeField] private bool showDebugGrid;
    
    private List<Grid<GridZone>> _grid = new();
    public List<GridMapData> GetGridMapDataList() => gridMapData;
    
    private void Start()
    {
        _grid.Clear();
        
        foreach (var mapData in gridMapData)
        {
            Grid<GridZone> newGrid = new Grid<GridZone>(mapData.widthCell, mapData.heightCell, mapData.cellSize, mapData.origin.position, mapData.container, mapData.nameGridMap, showDebugGrid);
            
            // Apply the zones painted in the editor onto the freshly created runtime grid.
            mapData.EnsureArraySize();
            for (int x = 0; x < mapData.widthCell; x++)
            for (int y = 0; y < mapData.heightCell; y++)
                newGrid.SetGridObject(x, y, mapData.GetZone(x, y));
 
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

    public void SetZone(Vector3 worldPosition, GridZone zone)
    {
        int mapIndex = GetMapIndex(worldPosition);
        if (mapIndex < 0 || mapIndex >= _grid.Count) return;
        _grid[mapIndex].SetGridObject(worldPosition, zone);
    }
    
    public GridZone GetZoneAt(Vector3 worldPosition)
    {
        int mapIndex =  GetMapIndex(worldPosition);
        
        if (mapIndex < 0 || mapIndex >= _grid.Count) return GridZone.None;
        
        return _grid[mapIndex].GetGridObject(worldPosition);
    }

    public Vector3 GetGridPosition(Vector3 worldPosition)
    {
        int mapIndex = GetMapIndex(worldPosition);
        if (mapIndex < 0 || mapIndex >= _grid.Count) return worldPosition;
        
        return _grid[mapIndex].GetGridPosition(worldPosition);
    }

    public void HighlightZone(GridZone zoneToShow, bool active)
    {
        for (int i = 0; i < gridMapData.Count; i++)
        {
            var mapData = gridMapData[i];
            for (int x = 0; x < mapData.widthCell; x++)
            for (int y = 0; y < mapData.heightCell; y++)
            {
                if (mapData.GetZone(x, y) == zoneToShow)
                    _grid[i].SetCellHighlight(x, y, active, highlightSortingOrder, highlightSprite, Color.yellow);
            }
        }
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

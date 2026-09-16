
using System;
using UnityEngine;

[System.Serializable]
public enum GridZone
{
    None,
    Pedestrian,
    Vehicle,
    Water,
    Build,
    Occupied
}

[System.Serializable]
public enum GridMode
{
    None,
    Building
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private GridMap buildingGridMap;

    [Header("Highlight Building Grid Zones")]
    [SerializeField] private GridMode currentGridMode;
    [SerializeField] private GameObject dimBackground;
    
    public GridMode CurrentGridMode => currentGridMode;
    public GridMap BuildingGridMap => buildingGridMap;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    // Building Position
    public Vector3 GridToWorldPosition(Vector3 worldPosition)
    {
        return buildingGridMap.GetGridPosition(worldPosition);
    }
    
    public void GetGridMapZoneCell(Vector3 worldPosition, out GridZone gridZone)
    {
        var gridMapData = buildingGridMap.GetZoneAt(worldPosition);

        gridZone = gridMapData;
        
        Debug.Log(gridMapData);
    }

    public void HighlightBuildGridZone()
    {
        if (currentGridMode != GridMode.Building)
        {
            currentGridMode = GridMode.Building;
            buildingGridMap.HighlightZone(GridZone.Build, true);

            dimBackground.SetActive(true);
        }
    }

    public void UnhighlightBuildGridZone()
    {
        if (currentGridMode != GridMode.Building) return;
        
        currentGridMode = GridMode.None;
        buildingGridMap.HighlightZone(GridZone.Build, false);

        dimBackground.SetActive(false);
    }
    
    // Enemy Path Finding
    public void GetGridMapZoneCell()
    {
        
    }
}

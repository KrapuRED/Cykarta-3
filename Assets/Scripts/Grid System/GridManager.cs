
using System;
using UnityEngine;

[System.Serializable]
public enum GridZone
{
    None,
    Pedestrian,
    Vehicle,
    Water,
    Build
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
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HighlightBuildGridZone();
        }
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
            
            // Find all grid zone that building
            // Change Sorting layer to Higher of that grid zone where type of building zone 
            buildingGridMap.HighlightZone(GridZone.Build, true);
           
            // Show dimHighlight 
            dimBackground.SetActive(true);
        }
        else
        {
            currentGridMode = GridMode.None;
            
            // Find all grid zone that building
            // Change Sorting layer to lower of that grid zone where type of building zone
            buildingGridMap.HighlightZone(GridZone.Build, false);
            
            // Show dimHighlight 
            dimBackground.SetActive(false);
        }
    }
    
    // Enemy Path Finding
    public void GetGridMapZoneCell()
    {
        
    }
}

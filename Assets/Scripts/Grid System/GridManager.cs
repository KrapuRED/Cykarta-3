using UnityEngine;
using UtilTools;

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
    Building,
    Confirmation
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private GridMap buildingGridMap;

    [Header("Highlight Building Grid Zones")]
    [SerializeField] private GridMode currentGridMode;
    [SerializeField] private GameObject dimBackground;
    [SerializeField] private GridPlacement gridPlacement;
    
    public GridMode CurrentGridMode => currentGridMode;
    public GridMap BuildingGridMap => buildingGridMap;
    public Tower InstancePreviewTower { get; private set; }

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
        var gridZoneData = buildingGridMap.GetZoneAt(worldPosition);
        
        gridZone =  gridZoneData;
    }

    public void HighlightBuildGridZone()
    {
        if (gridPlacement.PrefabTower == null) return;
        if (currentGridMode == GridMode.Confirmation) return;

        if (currentGridMode != GridMode.Building)
        {
            currentGridMode = GridMode.Building;
            buildingGridMap.HighlightZone(GridZone.Build, true);
            GameEvents.OnRequestOpenPanel.Invoke(PanelType.Building);
            dimBackground.SetActive(true);
        }

        if (InstancePreviewTower != null)
            Destroy(InstancePreviewTower.gameObject);

        InstancePreviewTower = Instantiate(gridPlacement.PrefabTower,
            UtilTools.UtilsClass.GetMouseWorldPositionWithZ(), Quaternion.identity);
    }

    public void UnhighlightBuildGridZone()
    {
        if (InstancePreviewTower != null)
        {
            Destroy(InstancePreviewTower.gameObject);
            InstancePreviewTower = null;
        }
        
        gridPlacement.ClearTowerPlacement();
    }

    public void CancelBuild()
    {
        if (InstancePreviewTower != null)
        {
            InstancePreviewTower.RemoveTower();
            InstancePreviewTower = null;
        }
        
        gridPlacement.ClearTowerPlacement();
    }
    
    // Enemy Path Finding
    public void GetGridMapZoneCell(GridZone gridZone)
    {
        
    }

    public void ChangeGridMode(GridMode gridMode)
    {
        if (currentGridMode != gridMode && gridMode == GridMode.None)
        {
            UnhighlightBuildGridZone();
            buildingGridMap.HighlightZone(GridZone.Build, false);

            dimBackground.SetActive(false);
            GameEvents.OnRequestClosePanel.Invoke(PanelType.Building);
        }
        
        currentGridMode = gridMode;
        
    }
}

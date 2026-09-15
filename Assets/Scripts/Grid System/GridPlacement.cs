using UnityEngine;

public class GridPlacement : MonoBehaviour
{
    [SerializeField] private Transform towerContainer;
    [SerializeField] private GameObject prefabTower;

    public void PlaceTower(Vector3 gridPosition)
    {
        if (GridManager.Instance.CurrentGridMode != GridMode.Building) return;
        
        //Check if the grid is already have building
        var gridZone =  GridManager.Instance.BuildingGridMap.GetZoneAt(gridPosition);
        if (gridZone == GridZone.Occupied)
            return;
        
        // Take Grid Position
        
        // Instantiate prefab
        
        if (prefabTower == null)
        {
            Debug.LogWarning($"[{name} (PlaceTower)] GridPlacement: prefabTower is not assigned.");
            return;
        }
 
        GridManager.Instance.BuildingGridMap.SetZone(gridPosition, GridZone.Occupied);
        Instantiate(prefabTower, gridPosition, Quaternion.identity, towerContainer);
    }
}

using UnityEngine;

public class GridPlacement : MonoBehaviour
{
    [SerializeField] private Transform towerContainer;
    [SerializeField] private Tower prefabTower;

    public Tower PrefabTower => prefabTower;
    
    private void OnEnable()
    {
        GameEvents.OnShowTowerCardDetail.AddListener(SetTowerPlacement);
        GameEvents.OnHideTowerCardDetail.AddListener(CancelGridPlacement);
    }

    private void OnDisable()
    {
        GameEvents.OnShowTowerCardDetail.RemoveListener(SetTowerPlacement);
        GameEvents.OnHideTowerCardDetail.RemoveListener(CancelGridPlacement);
    }

    private void SetTowerPlacement(TowerDataSO towerData)
    {
        prefabTower = towerData.prefabObjectTower;
    }

    public void CancelGridPlacement()
    {
        prefabTower = null;
    }
    
    public void PlaceTower(Vector3 gridPosition)
    {
        if (GridManager.Instance.CurrentGridMode != GridMode.Building) return;
        
        //Check if the grid is already have building
        var gridZone =  GridManager.Instance.BuildingGridMap.GetZoneAt(gridPosition);
        if (gridZone == GridZone.Occupied)
            return;
        
        if (prefabTower == null)
        {
            Debug.LogWarning($"[{name} (PlaceTower)] GridPlacement: prefabTower is not assigned.");
            return;
        }
 
        Tower newTower = Instantiate(prefabTower, gridPosition, Quaternion.identity, towerContainer);
        
        GridManager.Instance.BuildingGridMap.SetTower(gridPosition, newTower);
    }

    public void GetActiveTower(Vector3 gridPosition)
    {
        var tower = GridManager.Instance.BuildingGridMap.GetTowerAt(gridPosition);
        if (tower == null) return;
        
        GameEvents.OnShowTowerCardUpgrade.Invoke(tower);
        Debug.Log($"[{name} (PlaceTower)] Selected tower : {tower.name} ID : {tower.towerID}");
    }
    
    public void RemoveTower(Vector3 gridPosition, GridZone resetZone = GridZone.Build)
    {
        var tower = GridManager.Instance.BuildingGridMap.GetTowerAt(gridPosition);
        if (tower == null) return;

        GridManager.Instance.BuildingGridMap.ClearTower(gridPosition, resetZone);
        Destroy(tower.gameObject);
    }
}

using UnityEngine;

public class GridPlacement : MonoBehaviour
{
    [SerializeField] private Transform towerContainer;
    [SerializeField] private GameObject prefabTower;

    public void PlaceTower(Vector3 gridPosition)
    {
        if (GridManager.Instance.CurrentGridMode != GridMode.Building) return;
        
        // Take Grid Position
        
        // Instantiate prefab
        
        if (prefabTower == null)
        {
            Debug.LogWarning($"[{name} (PlaceTower)] GridPlacement: prefabTower is not assigned.");
            return;
        }
 
        Instantiate(prefabTower, gridPosition, Quaternion.identity, towerContainer);
    }
}

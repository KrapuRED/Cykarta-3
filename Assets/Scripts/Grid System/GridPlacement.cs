using UnityEngine;

public class GridPlacement : MonoBehaviour
{
    [SerializeField] private Transform towerContainer;
    [SerializeField] private GameObject prefabTower;

    public void PlaceTower(Vector3 gridPosition)
    {
        // Take Grid Position
        
        // Instantiate prefab
        
        if (prefabTower == null)
        {
            Debug.LogWarning("GridPlacement: prefabTower is not assigned.");
            return;
        }
 
        Instantiate(prefabTower, gridPosition, Quaternion.identity, towerContainer);
    }
}

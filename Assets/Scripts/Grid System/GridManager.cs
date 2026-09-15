
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

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private GridMap buildingGridMap;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_grid = new Grid<bool>(widthCell, heightCell, cellSize, origin.position, container);
    }

    // Building Position
    public Vector3 GridToWorldPosition(Vector3 worldPosition)
    {
        return buildingGridMap.GetGridPosition(worldPosition);
    }
    
    public void GetGridMapZoneCell(Vector3 worldPosition, out GridMap gridMap, out GridZone gridZone)
    {
        var gridMapData = buildingGridMap.GetZoneAt(worldPosition);

        gridMap = null;
        gridZone = gridMapData;
        
        Debug.Log(gridMapData);
    }

    // Enemy Path Finding
    public void GetGridMapZoneCell()
    {
        
    }
}

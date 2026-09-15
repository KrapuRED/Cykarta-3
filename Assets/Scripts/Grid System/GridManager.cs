
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

    public void IncreaseValueCell(Vector3 worldPosition)
    {

    }

    public void GetValueCell(Vector3 worldPosition)
    {
        var gridMapData = buildingGridMap.GetZoneAt(worldPosition);
        Debug.Log(gridMapData);
    }
}

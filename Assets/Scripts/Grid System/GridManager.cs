using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private int widthCell;
    [SerializeField] private int heightCell;
    [SerializeField] private int cellSize = 10;
    [SerializeField] private Transform origin;
    [SerializeField] private Transform container;
    
    private Grid<bool> _grid;

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
        _grid = new Grid<bool>(widthCell, heightCell, cellSize, origin.position, container);
    }

    public void IncreaseValueCell(Vector3 worldPosition)
    {
        _grid.SetGridObject(worldPosition, true);
    }

    public void GetValueCell(Vector3 worldPosition)
    {
       var gridValue = _grid.GetGridObject(worldPosition);
       Debug.Log(gridValue);
    }
}

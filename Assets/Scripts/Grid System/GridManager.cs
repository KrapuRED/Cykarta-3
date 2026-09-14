using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private int widthCell;
    [SerializeField] private int heightCell;
    [SerializeField] private int cellSize = 10;
    [SerializeField] private Transform origin;
    
    private Grid _grid;

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
        _grid = new Grid(widthCell, heightCell, cellSize, origin.position);
    }

    public void IncreaseValueCell(Vector3 worldPosition)
    {
        _grid.SetValue(worldPosition, 67);
    }

    public void GetValueCell(Vector3 worldPosition)
    {
        Debug.Log(_grid.GetValue(worldPosition));
    }
}

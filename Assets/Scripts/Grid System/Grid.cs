using System;
using TMPro;
using UnityEngine;
using UtilTools;

[Serializable]
public class OnGridObjectChangeEventArgs : EventArgs
{
    public int x;
    public int y;
}

public class Grid <TGridObject>
{
    public event EventHandler<OnGridObjectChangeEventArgs> OnGridObjectChange; 
    
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private Vector3 gridPosition;
    private TGridObject[,] gridArray;
    private TextMeshPro[,] debugTextArray;
    
    public Grid(int width, int height, float cellSize, Vector3 originPosition, Transform gridContianer, string nameObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        
        gridArray = new TGridObject[width, height];

        debugTextArray = new TextMeshPro[width, height];
        
        for (int x = 0; x < gridArray.GetLength(0); x++)
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                string newNameObject = $"{nameObject}({x},{y})";
                gridPosition = GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f;
                
                debugTextArray[x,y] = UtilTools.UtilsClass.CreateWorldText(gridArray[x, y].ToString(), gridContianer, gridPosition, 3
                    ,Color.white, sortingOrder:2 
                    ,boxSize: new Vector2(cellSize, cellSize),
                    customeName:newNameObject);
                
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x,  y) * cellSize +  originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }
    
    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            if (OnGridObjectChange != null) OnGridObjectChange(this, new OnGridObjectChangeEventArgs{x = x, y = y});
            debugTextArray[x, y].text =  gridArray[x, y].ToString();
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public void TriggerGridObject(int x, int y)
    {
        if (OnGridObjectChange != null) OnGridObjectChange(this, new OnGridObjectChangeEventArgs{x = x, y = y});
    }
    
    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return  gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public Vector3 GetGridPosition(Vector3 worldPosition)
    {
        GetXY(worldPosition, out int x, out int y);
        return GetGridPosition(x, y);
    }
    
    public Vector3 GetGridPosition(int x, int y)
    {
        return GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid
{

    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    private TextMesh[,] debugTextArray;
    private Vector3 originPosition;
    private bool debugMode;

    public MyGrid(int width, int height, float cellSize, Vector3 originPosition, bool debugMode)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.debugMode = debugMode;

        gridArray = new int[width, height];

        //DEBUG -> Text
        Transform textParent = null;
        if (debugMode)
        {
            debugTextArray = new TextMesh[width, height];
            textParent = GameObject.Find("GridValues").transform;
        }


        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {

                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100);
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100);
        }
    }



    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetCellCenterPosition(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        Vector3 position = GetWorldPosition(x, y);
        return new Vector3(position.x + cellSize / 2, position.y, position.z + cellSize / 2);
    }

    public Vector3 GetCellCenterPosition(int x, int y)
    {
        Vector3 worldPosition = GetWorldPosition(x, y);
        return new Vector3(worldPosition.x + cellSize /2 , worldPosition.y , worldPosition.z + cellSize / 2);
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + originPosition;
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            gridArray[x, y] = value;

            // DEBUG -> Text
            if (debugMode)
            {
                debugTextArray[x, y].text = gridArray[x, y].ToString();
            }

        }
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);

    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return gridArray[x, y];
        }
        return -1;
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return gridArray[x, y];
        }
        return -1;
    }

}

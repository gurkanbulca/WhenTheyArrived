using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoffeeRush.Utils;

public class GridManager : MonoBehaviour
{
    private MyGrid grid;

    public Vector3 origin;
    public int gridWidth;
    public int gridHeight;
    public float cellSize;
    public LayerMask layerMask;
    public bool debugMode;

    #region Singleton
    public static GridManager instace;

    private void Awake()
    {
        if(instace != null)
        {
            Debug.LogError("Instance already casted!");
            return;
        }
        instace = this;
    }

    #endregion

    void Start()
    {
        origin = new Vector3(origin.x - gridWidth * cellSize / 2, origin.y, origin.z - gridHeight * cellSize / 2);
        grid = new MyGrid(gridWidth, gridHeight,cellSize,origin, debugMode);

    }

    

    public MyGrid GetGrid()
    {
        return grid;
    }

    public void UpdateCellValues()
    {
        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                Vector3 position = grid.GetCellCenterPosition(x, y);
                Collider[] entities = Physics.OverlapBox(position, (Vector3.one * cellSize / 2) * 0.95f ,Quaternion.identity,layerMask);

                grid.SetValue(position, entities.Length);
            }
        }
    }

   

}

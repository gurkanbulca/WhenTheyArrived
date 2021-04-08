using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoRectSizer : MonoBehaviour
{
    public int rowCount;
    public int columnCount;
    public GridLayoutGroup pocketLayout;
    public RectTransform line;

    private static int[,] Grid = new[,]
    {
        {1,1,1,3,2},
        {1,1,1,1,2},
        {2,1,1,1,3},
        {2,2,2,1,2},
        {3,2,2,2,2}
    };

    public void Start()
    {
        FindMatch(1, 1);
    }

    void Update()
    {
        GridLayoutGroup layout = GetComponent<GridLayoutGroup>();
        RectTransform rect = GetComponent<RectTransform>();
        float height = rect.rect.height;
        height -= (layout.spacing.y * (rowCount - 1)) + layout.padding.top + layout.padding.bottom;
        layout.cellSize = new Vector2(height / rowCount, height / rowCount);
        float width = rect.rect.width;
        width -= (layout.spacing.x * (columnCount - 1)) + layout.padding.right + layout.padding.left;
        Vector2 alternateSize = new Vector2(width / columnCount, width / columnCount);
        if (alternateSize.magnitude < layout.cellSize.magnitude)
        {
            layout.cellSize = alternateSize;
        }
        pocketLayout.cellSize = layout.cellSize;

        line.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (layout.cellSize.x * columnCount) + (layout.spacing.x * (columnCount)));
    }

    public static List<List<int>> FindMatch(int x, int y)
    {
        // Use Grid
        int valueOfCell = Grid[x, y];
        List<List<int>> result = new List<List<int>>();
        if (x > 0)
        {
            if (Grid[x - 1, y] == valueOfCell)
            {
                result.Add(new List<int> { x - 1, y });
            }
        }
        if (x < 4)
        {
            if (Grid[x + 1, y] == valueOfCell)
            {
                result.Add(new List<int> { x + 1, y });
            }
        }
        if (y > 0)
        {
            if (Grid[x, y - 1] == valueOfCell)
            {
                result.Add(new List<int> { x, y - 1 });
            }
        }

        if (y < 4)
        {
            if (Grid[x, y + 1] == valueOfCell)
            {
                result.Add(new List<int> { x, y + 1 });
            }
        }
        foreach (List<int> item in result)
        {
        }
        return result;

    }



}

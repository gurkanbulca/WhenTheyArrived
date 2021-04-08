using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicRectSize : MonoBehaviour
{




    public void SetHeight()
    {
        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        RectTransform rect = GetComponent<RectTransform>();
        int childCount = transform.childCount;
        Vector2 spacing = gridLayout.spacing;
        Vector2 cellSize = gridLayout.cellSize;

        float width = rect.rect.width;
        int columnCount = Mathf.FloorToInt((width + spacing.x) / (cellSize.x + spacing.x));
        int rowCount = Mathf.CeilToInt((float)childCount / columnCount);
        float calculatedHeight = gridLayout.padding.top + gridLayout.padding.bottom + ((cellSize.y + spacing.y) * rowCount - spacing.y);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, calculatedHeight);
        rect.pivot = rect.anchorMin;
    }

    public void SetWidth()
    {
        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        RectTransform rect = GetComponent<RectTransform>();
        int childCount = transform.childCount;
        Vector2 spacing = gridLayout.spacing;
        Vector2 cellSize = gridLayout.cellSize;

        float height = rect.rect.height;
        int rowCount = Mathf.FloorToInt((height + spacing.y) / (cellSize.y + spacing.y));
        int columnCount = Mathf.FloorToInt((float)childCount / rowCount);
        float calculatedWidth = gridLayout.padding.left + gridLayout.padding.right + ((cellSize.x + spacing.x) * columnCount - spacing.x);
        rect.sizeDelta = new Vector2(calculatedWidth, rect.sizeDelta.y);
    }




}

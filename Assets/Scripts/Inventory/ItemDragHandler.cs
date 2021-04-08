using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    GameObject icon;

    GameObject copyIcon;
    Inventory inventory;

    void Start()
    {
        inventory = Inventory.instance;
        icon = GetComponent<InventorySlot>().icon.gameObject;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        inventory.draggingItemIndex = transform.GetSiblingIndex();
        if(transform.parent != InventoryUI.instance.baseParent)
        {
            inventory.draggingItemIndex += InventoryUI.instance.baseParent.childCount;
        }
        copyIcon = Instantiate(icon,transform.parent.parent.parent);
        copyIcon.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
        copyIcon.GetComponent<Image>().raycastTarget = false;
        copyIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(icon.GetComponent<RectTransform>().rect.width, icon.GetComponent<RectTransform>().rect.height);
        copyIcon.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        copyIcon.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if(inventory.draggingItemIndex != -1)
        {
        copyIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (inventory.draggingItemIndex != -1)
        {
            inventory.draggingItemIndex = -1;
            Destroy(copyIcon);
        }


    }
}

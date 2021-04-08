using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    GameObject icon;
    StorageUI storageUI;

    private void Start()
    {
        storageUI = StorageUI.instance;
        icon = GetComponent<InventorySlot>().icon.gameObject;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GetComponent<InventorySlot>().item != null)
        {
            StorageController storage = storageUI.storage;
            storage.draggingIndex = transform.GetSiblingIndex();
            Transform baseStorageParent = GetComponent<StorageDropHandler>().baseStorageParent;
            if (transform.parent != baseStorageParent)
            {
                storage.draggingIndex += baseStorageParent.childCount;
            }
            storageUI.copyIcon = Instantiate(icon, transform.parent.parent);
            storageUI.copyIcon.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
            storageUI.copyIcon.GetComponent<Image>().raycastTarget = false;
            storageUI.copyIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(icon.GetComponent<RectTransform>().rect.width, icon.GetComponent<RectTransform>().rect.height);
            storageUI.copyIcon.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            storageUI.copyIcon.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(storageUI.storage.draggingIndex != -1)
        {
            storageUI.copyIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(storageUI.storage.draggingIndex != -1)
        {
            storageUI.storage.draggingIndex = -1;
            Destroy(storageUI.copyIcon);
            storageUI.copyIcon = null;
        }
    }
}

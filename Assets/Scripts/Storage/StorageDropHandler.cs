using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StorageDropHandler : MonoBehaviour, IDropHandler
{

    Inventory inventory;
    public Transform baseStorageParent;

    private void Start()
    {
        inventory = Inventory.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        StorageController storage = StorageUI.instance.storage;
        int draggingInventoryIndex = inventory.draggingItemIndex;
        int draggingStorageIndex = storage.draggingIndex;
        int to = transform.parent != baseStorageParent
               ? transform.GetSiblingIndex() + baseStorageParent.childCount
               : transform.GetSiblingIndex();
        if (draggingInventoryIndex != -1)
        {
            storage.Add(inventory.items[draggingInventoryIndex],
                inventory.items[draggingInventoryIndex].amount,
                to);
        }
        else if(draggingStorageIndex != -1)
        {
            
            storage.Swap(storage.items[draggingStorageIndex],
                storage.items[draggingStorageIndex].amount,
                draggingStorageIndex,
                to);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageUI : MonoBehaviour
{

    #region Singleton

    public static StorageUI instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    [SerializeField]
    Transform BaseSlots;
    [SerializeField]
    Transform ExtraSlots;


    [HideInInspector]
    public GameObject copyIcon;
    public StorageController storage;

    
    public void UpdateStorage(Item[] items,StorageController storage)
    {
        this.storage = storage;
        for(int i = 0; i < BaseSlots.childCount; i++)
        {
            BaseSlots.GetChild(i).GetComponent<InventorySlot>().SetSlot(items[i]);
        }
        for (int i = 0; i < items.Length - BaseSlots.childCount; i++)
        {
            InventorySlot slot = ExtraSlots.GetChild(i).GetComponent<InventorySlot>();
            slot.UnlockSlot();
            slot.SetSlot(items[i + BaseSlots.childCount]);
        }
        for (int i = items.Length - BaseSlots.childCount; i < ExtraSlots.childCount; i++)
        {
            InventorySlot slot = ExtraSlots.GetChild(i).GetComponent<InventorySlot>();
            slot.LockSlot();
        }
    }

    public void ClearHighlights()
    {
        for(int i = 0;i < BaseSlots.childCount; i++)
        {
            BaseSlots.GetChild(i).GetComponent<InventorySlot>().ClearHighlight();
        }

        for (int i = 0; i < ExtraSlots.childCount; i++)
        {
            ExtraSlots.GetChild(i).GetComponent<InventorySlot>().ClearHighlight();
        }
    }

}

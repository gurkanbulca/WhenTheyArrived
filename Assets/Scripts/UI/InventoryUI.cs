using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    #region Singleton

    public static InventoryUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion


    public Transform baseParent,extraParent;
    [SerializeField]
    GameObject inventoryUI;

    Inventory inventory;
    InventorySlot[] baseSlots,extraSlots;
    WindowManager windowManager;

    private void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        windowManager = MultiuseUI.instace.windowManager;
        baseSlots = baseParent.GetComponentsInChildren<InventorySlot>();
        extraSlots = extraParent.GetComponentsInChildren<InventorySlot>();
        UpdateUI();
    }

    

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            windowManager.OpenPanel("Equipments");
        }
    }

 


    void UpdateUI()
    {
        Item[] items = Inventory.instance.items;
        for(int i = 0; i < baseSlots.Length; i++)
        {
            baseSlots[i].GetComponent<InventorySlot>().SetSlot(items[i]);
        }
        for(int i = baseSlots.Length; i < items.Length; i++)
        {
            extraSlots[i - baseSlots.Length].GetComponent<InventorySlot>().UnlockSlot();
            extraSlots[i - baseSlots.Length].GetComponent<InventorySlot>().SetSlot(items[i]);
        }
        for(int i = items.Length - baseSlots.Length; i < extraSlots.Length; i++)
        {
            extraSlots[i].GetComponent<InventorySlot>().LockSlot();

        }
    }

    public bool UpdateDurability(Equipment equipment)
    {
        Item[] items = inventory.items;
        if (equipment.GetCurrentDurability() <= 0)
        {
            return inventory.DestroyItem(equipment);
        }
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == equipment)
            {
                if (i < baseSlots.Length)
                {
                    baseSlots[i].GetComponent<InventorySlot>().UpdateDurability();
                }
                else
                {
                    extraSlots[i - baseSlots.Length].GetComponent<InventorySlot>().UpdateDurability();
                }
                return true;
            }
        }
        return false;
    }

    public void ClearHighlights()
    {
        foreach (var slot in baseSlots)
        {
            slot.ClearHighlight();
        }

        foreach (var slot in extraSlots)
        {
            slot.ClearHighlight();
        }
    }


}

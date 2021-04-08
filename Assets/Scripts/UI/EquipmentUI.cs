﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public Transform EquipmentsParent;
    public GameObject equipmentUI;

    EquipmentManager equipmentManager;
    Inventory inventory;

    EquipmentSlot[] slots;
    ConsumableSlot[] pockets;

    private void Start()
    {
        equipmentManager = EquipmentManager.instance;
        inventory = Inventory.instance;
        equipmentManager.onEquipmentChanged += UpdateUI;

        slots = EquipmentsParent.GetComponentsInChildren<EquipmentSlot>();
        pockets = EquipmentsParent.GetComponentsInChildren<ConsumableSlot>();
    }

    private void Update()
    {
    }


    void UpdateUI(Item newItem, Item oldItem,int slotIndex = -1)
    {
        if (slotIndex == -1)
        {
            if (oldItem != null)
            {
                    EquipmentSlot targetSlot = FindSuitableSlot((Equipment)oldItem);
                    targetSlot.clearSlot();

            }
            if (newItem != null)
            {
                    EquipmentSlot targetSlot = FindSuitableSlot((Equipment)newItem);
                    targetSlot.AddItem((Equipment)newItem);

            }
        }
        else
        {
            if(oldItem != null)
            {
                pockets[slotIndex].clearSlot();
            }
            if (newItem != null)
            {
                pockets[slotIndex].AddItem((Consumable)newItem);
            }
        }
       

    }

    public EquipmentSlot FindSuitableSlot(Equipment equipment)
    {
        EquipmentSlot result = null;
        foreach (EquipmentSlot slot in slots)
        {
            if (slot.slotType == equipment.equipSlot)
            {
                result = slot;
            }
        }
        return result;
    }
}

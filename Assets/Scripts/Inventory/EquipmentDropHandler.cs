using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentDropHandler : MonoBehaviour,IDropHandler
{
    EquipmentManager equipmentManager;
    Inventory inventory;

    void Start()
    {
        equipmentManager = EquipmentManager.instance;
        inventory = Inventory.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(inventory.draggingItemIndex != -1)
        {
            if(GetComponent<ConsumableSlot>() != null)
            {
                Consumable consumable = inventory.items[inventory.draggingItemIndex] as Consumable; 
                if(consumable!= null)
                {
                    //POCKETS
                    equipmentManager.EquipConsumable(consumable,GetComponent<ConsumableSlot>().slotIndex);
                    //inventory.Remove(consumable,consumable.amount,inventory.draggingItemIndex);
                    return;
                }
            }

            Equipment draggingEquipment = inventory.items[inventory.draggingItemIndex] as Equipment;
            if (GetComponent<EquipmentSlot>() != null)
            {
                if (draggingEquipment?.equipSlot == GetComponent<EquipmentSlot>()?.slotType)
                {
                    EquipProcess(draggingEquipment);
                }
            }
            
        }
        else if (equipmentManager.draggingEquipmentIndex != -1)
        {
            Equipment draggingEquipment = equipmentManager.currentEquipment[equipmentManager.draggingEquipmentIndex];
            if (draggingEquipment != null)
            {
                if ((int)draggingEquipment.equipSlot == transform.GetSiblingIndex())
                {
                    EquipProcess(draggingEquipment);
                }
            }
            
        }
        else if (equipmentManager.draggingConsumable != null)
        {
            Consumable consumable = equipmentManager.draggingConsumable;
            if (GetComponent<ConsumableSlot>()!=null)
            {
                equipmentManager.UnequipConsumable(consumable);
                inventory.Remove(consumable, consumable.amount);
                equipmentManager.EquipConsumable(consumable, GetComponent<ConsumableSlot>().slotIndex);
                return;
            }
        }
    }

    private void EquipProcess(Equipment item)
    {
        equipmentManager.Equip(item);
        inventory.Remove(item);
    }

    

    
}

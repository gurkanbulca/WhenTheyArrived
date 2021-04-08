using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public Image icon;
    Equipment item;
    public EquipmentTypes slotType;

    public void AddItem(Equipment newItem)
    {
        if (newItem.equipSlot == slotType)
        {
            item = newItem;
            icon.sprite = item.icon;
            icon.enabled = true;
        }
        
    }

    public void clearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }


    public void OnRemoveButton()
    {
        EquipmentManager.instance.Unequip((int)item.equipSlot);
    }
}

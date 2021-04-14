using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public Image icon;
    [HideInInspector]
    public Equipment item;
    public EquipmentTypes slotType;

    public void AddItem(Equipment newItem)
    {
        if (newItem.equipSlot == slotType)
        {
            item = newItem;
            icon.sprite = item.icon;
            icon.enabled = true;
            SetDurabilityBar();
        }
        
    }

    public void SetDurabilityBar()
    {
        if (item != null)
        {
            transform.Find("Durability")?.GetComponent<ProgressBarPro>().SetValue(item.GetCurrentDurability(), item.GetMaxDurability());
        }
        else
        {
            transform.Find("Durability")?.GetComponent<ProgressBarPro>().SetValue(0,1);
        }
    }

    public void clearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        SetDurabilityBar();
    }


    public void OnRemoveButton()
    {
        EquipmentManager.instance.Unequip((int)item.equipSlot);
    }
}

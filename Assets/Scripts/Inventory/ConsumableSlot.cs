using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableSlot : MonoBehaviour
{
    public GameObject amount;
    public GameObject placeholder;
    public Image icon;
    public int slotIndex;
    [HideInInspector]
    public Consumable item;


    public void AddItem(Consumable newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        amount.GetComponent<TMP_Text>().text = newItem.amount.ToString();
        placeholder.SetActive(false);

    }

    public void clearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        amount.GetComponent<TMP_Text>().text = "";
        placeholder.SetActive(true);
    }

    public Consumable GetItem()
    {
        return item;
    }


    public void OnRemoveButton()
    {
        EquipmentManager.instance.UnequipConsumable(item);
    }
}

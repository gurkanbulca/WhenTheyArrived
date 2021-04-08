using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsumableScreenSlot : MonoBehaviour
{
    Consumable consumable;
    Button button;

    public Image icon;
    public TMP_Text amount;
    public int id;

    void Start()
    {
        EquipmentManager.instance.onEquipmentChanged += UpdateSlotUI;
        button = GetComponent<Button>();
    }

    void UpdateSlotUI(Item newItem, Item oldItem, int slotIndex = -1)
    {
        consumable = EquipmentManager.instance.pockets[id];
        if(consumable != null)
        {
            button.interactable = true;
            icon.enabled = true;
            icon.sprite = consumable.icon;
            amount.text = consumable.amount.ToString();
        }
        else
        {
            button.interactable = false;
            icon.enabled = false;
            amount.text = "";
        }
        
    }

    public void OnButtonClick()
    {
        consumable.Use();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductionSlot : MonoBehaviour
{
    [SerializeField]
    GameObject placeholder;
    [SerializeField]
    TMP_Text amount;
    public Image image;
    public Item item;
    public StationController parentStation;
    public ProductionSlotType slotType;

    public void SetSlot(Item item,int itemAmount,StationController station)
    {
        this.parentStation = station;
        this.item = item;
        if (item == null)
        {
            placeholder.SetActive(true);
            amount.text = "";
            image.enabled = false;
        }
        else
        {
            placeholder.SetActive(false);
            amount.text = itemAmount.ToString();
            image.enabled = true;
            image.sprite = item.icon;
        }

    }

}

public enum ProductionSlotType
{
    Input, Output
}

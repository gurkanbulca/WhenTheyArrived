using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SmelterSlot : MonoBehaviour
{
    [SerializeField]
    GameObject placeholder;
    [SerializeField]
    TMP_Text amount;
    public Image image;
    [HideInInspector]
    public Item item;
    [HideInInspector]
    public SmelterController parentSmelter;
    public SmelterSlotType slotType;

    public void SetSlot(Item item, int itemAmount, SmelterController smelter)
    {
        this.parentSmelter = smelter;
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

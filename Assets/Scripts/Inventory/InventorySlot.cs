using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    [SerializeField]
    GameObject amountText;
    [SerializeField]
    Sprite lockSprite;
    [SerializeField]
    Image highlight;
    [HideInInspector]
    public Item item;

    private void OnEnable()
    {
        ClearHighlight();
    }

    private void OnDisable()
    {
        ClearHighlight();
        Inventory.instance.selectedSlot = this;

    }

    public void SetSlot(Item item)
    {
        if (item != null)
        {
            AddItem(item);
        }
        else
        {
            clearSlot();
        }
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        if (newItem.isStackable)
        {
            amountText.GetComponent<TMP_Text>().text = newItem.amount.ToString();
            amountText.GetComponent<TMP_Text>().enabled = true;
        }
        else
        {
            amountText.GetComponent<TMP_Text>().enabled = false;
        }
    }

    public void clearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        amountText.GetComponent<TMP_Text>().enabled = false;
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item,item.amount,this.transform.GetSiblingIndex());
    }

    public void SelectSlot()
    {
        Inventory.instance.selectedSlot = this;
        InventoryUI.instance.ClearHighlights();
        StorageUI.instance.ClearHighlights();
        HighlightSlot();
    }

    void HighlightSlot()
    {
        highlight.enabled = true;

    }

    public void ClearHighlight()
    {
        highlight.enabled = false;
    }



    public void SplitAmount()
    {
        if (item?.amount > 1 && !Inventory.instance.IsFull())
        {
            int splitedAmount = item.amount / 2;
            item.amount -= splitedAmount;
            Item newItem = Instantiate(item);
            int index = Inventory.instance.FindEmptySlot(transform.GetSiblingIndex() + 1);
            Inventory.instance.Add(newItem, splitedAmount,index);
        }
    }

    public void LockSlot()
    {
        Image image = GetComponent<Image>();
        image.sprite = lockSprite;
        GetComponent<Button>().enabled = false;
        clearSlot();
    }

    public void UnlockSlot()
    {
        Image image = GetComponent<Image>();
        image.sprite = null;
        GetComponent<Button>().enabled = true;

    }

}

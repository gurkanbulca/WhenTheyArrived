using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageController : MonoBehaviour,IItemContainer
{
    [SerializeField]
    int inventorySize;
    public Item[] items;

    WindowManager windowManager;
    GameObject multiUseUI;
    [HideInInspector]
    public int draggingIndex=-1;
    Inventory inventory;

    private void Start()
    {
        items = new Item[inventorySize];
        multiUseUI = MultiuseUI.instace.multiuseUI;
        windowManager = MultiuseUI.instace.windowManager;
        inventory = Inventory.instance;
    }

    public void Interact()
    {
        if (!multiUseUI.activeSelf)
        {
            multiUseUI.SetActive(true);
            windowManager.OpenPanel("Storage");
            StorageUI.instance.UpdateStorage(items,this);
        }
    }

    public bool Add(Item newItem, int amount, int inventorySlot = -1)
    {
        bool result = true;
        if(inventorySlot > -1)
        {
            if(items[inventorySlot] == null)
            {
                if (newItem.stackSize >= amount)
                {
                    items[inventorySlot] = newItem;
                    items[inventorySlot].amount = amount;
                    inventory.DestroyItem(newItem);
                }
                else
                {
                    amount -= newItem.stackSize;
                    items[inventorySlot] = newItem;
                    items[inventorySlot].amount = newItem.stackSize;
                    newItem.amount = amount;
                    inventory.onItemChangedCallback?.Invoke();
                    result = Add(newItem, amount);
                }
            }
            else
            {
                if (items[inventorySlot].name.Equals(newItem.name))
                {
                    if(items[inventorySlot].amount+amount <= newItem.stackSize)
                    {
                        items[inventorySlot].amount += amount;
                        inventory.DestroyItem(newItem);
                    }
                    else
                    {
                        amount -= (items[inventorySlot].stackSize - items[inventorySlot].amount);
                        items[inventorySlot].amount = newItem.stackSize;
                        newItem.amount = amount;
                        inventory.onItemChangedCallback?.Invoke();
                        result = Add(newItem, amount);
                    }
                }
            }
        }
        else
        {
            result = false;
            for(int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    if(newItem.stackSize >= amount)
                    {
                        items[i] = newItem;
                        items[i].amount = amount;
                        result = true;
                        break;
                    }
                    else
                    {
                        amount -= newItem.stackSize;
                        items[i] = newItem;
                        items[i].amount = newItem.stackSize;
                    }
                }
                else
                {
                    if (items[i].name.Equals(newItem.name))
                    {
                        if(amount + items[i].amount <= newItem.stackSize)
                        {
                            items[i].amount += amount;
                            result = true;
                            break;
                        }
                        else
                        {
                            amount -= (items[i].stackSize - items[i].amount);
                            items[i].amount = items[i].stackSize;
                        }
                    }
                }
            }
        }
        StorageUI.instance.UpdateStorage(items,this);
        return result;
    }

    public void Swap(Item item,int amount,int from,int to)
    {
        if (items[to] == null)
        {
                items[to] = item;
                item.amount = amount;
                items[from] = null;
        }
        else
        {
            if(items[to].name.Equals(item.name)){
                if(items[to].amount + amount <= items[to].stackSize)
                {
                    items[to].amount += amount;
                    items[from] = null;
                }
                else
                {
                    amount -= (items[to].stackSize - items[to].amount);
                    items[to].amount = item.stackSize;
                    items[from].amount = amount;
                }
            }
        }
        StorageUI.instance.UpdateStorage(items,this);
    }

    public bool hasEnoughSpaceForItem(Item item, int amount)
    {
        throw new System.NotImplementedException();
    }

    public bool IsContainsItem(Item item, int amount)
    {
        throw new System.NotImplementedException();
    }

    public bool IsFull()
    {
        throw new System.NotImplementedException();
    }

    public int ItemCount(Item item)
    {
        throw new System.NotImplementedException();
    }

    public void Remove(Item item, int amount = 1, int slotIndex = -1)
    {

        if(slotIndex != -1)
        {
            if (items[slotIndex].name.Equals(item.name))
            {
                if(items[slotIndex].amount >= amount)
                {
                    items[slotIndex].amount -= amount;
                    if (items[slotIndex].amount == 0)
                    {
                        items[slotIndex] = null;
                    }
                }
                else
                {
                    amount -= items[slotIndex].amount;
                    items[slotIndex] = null;
                    Remove(item, amount);
                }
            }
        }
        else
        {
            for(int i = 0; i < items.Length; i++)
            {
                if (items[i].name.Equals(item.name))
                {
                    if (items[i].amount >= amount)
                    {
                        items[slotIndex].amount -= amount;
                        if (items[slotIndex].amount == 0)
                        {
                            items[slotIndex] = null;
                            break;
                        }
                    }
                    else
                    {
                        amount -= items[i].amount;
                        items[i] = null;
                        Remove(item, amount);
                    }
                }
            }
        }
        
        StorageUI.instance.UpdateStorage(items,this);
    }
}

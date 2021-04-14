using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, IItemContainer
{
 
    #region Singleton

    public static Inventory instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of inventory found!");
            return;
        }
        instance = this;
        items = new Item[space];
    }

    #endregion

    public delegate void OnItemChange();
    public OnItemChange onItemChangedCallback;
    public int space = 32;
    public Item[] items;
    public int draggingItemIndex = -1;

    public InventorySlot selectedSlot;

    public bool Add(Item newItem, int amount, int inventorySlot = -1)
    {
        if (!newItem.isDefaultItem)
        {
            if (newItem.isStackable)
            {
                bool isItemFound = false;
                if (inventorySlot != -1)
                {
                    if (items[inventorySlot] != null)
                    {
                        if (items[inventorySlot].name.Equals(newItem.name))
                        {
                            if (items[inventorySlot].amount <= (items[inventorySlot].stackSize - amount))
                            {
                                isItemFound = true;
                                items[inventorySlot].amount += amount;
                            }
                            else
                            {
                                amount -= (items[inventorySlot].stackSize - items[inventorySlot].amount);
                                items[inventorySlot].amount = items[inventorySlot].stackSize;
                            }
                        }
                    }
                    else
                    {
                        items[inventorySlot] = Instantiate(newItem);
                        if(newItem.stackSize < amount)
                        {
                            amount -= newItem.stackSize;
                            items[inventorySlot].amount = newItem.stackSize;
                        }
                        else
                        {
                            isItemFound = true;
                            items[inventorySlot].amount = amount;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i] == null)
                        {
                            if (amount <= newItem.stackSize)
                            {
                                isItemFound = true;
                                items[i] = newItem;
                                items[i].amount = amount;
                                break;
                            }
                            else
                            {
                                items[i] = Instantiate(newItem);
                                amount -= items[i].stackSize;
                                items[i].amount = items[i].stackSize;
                            }
                        }
                        else 
                        {
                            if (items[i].name.Equals(newItem.name))
                            {
                                if (items[i].amount <= (items[i].stackSize - amount))
                                {
                                    isItemFound = true;
                                    items[i].amount += amount;

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

                if (!isItemFound)
                {
                    Item overStackedItem = Item.Instantiate(newItem);
                    if (hasEnoughSpaceForItem(overStackedItem, amount))
                    {
                        overStackedItem.amount = amount;
                        Add(overStackedItem, amount);
                    }
                    else
                    {
                        newItem.amount = amount;
                        onItemChangedCallback?.Invoke();
                        return false;
                    }
                }
            }
            else
            {
                PushItem(newItem, inventorySlot);
            }
            onItemChangedCallback?.Invoke();
        }
        return true;
    }

    public void Remove(Item item, int amount = 1, int slotIndex = -1)
    {
        if (slotIndex > -1)
        {
            if (items[slotIndex] != null)
            {
                if (items[slotIndex].amount > amount)
                {
                    items[slotIndex].amount -= amount;

                }
                else if (items[slotIndex].amount == amount)
                {
                    items[slotIndex] = null;
                }
                else
                {
                    Debug.LogError("Not enough item!");
                }
            }
        }
        else
        {
            RemoveFromItems(item, amount);

        }
        onItemChangedCallback?.Invoke();

    }

    public bool PushItem(Item item, int inventorySlot = -1)
    {
        if (inventorySlot != -1)
        {
            if (items[inventorySlot] == null)
            {
                items[inventorySlot] = item;
                return true;
            }
        }
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                return true;
            }
        }
        return false;
    }

    bool RemoveFromItems(Item item, int amount)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                if (items[i].name.Equals(item.name))
                {
                    if (items[i].amount <= amount)
                    {
                        amount -= items[i].amount;
                        items[i] = null;
                        if (amount <= 0) return true;
                        continue;
                    }
                    items[i].amount -= amount;
                    return true;
                }
            }
        }
        return false;
    }

    public void SwapItems(int to, int from)
    {
        if (to == from) return;
        if (items[from] != null)
        {
            if (items[to]?.name == items[from]?.name && items[to].isStackable)
            {
                if (items[to].amount + items[from].amount > items[to].stackSize)
                {
                    int overflowAmount = items[to].amount + items[from].amount - items[to].stackSize;
                    items[to].amount = items[to].stackSize;
                    items[from].amount = overflowAmount;
                }
                else
                {
                    items[to].amount += items[from].amount;
                    items[from] = null;
                }
            }
            else
            {
                Item temp;
                temp = items[to];
                items[to] = items[from];
                items[from] = temp;
            }
            onItemChangedCallback?.Invoke();
        }

    }

    public int FindEmptySlot(int begin = 0, int end = -1)
    {
        if (end == -1) end = items.Length;
        for (int i = begin; i < end; i++)
        {
            if (items[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public void DestroyItem(Item item,int amount = 0)
    {
        if(amount == 0)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == item)
                {
                    items[i] = null;
                    break;
                }

            }
        }
        
        onItemChangedCallback?.Invoke();
    }

    public bool hasEnoughSpaceForItem(Item item, int amount)
    {
        if (item != null)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    amount -= item.stackSize;
                }
                else if (items[i].name.Equals(item.name))
                {

                    amount -= items[i].stackSize - items[i].amount;

                }
                if (amount <= 0)
                {
                    return true;
                }
            }
            return false;
        }
        return true;

    }

    public bool IsFull()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsContainsItem(Item item, int amount)
    {
        int test = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                if (items[i].name.Equals(item.name))
                {
                    amount -= items[i].amount;
                    test += items[i].amount;
                    if (amount <= 0)
                    {
                        return true;
                    }
                }
            }

        }
        return false;
    }

    public int ItemCount(Item item)
    {
        int amount = 0;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                if (items[i].name.Equals(item.name))
                {
                    amount += items[i].amount;
                }
            }

        }
        return amount;
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
        for (int i = 0; i < recipe.inputs.Count; i++)
        {
            if (!IsContainsItem(recipe.inputs[i].item, recipe.inputs[i].amount))
            {
                return false;
            }
        }
        return true;
    }

    public Tool FindUsefulTool(ToolType toolType)
    {
        Tool currentTool = EquipmentManager.instance.CurrentEquipmentByEquipmentType(EquipmentTypes.Weapon) as Tool;
        if (currentTool?.toolType == toolType || currentTool?.toolType == ToolType.Multitool)
        {
            return currentTool;
        }

        foreach (Item item in items)
        {
            Tool tool = item as Tool;

            if (tool?.toolType == toolType || tool?.toolType == ToolType.Multitool)
            {
                return tool;
            }
        }

        return null;

    }

    public void UseSelectedSlot()
    {
        selectedSlot?.UseItem();
    }

    public void SplitSelectedSlot()
    {
        selectedSlot?.SplitAmount();
    }

    public void RemoveSelectedSlot()
    {
        selectedSlot?.OnRemoveButton();
    }


}

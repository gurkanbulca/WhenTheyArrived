using Michsky.UI.ModernUIPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour, IItemContainer
{
    [SerializeField]
    int inputSize, outputSize;
    [SerializeField]
    CraftingRecipe[] recipeList;
    
    [HideInInspector]
    public DateTime productionComplateDate;
    [HideInInspector]
    public int draggingInputIndex = -1, draggingOutputIndex = -1;
    [HideInInspector]
    public Item[] inputs, outputs;
    [HideInInspector]
    public bool isProducing;

    WindowManager windowManager;
    GameObject multiUseUI;
    Inventory inventory;
    CraftingRecipe producingRecipe;
    




    private void Start()
    {
        multiUseUI = MultiuseUI.instace.multiuseUI;
        windowManager = MultiuseUI.instace.windowManager;

        inputs = new Item[inputSize];
        outputs = new Item[outputSize];
        inventory = Inventory.instance;
        productionComplateDate = GetTime();

        //test case
        //productionComplateDate = GetTime(272);


    }

    public void Interact()
    {
        if (!multiUseUI.activeSelf)
        {
            multiUseUI.SetActive(true);
            windowManager.OpenPanel("Production");
            ProductionUI.instance.UpdateRecipeList(recipeList, this);
            ProductionUI.instance.UpdateInputsAndOutputs(inputs, outputs, this);
        }

    }

    public bool hasEnoughSpaceForItem(Item item, int amount)
    {
        foreach (var output in outputs)
        {
            if (output == null)
            {
                amount -= item.stackSize;
            }
            else if (output.name.Equals(item.name))
            {

                amount -= output.stackSize - output.amount;

            }
            if (amount <= 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsContainsItem(Item item, int amount)
    {
        foreach (var input in inputs)
        {
            if (input != null)
            {
                if (input.name.Equals(item.name))
                {
                    amount -= input.amount;
                    if (amount <= 0)
                    {
                        return true;
                    }
                }

            }

        }
        return false;
    }

    public bool Add(Item newItem, int amount, int inventorySlot = -1)
    {
        bool result = true;
        if (inventorySlot != -1)
        {
            if (inputs[inventorySlot] == null)
            {
                if (amount <= newItem.stackSize)
                {
                    newItem.amount = amount;
                    inputs[inventorySlot] = newItem;
                    inventory.DestroyItem(newItem);
                }
                else
                {
                    inputs[inventorySlot] = Instantiate(newItem);
                    inputs[inventorySlot].amount = newItem.stackSize;
                    newItem.amount = amount - newItem.stackSize;
                    inventory.onItemChangedCallback?.Invoke();
                }
            }
            else
            {
                if (inputs[inventorySlot].name.Equals(newItem.name))
                {
                    if (amount + inputs[inventorySlot].amount <= newItem.stackSize)
                    {
                        inputs[inventorySlot].amount += amount;
                        inventory.DestroyItem(newItem);
                    }
                    else
                    {
                        newItem.amount -= inputs[inventorySlot].stackSize - inputs[inventorySlot].amount;
                        inputs[inventorySlot].amount = inputs[inventorySlot].stackSize;
                        inventory.onItemChangedCallback?.Invoke();
                    }
                }
                else
                {
                    Item oldItem = inputs[inventorySlot];
                    inputs[inventorySlot] = newItem;
                    newItem.amount = amount;
                    inventory.DestroyItem(newItem);
                    inventory.Add(oldItem, oldItem.amount);
                }
            }
            result = false;
        }
        else
        {
            result = false;
            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] == null)
                {
                    outputs[i] = newItem;
                    if (outputs[i].stackSize < amount)
                    {
                        outputs[i].amount = outputs[i].stackSize;
                        amount -= outputs[i].stackSize;
                    }
                    else
                    {
                        outputs[i].amount = amount;
                        result = true;
                        break;
                    }
                }
                else
                {
                    if (outputs[i].name.Equals(newItem.name))
                    {
                        if (outputs[i].amount + amount > outputs[i].stackSize)
                        {
                            amount -= outputs[i].stackSize - outputs[i].amount;
                            outputs[i].amount = outputs[i].stackSize;
                        }
                        else
                        {
                            outputs[i].amount += amount;
                            result = true;
                            break;
                        }
                    }
                }
            }
        }

        ProductionUI.instance.UpdateInputsAndOutputs(inputs, outputs, this);
        SetCraftingStatus();
        return result;
    }

    public void Swap(Item newItem, int amount, ProductionSlotType toType, int toIndex)
    {
        if (newItem != null)
        {
            int fromIndex;
            ProductionSlotType fromType = FindSlotTypeByItem(newItem, out fromIndex);
            Item[] stackTo = GetStackBySlotType(toType);
            Item[] stackFrom = GetStackBySlotType(fromType);

            if (stackTo[toIndex] == null)
            {
                stackTo[toIndex] = newItem;
                stackFrom[fromIndex] = null;
            }
            else
            {
                if (stackTo[toIndex].name.Equals(newItem.name))
                {
                    if (amount <= newItem.stackSize - stackTo[toIndex].amount)
                    {
                        stackTo[toIndex].amount += amount;
                        stackFrom[fromIndex] = null;
                    }
                    else
                    {
                        amount -= (stackTo[toIndex].stackSize - stackTo[toIndex].amount);
                        stackFrom[fromIndex].amount = amount;
                        stackTo[toIndex].amount = newItem.stackSize;
                    }
                }
                else
                {
                    if (CanContain(stackTo[toIndex], fromType))
                    {
                        Item temp = stackTo[toIndex];
                        stackTo[toIndex] = stackFrom[fromIndex];
                        stackFrom[fromIndex] = temp;
                    }
                }
            }
            ProductionUI.instance.UpdateInputsAndOutputs(inputs, outputs, this);
            SetCraftingStatus();
        }
    }

    public void Remove(Item item, int amount = 1, int slotIndex = -1)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] != null)
            {
                if (inputs[i].name.Equals(item.name))
                {
                    int itemAmount = inputs[i].amount;
                    inputs[i].amount -= amount;
                    if (inputs[i].amount <= 0)
                    {
                        inputs[i] = null;
                    }
                    amount -= itemAmount;
                    if (amount <= 0)
                    {
                        break;
                    }
                }
            }
        }
        ProductionUI.instance.UpdateInputsAndOutputs(inputs, outputs, this);
        SetCraftingStatus();
    }

    public bool IsFull()
    {
        foreach (var output in outputs)
        {
            if (output == null) return false;
        }
        return true;
    }

    public int ItemCount(Item item)
    {
        int result = 0;
        foreach (var input in inputs)
        {
            if (input != null)
            {
                if (input.name.Equals(item.name))
                {
                    result += input.amount;
                }
            }
        }
        return result;
    }


    public DateTime GetTime(int seconds = 0)
    {
        return DateTime.UtcNow.AddSeconds(seconds);
    }

    public void DestroyItem(Item item)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] == item)
            {
                inputs[i] = null;
                break;
            }
        }
        for (int i = 0; i < outputs.Length; i++)
        {
            if (outputs[i] == item)
            {
                outputs[i] = null;
                break;
            }
        }
        ProductionUI.instance.UpdateInputsAndOutputs(inputs, outputs, this);
        SetCraftingStatus();
    }

    public string secondsToString(int time)
    {
        string minutes = (time / 60).ToString("00");
        string seconds = (time % 60).ToString("00");
        return minutes + ":" + seconds;
    }

    public bool CanContain(Item item,ProductionSlotType slotType)
    {
        if(slotType == ProductionSlotType.Input)
        {
            foreach (CraftingRecipe recipe in recipeList)
            {
                foreach (var input in recipe.inputs)
                {
                    if (input.item.name.Equals(item.name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        else
        {
            return false;
        }
        
    }

    CraftingRecipe FindCraftableRecipe()
    {
        foreach (var recipe in recipeList)
        {
            if (CanCraft(recipe))
            {
                return recipe;
            }
        }
        return null;

    }

    bool CanCraft(CraftingRecipe recipe)
    {
        foreach (var requirement in recipe.inputs)
        {
            if (!IsContainsItem(requirement.item, requirement.amount))
            {
                return false;
            }
        }
        foreach(var output in recipe.outputs)
        {
            if (!hasEnoughSpaceForItem(output.item, output.amount))
            {
                return false;
            }
        }
        foreach (var input in inputs)
        {
            if (input != null)
            {
                bool containsWrongItem = true;
                foreach (var requirement in recipe.inputs)
                {
                    if (input.name.Equals(requirement.item.name)) containsWrongItem = false;
                }
                if (containsWrongItem) return false;
            }
        }
        return true;
    }

    void SetCraftingStatus()
    {
        CraftingRecipe recipe = FindCraftableRecipe();
        if (recipe != null)
        {
            if (isProducing)
            {
                if (CanCraft(recipe))
                {
                    if (recipe != producingRecipe)
                    {
                        isProducing = true;
                        producingRecipe = recipe;
                        productionComplateDate = GetTime(recipe.craftTime);
                    }
                }
                else
                {
                    isProducing = false;
                    producingRecipe = null;
                    productionComplateDate = GetTime();
                }
            }
            else
            {
                isProducing = true;
                producingRecipe = recipe;
                productionComplateDate = GetTime(recipe.craftTime);
            }
        }
        else
        {
            isProducing = false;
            producingRecipe = null;
            productionComplateDate = GetTime();
        }
    }

    public int FindRemainingTime()
    {
        return (int)(productionComplateDate - GetTime()).TotalSeconds;
    }

    public void Craft()
    {
        if (isProducing)
        {
            Destroy(ProductionUI.instance.copyIcon);
            ProductionUI.instance.copyIcon = null;
            draggingInputIndex = -1;
            draggingOutputIndex = -1;
            int remainingTime = FindRemainingTime();
            
            if (remainingTime <= 0)
            {
                bool hasEnoughSpace = true;
                foreach (var output in producingRecipe.outputs)
                {
                    hasEnoughSpace &= hasEnoughSpaceForItem(output.item, output.amount);
                }
                if (hasEnoughSpace)
                {
                    producingRecipe.Craft(this);
                    if (producingRecipe)
                    {
                        productionComplateDate = productionComplateDate.AddSeconds(producingRecipe.craftTime);
                        if (producingRecipe.CanCraft(this))
                        {
                            Craft();
                        }
                        else
                        {
                            ProductionUI.instance.UpdateTimerText();
                        }
                    }

                }
                else
                {
                    SetCraftingStatus();
                }
                
            }
        }
        
    }

    public Item[] GetStackBySlotType(ProductionSlotType type)
    {
        Item[] result = null;
        switch (type)
        {
            case ProductionSlotType.Input:
                result = inputs;
                break;
            case ProductionSlotType.Output:
                result = outputs;
                break;
            default:
                break;
        }
        return result;
    }

    ProductionSlotType FindSlotTypeByItem(Item item,out int index)
    {
        index = -1;
        for(int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] == item)
            {
                index = i;
                return ProductionSlotType.Input;
            }
        }
        for(int i = 0; i < outputs.Length; i++)
        {
            if (outputs[i] == item)
            {
                index = i;
                return ProductionSlotType.Output;
            }
        }

        return ProductionSlotType.Input;
    }


}

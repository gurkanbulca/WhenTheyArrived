using Michsky.UI.ModernUIPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Burnable
{
    public Item item;
    public int burnAmount;
}

public class SmelterController : MonoBehaviour,IItemContainer
{
    

    [SerializeField]
    int inputSize, outputSize,fuelSize;
    [SerializeField]
    CraftingRecipe[] recipeList;
    [SerializeField]
    Burnable[] burnables;
    [HideInInspector]
    public DateTime smelthingComplateDate;
    [HideInInspector]
    public int draggingInputIndex = -1, draggingOutputIndex = -1,draggingFuelIndex=-1;
    [HideInInspector]
    public Item[] inputs, outputs;
    [HideInInspector]
    public Item[] fuels;
    [HideInInspector]
    public bool isSmelthing;

    WindowManager windowManager;
    GameObject multiUseUI;
    Inventory inventory;
    CraftingRecipe smelthingRecipe;





    private void Start()
    {
        multiUseUI = MultiuseUI.instace.multiuseUI;
        windowManager = MultiuseUI.instace.windowManager;

        inputs = new Item[inputSize];
        outputs = new Item[outputSize];
        fuels = new Item[fuelSize];
        inventory = Inventory.instance;
        smelthingComplateDate = GetTime();

        // TEST CASE
        //smelthingComplateDate = GetTime(272);

    }


    public void Interact()
    {
        if (!multiUseUI.activeSelf)
        {
            multiUseUI.SetActive(true);
            windowManager.OpenPanel("Smelthing");
            SmelterUI.instance.UpdateRecipeList(recipeList,burnables, this);
            SmelterUI.instance.UpdateInputsOutputsAndFuel(inputs, outputs, fuels, this);
        }

    }

    public void Craft()
    {
        if (isSmelthing)
        {
            Destroy(SmelterUI.instance.copyIcon);
            SmelterUI.instance.copyIcon = null;
            draggingInputIndex = -1;
            draggingOutputIndex = -1;
            int remainingTime = FindRemainingTime();

            if (remainingTime <= 0)
            {
                bool hasEnoughSpace = true;
                foreach (var output in smelthingRecipe.outputs)
                {
                    hasEnoughSpace &= hasEnoughSpaceForItem(output.item, output.amount);
                }
                if (hasEnoughSpace)
                {
                    BurnFuel();
                    smelthingRecipe.Craft(this);
                    if (smelthingRecipe)
                    {
                        smelthingComplateDate = smelthingComplateDate.AddSeconds(smelthingRecipe.craftTime);
                        if (smelthingRecipe.CanCraft(this))
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



    // Outputs Only
    public bool hasEnoughSpaceForItem(Item item, int amount)
    {
        foreach (var output in outputs)
        {
            if (output != null)
            {
                if (output.name.Equals(item.name))
                {
                    amount -= (output.stackSize - output.amount);
                }
            }
            else
            {
                amount -= item.stackSize;
            }
            if (amount <= 0) return true;

        }
        return false;
    }

    //Inputs Only
    public bool IsContainsItem(Item item, int amount)
    {
        foreach(var input in inputs)
        {
            if (input != null)
            {
                if (input.name.Equals(item.name))
                {
                    amount -= input.amount;
                    if (amount <= 0) return true;
                }
            }
        }
        return false;
    }

    public bool Add(Item newItem, int amount, int inventorySlot = -1)
    {
        if (inventorySlot == -1)
        {
            bool result = true;
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
            SmelterUI.instance.UpdateInputsOutputsAndFuel(inputs, outputs,fuels, this);
            SetCraftingStatus();
            return result;
        }
        else
        {
            throw new NotImplementedException();
        }

    }

    public void Swap(Item newItem,int amount,SmelterSlotType toType,int toIndex)
    {
        if (newItem != null)
        {
            int fromIndex;
            SmelterSlotType fromType = FindSlotTypeByItem(newItem, out fromIndex);
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
                    if(amount <= newItem.stackSize - stackTo[toIndex].amount)
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
                    if(CanContain(stackTo[toIndex], fromType))
                    {
                        Item temp = stackTo[toIndex];
                        stackTo[toIndex] = stackFrom[fromIndex];
                        stackFrom[fromIndex] = temp;
                    }
                }
            }
            SmelterUI.instance.UpdateInputsOutputsAndFuel(inputs, outputs, fuels, this);
            SetCraftingStatus();
        }
    }

   

    public void Add(Item newItem,int amount,SmelterSlotType slotType, int inventorySlot = -1)
    {
        if(inventorySlot > -1)
        {
            Item[] slots;
            switch (slotType)
            {
                case SmelterSlotType.Input:
                    slots = inputs;
                    break;
                case SmelterSlotType.Output:
                    slots = outputs;
                    break;
                case SmelterSlotType.Fuel:
                    slots = fuels;
                    break;
                default:
                    slots = null;
                    break;
            }
            if (slots[inventorySlot] == null)
            {
                slots[inventorySlot] = newItem;
                newItem.amount = amount;
                inventory.DestroyItem(newItem);
            }
            else
            {
                if (slots[inventorySlot].name.Equals(newItem.name))
                {
                    if(amount + slots[inventorySlot].amount <= newItem.stackSize)
                    {
                        slots[inventorySlot].amount += amount;
                        inventory.DestroyItem(newItem);
                    }
                    else
                    {
                        amount -= slots[inventorySlot].stackSize - slots[inventorySlot].amount;
                        slots[inventorySlot].amount = newItem.stackSize;
                        newItem.amount = amount;
                        inventory.onItemChangedCallback?.Invoke();
                    }
                }
                else
                {
                    Item oldItem = slots[inventorySlot];
                    slots[inventorySlot] = newItem;
                    newItem.amount = amount;
                    inventory.DestroyItem(newItem);
                    inventory.Add(oldItem, oldItem.amount);
                }
            }
        }

        SmelterUI.instance.UpdateInputsOutputsAndFuel(inputs, outputs, fuels, this);
        SetCraftingStatus();
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
        SmelterUI.instance.UpdateInputsOutputsAndFuel(inputs, outputs,fuels, this);
        SetCraftingStatus();
    }

    public bool DestroyItem(Item item,int amount = 0)
    {
        for(int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] == item)
            {
                DestroyProcess(inputs[i],amount);
                SmelterUI.instance.UpdateInputsOutputsAndFuel(inputs, outputs, fuels, this);
                SetCraftingStatus();
                return true;
            }
        }
        for (int i = 0; i < outputs.Length; i++){
            if (outputs[i] == item)
            {
                DestroyProcess(outputs[i], amount);
                SmelterUI.instance.UpdateInputsOutputsAndFuel(inputs, outputs, fuels, this);
                SetCraftingStatus();
                return true;
            }
        }
        for (int i = 0; i < fuels.Length; i++)
        {
            if (fuels[i] == item)
            {
                DestroyProcess(fuels[i], amount);
                SmelterUI.instance.UpdateInputsOutputsAndFuel(inputs, outputs, fuels, this);
                SetCraftingStatus();
                return true;
            }
        }
        return false;
        
    }

    void DestroyProcess(Item item,int amount)
    {
        if (amount == 0)
        {
            item = null;
        }
        else
        {
            item.amount -= amount;
            if (item.amount <= 0)
            {
                item = null;
            }
        }
    }

    public bool IsFull()
    {
        throw new NotImplementedException();
    }

    public int ItemCount(Item item)
    {
        throw new NotImplementedException();
    }

    public bool CanContain(Item item,SmelterSlotType slotType)
    {
        switch (slotType)
        {
            case SmelterSlotType.Input:
                foreach (var recipe in recipeList)
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
            case SmelterSlotType.Fuel:
                foreach (var burnable in burnables)
                {
                    if (burnable.item.name.Equals(item.name)){
                        return true;
                    }
                }
                return false;
            default:
                return false;
        }
    }

    public void SetCraftingStatus()
    {
        CraftingRecipe recipe = FindCraftableRecipe();
        if(recipe != null)
        {
            if (isSmelthing)
            {
                if (CanCraft(recipe))
                {
                    if(recipe!= smelthingRecipe)
                    {
                        isSmelthing = true;
                        smelthingRecipe = recipe;
                        smelthingComplateDate = GetTime(recipe.craftTime);
                    }
                }
                else
                {
                    isSmelthing = false;
                    smelthingRecipe = null;
                    smelthingComplateDate = GetTime();
                }
            }
            else
            {
                isSmelthing = true;
                smelthingRecipe = recipe;
                smelthingComplateDate = GetTime(recipe.craftTime);
            }
        }
        else
        {
            isSmelthing = false;
            smelthingRecipe = null;
            smelthingComplateDate = GetTime();
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
        foreach (var output in recipe.outputs)
        {
            if (!hasEnoughSpaceForItem(output.item, output.amount))
            {
                return false;
            }
        }
        foreach(var input in inputs)
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
        return HasEnoughFuel();
    }

    bool HasEnoughFuel()
    {
        foreach (var fuel in fuels)
        {
            foreach (var burnable in burnables)
            {
                if (fuel != null)
                {
                    if (burnable.item.name.Equals(fuel.name) && fuel.amount >= burnable.burnAmount)
                    {
                        return true;
                    }
                }
               
            }
        }
        return false;
    }

    public DateTime GetTime(int seconds = 0)
    {
        return DateTime.UtcNow.AddSeconds(seconds);
    }

    public string secondsToString(int time)
    {
        string minutes = (time / 60).ToString("00");
        string seconds = (time % 60).ToString("00");
        return minutes + ":" + seconds;
    }

    public int FindRemainingTime()
    {
        return (int)(smelthingComplateDate - GetTime()).TotalSeconds;
    }

    public void BurnFuel()
    {
        for(int i = 0; i < fuels.Length; i++)
        {
            foreach(var burnable in burnables)
            {
                if (fuels[i] != null)
                {
                    if (burnable.item.name.Equals(fuels[i].name))
                    {
                        if (fuels[i].amount >= burnable.burnAmount)
                        {
                            fuels[i].amount -= burnable.burnAmount;
                            if (fuels[i].amount == 0)
                            {
                                fuels[i] = null;
                            }
                            SmelterUI.instance.UpdateInputsOutputsAndFuel(inputs, outputs, fuels, this);
                        }
                    }
                }
                
            }
        }
    }

    SmelterSlotType FindSlotTypeByItem(Item item,out int index)
    {
        index = -1;
        for(int i=0;i< inputs.Length;i++)
        {
            if (inputs[i] == item)
            {
                index = i;
                return SmelterSlotType.Input;
            }
        }
        for (int i = 0; i < outputs.Length; i++)
        {
            if (outputs[i] == item)
            {
                index = i;
                return SmelterSlotType.Output;
            }
        }
        for (int i = 0; i < fuels.Length; i++)
        {
            if (fuels[i] == item)
            {
                index = i;
                return SmelterSlotType.Fuel;
            }
        }
        return SmelterSlotType.Input;
    }

    public Item[] GetStackBySlotType(SmelterSlotType type)
    {
        Item[] result = null;
        switch (type)
        {
            case SmelterSlotType.Input:
                result = inputs;
                break;
            case SmelterSlotType.Output:
                result = outputs;
                break;
            case SmelterSlotType.Fuel:
                result = fuels;
                break;
            default:
                break;
        }
        return result;
    }

}

public enum SmelterSlotType
{
    Input,Output,Fuel
}



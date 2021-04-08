using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemAmount
{
    public Item item;
    [Range(1,999)]
    public int amount;
}


[CreateAssetMenu(fileName ="New Recipe",menuName ="Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{

    public List<ItemAmount> inputs;
    public List<ItemAmount> outputs;
    public int craftTime;

    public bool CanCraft(IItemContainer itemContainer)
    {
        foreach (ItemAmount itemAmount in inputs)
        {
            if (!itemContainer.IsContainsItem(itemAmount.item, itemAmount.amount))
            {
                return false;
            }
        }

        return true;
    }

    public void Craft(IItemContainer itemContainer)
    {
        if (CanCraft(itemContainer))
        {
            for(int i = 0; i < inputs.Count; i++)
            {
                itemContainer.Remove(inputs[i].item, inputs[i].amount);
            }
            for(int i = 0; i < outputs.Count; i++)
            {
                if (!itemContainer.hasEnoughSpaceForItem(outputs[i].item, outputs[i].amount))
                {
                    for(int j = 0;j < i; j++)
                    {
                        itemContainer.Remove(outputs[j].item, outputs[j].amount);
                    }
                    for(int j = 0; j < inputs.Count; j++)
                    {
                        itemContainer.Add(inputs[j].item, inputs[j].amount);
                    }
                    Debug.LogError("Not enough inventory space!");
                    return;
                }
                Item newItem = Instantiate(outputs[i].item);
                itemContainer.Add(newItem, outputs[i].amount);
            }
        }

    }

    public void CraftWithoutOutputs(IItemContainer itemContainer)
    {
        if (CanCraft(itemContainer))
        {
            for(int i = 0; i < inputs.Count; i++)
            {
                itemContainer.Remove(inputs[i].item, inputs[i].amount);
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour
{
    Sprite sprite;
    CraftingRecipe recipe;
    CraftingSlotType slotType;
    CraftingUI craftingUI;

    public Image image;
    public Text text;
    public Button button;
    public Material grayscaleMaterial;

    private void Start()
    {
        this.craftingUI = CraftingUI.instance;
    }

    public void SetSlotValues(CraftingRecipe recipe, CraftingSlotType slotType, int inputIndex = -1, int outputIndex = -1)
    {
        this.slotType = slotType;
        this.recipe = recipe;
        switch (slotType)
        {
            case CraftingSlotType.Recipe:
                sprite = recipe.outputs[0].item.icon;
                break;
            case CraftingSlotType.Input:
                sprite = recipe.inputs[inputIndex].item.icon;
                Inventory inventory = Inventory.instance;
                int itemCount = inventory.ItemCount(recipe.inputs[inputIndex].item);
                text.enabled = true;
                text.text = itemCount + "/" + recipe.inputs[inputIndex].amount;
                image.material = Instantiate(grayscaleMaterial);
                if(itemCount < recipe.inputs[inputIndex].amount)
                {
                    image.material.SetFloat("_EffectAmount", 1);
                }
                button.enabled = false;
                break;
            case CraftingSlotType.Output:
                sprite = recipe.outputs[outputIndex].item.icon;
                int amount = recipe.outputs[outputIndex].amount;
                if (amount > 1)
                {
                    text.enabled = true;
                    text.text = amount.ToString();
                }
                button.enabled = false;
                break;
            default:
                break;
        }
        image.enabled = true;
        image.sprite = sprite;
    }

    public void OnRecipeClicked()
    {
        if (slotType == CraftingSlotType.Recipe)
        {
            craftingUI.SetInputOutput(recipe);
        }
    }

}

public enum CraftingSlotType
{
    Recipe, Input, Output
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    #region Singleton

    public static CraftingUI instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Instance already casted!");
            return;
        }
        instance = this;
    }

    #endregion

    public GameObject craftingUI;
    public GameObject craftingSlotSample;
    public Transform recipeParent,inputParent,outputParent;
    public Button craftButton;
    public Text title, description;
    
    CraftingRecipe selectedRecipe;
    Inventory inventory;

    private void Start()
    {
        inventory = Inventory.instance;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Crafting"))
        {
            craftingUI.SetActive(!craftingUI.activeSelf);
            if(selectedRecipe != null)
            {
                SetInputOutput(selectedRecipe);
            }

        }
    }

    public void GenerateRecipeList(List<CraftingRecipe> recipes)
    {
        ClearParent(recipeParent);
        foreach (CraftingRecipe recipe in recipes)
        {
            GameObject slot = Instantiate(craftingSlotSample, recipeParent.transform);
            CraftingSlot script = slot.GetComponent<CraftingSlot>();
            script.SetSlotValues(recipe, CraftingSlotType.Recipe);
        }
        //recipeParent.GetComponent<DynamicRectSize>().SetHeight();
    }

    void ClearParent(Transform parent)
    {
        for(int i=0;i< parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    public void SetInputOutput(CraftingRecipe recipe)
    {
        selectedRecipe = recipe;
        ClearParent(inputParent);
        ClearParent(outputParent);
        for (int i=0;i< recipe.inputs.Count;i++)
        {
            GameObject slot = Instantiate(craftingSlotSample, inputParent);
            CraftingSlot script = slot.GetComponent<CraftingSlot>();
            script.SetSlotValues(recipe, CraftingSlotType.Input, i);
        }
        for (int i = 0; i < recipe.outputs.Count; i++)
        {
            GameObject slot = Instantiate(craftingSlotSample, outputParent);
            CraftingSlot script = slot.GetComponent<CraftingSlot>();
            script.SetSlotValues(recipe, CraftingSlotType.Output, -1,i);
        }
        SetDescriptions(recipe);
        SetCraftButton();
    }

    void SetCraftButton()
    {
        if (selectedRecipe.CanCraft(inventory))
        {
            craftButton.interactable = true;
        }
        else
        {
            craftButton.interactable = false;
        }
    }

    void SetDescriptions(CraftingRecipe recipe)
    {
        title.text = recipe.outputs[0].item.name;
        description.text = recipe.outputs[0].item.description;
    }

    public void OnCraftButtonClicked()
    {
        if (selectedRecipe.CanCraft(inventory))
        {
            selectedRecipe.Craft(inventory);
            SetCraftButton();
            SetInputOutput(selectedRecipe);
        }
    }

}

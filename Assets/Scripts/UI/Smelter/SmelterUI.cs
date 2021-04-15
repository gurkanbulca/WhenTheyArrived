using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SmelterUI : MonoBehaviour,ISidePanelUI
{

    #region Singleton

    public static SmelterUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion


    [SerializeField]
    Transform inputParent, outputParent,fuelParent, recipeList,burnableParent ,craftTime;
    [SerializeField]
    GameObject SmelterLabel, inputSlot, outputSlot,fuelSlot;
    [HideInInspector]
    public SmelterController smelter;
    [HideInInspector]
    public GameObject copyIcon;

    private void OnEnable()
    {
        craftTime.GetComponent<TMP_Text>().text = "";
        InvokeRepeating("UpdateTimerText", 0, 0.5f);

    }

    private void OnDisable()
    {
        CancelInvoke("UpdateTimerText");
        smelter = null;
    }

    public void UpdateTimerText()
    {
        if (smelter != null)
        {

            int remainingTime = smelter.FindRemainingTime();
            if (remainingTime <= 0)
            {
                if (smelter.isSmelthing)
                {
                    smelter.Craft();
                }
                else
                {
                    craftTime.GetComponent<TMP_Text>().text = "";
                }
            }
            else
            {
                craftTime.GetComponent<TMP_Text>().text = smelter.secondsToString(remainingTime);
            }
        }
    }


    public void UpdateRecipeList(CraftingRecipe[] recipes,Burnable[] burnables, SmelterController smelter)
    {
        this.smelter = smelter;
        ClearParent(recipeList);
        ClearParent(burnableParent);
        foreach (var recipe in recipes)
        {
            GameObject newSmelterLabel = Instantiate(SmelterLabel, recipeList);
            newSmelterLabel.GetComponent<SmelterLabelUI>().SetSmelterLabel(recipe, smelter);
        }

        foreach (var burnable in burnables)
        {
            GameObject newBurnableSlot = Instantiate(fuelSlot, burnableParent);
            newBurnableSlot.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            newBurnableSlot.GetComponent<SmelterSlot>().SetSlot(burnable.item, burnable.burnAmount, smelter);
            Destroy(newBurnableSlot.GetComponent<SmelterDragHandler>());
            Destroy(newBurnableSlot.GetComponent<SmelterDropHandler>());
        }
    }

    public void UpdateInputsOutputsAndFuel(Item[] inputs, Item[] outputs,Item[] fuels, SmelterController smelter)
    {
        ClearParent(inputParent);
        ClearParent(outputParent);
        ClearParent(fuelParent);
        int amount;
        foreach (var item in inputs)
        {
            amount = item != null ? item.amount : 0;
            GameObject newInput = Instantiate(inputSlot, inputParent);
            newInput.GetComponent<SmelterSlot>().SetSlot(item, amount, smelter);
        }

        foreach (var item in outputs)
        {
            amount = item != null ? item.amount : 0;
            GameObject newOutput = Instantiate(outputSlot, outputParent);
            newOutput.GetComponent<SmelterSlot>().SetSlot(item, amount, smelter);
        }
        foreach (var fuel in fuels)
        {
            amount = fuel != null ? fuel.amount : 0;
            GameObject newFuel = Instantiate(fuelSlot, fuelParent);
            newFuel.GetComponent<SmelterSlot>().SetSlot(fuel, amount, smelter);
        }
        
    }

    void ClearParent(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    public IItemContainer GetStation()
    {
        return this.smelter;
    }
}

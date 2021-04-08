using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProductionUI : MonoBehaviour
{
    #region Singleton

    public static ProductionUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    [SerializeField]
    Transform inputParent, outputParent,recipeList,craftTime;
    [SerializeField]
    GameObject ProductionLabel;
    [SerializeField]
    GameObject inputSlot,outputSlot;
    [HideInInspector]
    public StationController station;
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
        station = null;
    }

    public void UpdateTimerText()
    {
        if (station != null)
        {

            int remainingTime = station.FindRemainingTime();
            if (remainingTime <= 0)
            {
                if (station.isProducing)
                {
                    station.Craft();
                }
                else
                {
                    craftTime.GetComponent<TMP_Text>().text = "";
                }
            }
            else
            {
                craftTime.GetComponent<TMP_Text>().text = station.secondsToString(remainingTime);
            }
        }
    }




    public void UpdateRecipeList(CraftingRecipe[] recipes,StationController station)
    {
        this.station = station;
        for(int i = 0; i < recipeList.childCount; i++)
        {
            Destroy(recipeList.GetChild(i).gameObject);
        }
        foreach (var recipe in recipes)
        {
            GameObject newProductionLabel = Instantiate(ProductionLabel, recipeList);
            newProductionLabel.GetComponent<ProductionLabelUI>().SetProductionLabel(recipe, station);
        }
    }

    public void UpdateInputsAndOutputs(Item[] inputs,Item[] outputs,StationController station)
    {
        ClearParent(inputParent);
        ClearParent(outputParent);
        foreach (var item in inputs)
        {
            int amount = item != null ? item.amount : 0;
            GameObject newInput = Instantiate(inputSlot, inputParent);
            newInput.GetComponent<ProductionSlot>().SetSlot(item, amount,station);
        }

        foreach (var item in outputs)
        {
            int amount = item != null ? item.amount : 0;
            GameObject newOutput = Instantiate(outputSlot, outputParent);
            newOutput.GetComponent<ProductionSlot>().SetSlot(item, amount,station);
        }
    }

    void ClearParent(Transform parent)
    {
        for(int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

}

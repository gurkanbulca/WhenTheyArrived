using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProductionLabelUI : MonoBehaviour
{
    [SerializeField]
    GameObject inputParent, outputParent;
    [SerializeField]
    GameObject inputSlot, outputSlot;
    [SerializeField]
    TMP_Text timeText;

    public void SetProductionLabel(CraftingRecipe recipe,StationController station)
    {
        foreach(ItemAmount input in recipe.inputs)
        {
            GameObject newInput = Instantiate(inputSlot, inputParent.transform);
            newInput.GetComponent<ProductionSlot>().SetSlot(input.item,input.amount, station);
            Destroy(newInput.GetComponent<ProductionDragHandler>());
            Destroy(newInput.GetComponent<ProductionDropHandler>());
        }
        foreach(ItemAmount output in recipe.outputs)
        {
            GameObject newOutput = Instantiate(outputSlot, outputParent.transform);
            newOutput.GetComponent<ProductionSlot>().SetSlot(output.item,output.amount, station);
            Destroy(newOutput.GetComponent<ProductionDragHandler>());

        }

        timeText.text = station.secondsToString(recipe.craftTime);
    }
}

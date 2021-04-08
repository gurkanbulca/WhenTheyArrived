using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SmelterLabelUI : MonoBehaviour
{
    [SerializeField]
    GameObject inputParent, outputParent;
    [SerializeField]
    GameObject inputSlot, outputSlot;
    [SerializeField]
    TMP_Text timeText;

    public void SetSmelterLabel(CraftingRecipe recipe, SmelterController smelter)
    {
        foreach (ItemAmount input in recipe.inputs)
        {
            GameObject newInput = Instantiate(inputSlot, inputParent.transform);
            newInput.GetComponent<SmelterSlot>().SetSlot(input.item, input.amount, smelter);
            Destroy(newInput.GetComponent<SmelterDragHandler>());
            Destroy(newInput.GetComponent<SmelterDropHandler>());
        }
        foreach (ItemAmount output in recipe.outputs)
        {
            GameObject newOutput = Instantiate(outputSlot, outputParent.transform);
            newOutput.GetComponent<SmelterSlot>().SetSlot(output.item, output.amount, smelter);
            Destroy(newOutput.GetComponent<SmelterDragHandler>());

        }

        timeText.text = smelter.secondsToString(recipe.craftTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropFieldManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] carrotPrefabs;
    [SerializeField]
    Transform placementParent;

    private void Start()
    {
        StationController stationController = GetComponent<StationController>();
        int duration=int.MaxValue;
        foreach(var recipe in stationController.recipeList)
        {
            duration = Mathf.Min(duration, recipe.craftTime);
        }
        InvokeRepeating("InitCraft", 0, duration);
    }


    void InitCraft()
    {
        GetComponent<StationController>().Craft();
    }

    public void UpdateCropField()
    {
        Item[] inputs = GetComponent<StationController>().inputs;
        Item[] outputs = GetComponent<StationController>().outputs;
        int i,j=0;
        for (i = 0; i < placementParent.childCount; i++)
        {
            if (placementParent.GetChild(i).childCount > 0)
            {
                for(int k=0;k< placementParent.GetChild(i).childCount; k++)
                {
                    Destroy(placementParent.GetChild(i).GetChild(k).gameObject);

                }
            }
        }

        foreach(var output in outputs)
        {
            if (output != null)
            {
                i = 0;
                while (i < output.amount && j < placementParent.childCount)
                {
                    if (outputs[0] == null) break;
                    GameObject newObj = Instantiate(carrotPrefabs[1], placementParent.GetChild(j));
                    newObj.transform.localPosition = Vector3.zero;
                    i++; j++;
                }
            }
        }

        foreach (var input in inputs)
        {
            if (input != null)
            {
                i = 0;
                while (i < inputs[0].amount && j < placementParent.childCount)
                {
                    if (inputs[0] == null) break;
                    GameObject newObj = Instantiate(carrotPrefabs[0], placementParent.GetChild(j));
                    newObj.transform.localPosition = Vector3.zero;
                    i++; j++;
                }
            }
        }
        
    }

}

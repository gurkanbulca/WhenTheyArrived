using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUI : MonoBehaviour
{

    public GameObject buildingUI;
    BuildingManager buildingManager;
    public Transform buildingParent;
    public GameObject buildingSlot;

    private void Start()
    {
        buildingManager = BuildingManager.instance;
    }
    void Update()
    {
        if (Input.GetButtonDown("Building"))
        {

            buildingUI.SetActive(!buildingUI.activeSelf);
            if (!buildingUI.activeSelf)
            {
                buildingManager.destroyHighlight();
                buildingManager.placementUI.SetActive(false);
                buildingManager.ReturnReplacement();
                buildingManager.CancelPicking();
            }
        }
    }

    public void CreateBuildingSlotsByRecipeList(List<CraftingRecipe> recipes)
    {
        foreach (CraftingRecipe recipe in recipes)
        {
            GameObject slot = Instantiate(buildingSlot, buildingParent);
            slot.GetComponent<BuildingSlot>().SetSlotValues(recipe);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSlot : MonoBehaviour
{
    
    public Building building;
    public CraftingRecipe recipe;
    public Text text;

    public void ButtonOnClick()
    {
        if (BuildingManager.instance.recipe != null)
        {
            BuildingManager.instance.PickBuilding(building.gameObject, building.buildingType);
        }
        else
        {
            if (BuildingManager.instance.isBuildingAlreadyPicked())
            {
                BuildingManager.instance.PickBuilding(building.gameObject, building.buildingType);
            }

            else if (recipe.CanCraft(Inventory.instance))
            {
                BuildingManager.instance.recipe = recipe;
                BuildingManager.instance.PickBuilding(building.gameObject, building.buildingType);

            }
        }

        
    }

    public void SetSlotValues(CraftingRecipe recipe)
    {
        this.recipe = recipe;
        BuildingPart buildingPart = (BuildingPart)recipe.outputs[0].item;
        text.text = buildingPart.name;
        building = buildingPart.prefab.GetComponent<Building>();
    }

}

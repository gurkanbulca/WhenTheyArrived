using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlacementUI : MonoBehaviour
{

    BuildingManager buildingManager;
    
    public Button rotateButton;


    private void Start()
    {
        buildingManager = BuildingManager.instance;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);

        GameObject highligtedObject = buildingManager.GetHighlight();
        if (highligtedObject != null)
        {
            switch (highligtedObject.transform.GetComponent<Building>().buildingType)
            {
                case BuildingType.Foundation:
                case BuildingType.Wall:
                case BuildingType.Doorway:
                case BuildingType.Door:
                    rotateButton.interactable = false;
                    break;
                default:
                    rotateButton.interactable = true;
                    break;
            }
        }
    }


    public void ReplaceButton()
    {
        GameObject buildingPart = transform.parent.gameObject;
        buildingManager.ReplaceProtocol(buildingPart, buildingPart.transform.parent, new Vector3(buildingPart.transform.position.x, buildingPart.transform.position.y, buildingPart.transform.position.z));
        buildingPart.transform.SetParent(null);
        transform.SetParent(null);
        buildingManager.PickBuilding(buildingPart, buildingPart.GetComponent<Building>().buildingType);

        buildingPart.transform.position = new Vector3(0, -100, 0);
        buildingPart.transform.rotation = Quaternion.Euler(0, 0, 0);
        gameObject.SetActive(false);

            
    
    }

    public void PlaceButton()
    {
        buildingManager.destroyHighlight();
    }

    public void DeleteButton()
    {
        GameObject buildingPart = transform.parent.gameObject;
        switch (buildingPart.GetComponent<Building>().buildingType)
        {
            case BuildingType.Doorway:
            case BuildingType.Foundation:
                for(int i = 0; i < buildingPart.transform.childCount; i++)
                {
                    if ((buildingPart.transform.GetChild(i).CompareTag("SnapPoint") || buildingPart.transform.GetChild(i).CompareTag("PlacementSlot")) && buildingPart.transform.GetChild(i).childCount > 0)
                    {
                        return;
                    }
                }
                break;
            default:
                break;
        }
        CraftingRecipe recipe = buildingPart.GetComponent<Building>().recipe;
        for (int i = 0; i < recipe.inputs.Count; i++)
        {
            ItemAmount input = recipe.inputs[i];
            Inventory inventory = Inventory.instance;
            if (inventory.hasEnoughSpaceForItem(input.item, input.amount))
            {
                inventory.Add(input.item, input.amount);
            }
            else
            {
                for (int j = 0; j < i; j++)
                {
                    inventory.Remove(recipe.inputs[j].item, recipe.inputs[j].amount);
                }
                Debug.LogError("Inventory is full!");
            }
        }

        transform.SetParent(null);
        gameObject.SetActive(false);
        

        Destroy(buildingPart);
        NavMeshBaker.instance.RemoveFromSurfaces(buildingPart.GetComponent<NavMeshSurface>());

    }


}

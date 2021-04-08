using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherableManager : MonoBehaviour
{
    public int health = 3;
    ItemPickup pickup;
    [Range(0,2)]
    public int gatherableLevel;
    public ToolType gatherTool;
    public int experience;

    private void Start()
    {
        pickup = GetComponent<ItemPickup>();
    }

    public void Gather(Tool tool)
    {
        if (CanGatherable(tool))
        {
            health--;
            if (health <= 0)
            {
                pickup.PickUp();
                StatManagement.instance.GainExperience(experience);
            }
        }
        else
        {
            Debug.LogError("Not have useful tool!");
        }
        
    }

    public bool CanGatherable(Tool tool)
    {
        if(tool?.toolType == gatherTool || tool?.toolType == ToolType.Multitool)
        {
            if(tool.gatherLevel >= gatherableLevel)
            {
                return true;
            }
        }
        return false;
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SmelterDropHandler : MonoBehaviour, IDropHandler
{

    Inventory inventory;
    SmelterController parentSmelter;

    private void Start()
    {
        inventory = Inventory.instance;
        parentSmelter = GetComponent<SmelterSlot>().parentSmelter;
    }

    public void OnDrop(PointerEventData eventData)
    {
        int draggingItemIndex = inventory.draggingItemIndex;
        

        if (draggingItemIndex > -1)
        {
            SmelterSlotType slotType = GetComponent<SmelterSlot>().slotType;
            if (parentSmelter.CanContain(inventory.items[draggingItemIndex],slotType))
            {
                parentSmelter.Add(inventory.items[draggingItemIndex],
                inventory.items[draggingItemIndex].amount,
                slotType,
                transform.GetSiblingIndex());
            }
            return;
        }
        int draggingInputIndex = parentSmelter.draggingInputIndex;
        int draggingOutputIndex = parentSmelter.draggingOutputIndex;
        int draggingFuelIndex = parentSmelter.draggingFuelIndex;
        int index = -1;
        Item[] stack=null;
        if(draggingInputIndex > -1)
        {
            index = draggingInputIndex;
            stack = parentSmelter.GetStackBySlotType(SmelterSlotType.Input);
        }
        else if(draggingOutputIndex > -1)
        {
            index = draggingOutputIndex;
            stack = parentSmelter.GetStackBySlotType(SmelterSlotType.Output);
        }
        else if (draggingFuelIndex > -1)
        {
            index = draggingFuelIndex;
            stack = parentSmelter.GetStackBySlotType(SmelterSlotType.Fuel);
        }

        if (index > -1)
        {
            SmelterSlotType toType = GetComponent<SmelterSlot>().slotType;
            if(parentSmelter.CanContain(stack[index], toType))
            {
                parentSmelter.Swap(stack[index]
                    , stack[index].amount
                    , toType
                    , transform.GetSiblingIndex());
            }
            SmelterUI smelthingUI = SmelterUI.instance;
            smelthingUI.smelter.draggingInputIndex = -1;
            smelthingUI.smelter.draggingOutputIndex = -1;
            smelthingUI.smelter.draggingFuelIndex = -1;
            Destroy(smelthingUI.copyIcon);
            smelthingUI.copyIcon = null;
        }

    }

}

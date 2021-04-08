using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProductionDropHandler : MonoBehaviour , IDropHandler
{

    Inventory inventory;
    StationController parentStation;

    private void Start()
    {
        inventory = Inventory.instance;
        parentStation = GetComponent<ProductionSlot>().parentStation;
    }

    public void OnDrop(PointerEventData eventData)
    {
        int draggingItemIndex = inventory.draggingItemIndex;
        if(draggingItemIndex != -1)
        {
            if (parentStation.CanContain(inventory.items[draggingItemIndex],ProductionSlotType.Input))
            {
                parentStation.Add(inventory.items[draggingItemIndex],
                inventory.items[draggingItemIndex].amount,
                transform.GetSiblingIndex());
            }
            return;
        }
        int draggingInputIndex = parentStation.draggingInputIndex;
        int draggingOutputIndex = parentStation.draggingOutputIndex;
        int index = -1;
        Item[] stack = null;

        if(draggingInputIndex> -1)
        {
            index = draggingInputIndex;
            stack = parentStation.GetStackBySlotType(ProductionSlotType.Input);
        }
        else if(draggingOutputIndex > -1)
        {
            index = draggingOutputIndex;
            stack = parentStation.GetStackBySlotType(ProductionSlotType.Output);
        }
        if(index > -1)
        {
            ProductionSlotType toType = GetComponent<ProductionSlot>().slotType;
            if (parentStation.CanContain(stack[index], toType))
            {
                parentStation.Swap(stack[index]
                    , stack[index].amount
                    , toType
                    , transform.GetSiblingIndex());
            }
            ProductionUI productionUI = ProductionUI.instance;
            productionUI.station.draggingInputIndex = -1;
            productionUI.station.draggingOutputIndex = -1;
            Destroy(productionUI.copyIcon);
            productionUI.copyIcon = null;
        }


    }

}

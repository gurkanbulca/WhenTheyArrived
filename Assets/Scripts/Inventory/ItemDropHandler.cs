using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    Inventory inventory;
    EquipmentManager equipmentManager;

    private void Start()
    {
        inventory = Inventory.instance;
        equipmentManager = EquipmentManager.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        StationController station = ProductionUI.instance?.station;
        SmelterController smelter = SmelterUI.instance?.smelter;
        StorageController storage = StorageUI.instance?.storage;
        int draggingItemIndex = inventory.draggingItemIndex;
        int draggingEquipmentIndex = equipmentManager.draggingEquipmentIndex;
        int draggingInputIndex = -1;
        int draggingOutputIndex = -1;
        int draggingFuelIndex = -1;
        int draggingStorageIndex = -1;
        if (station != null)
        {
            draggingInputIndex = station.draggingInputIndex;
            draggingOutputIndex = station.draggingOutputIndex;
        }
        if (smelter!=null)
        {
            draggingInputIndex = smelter.draggingInputIndex;
            draggingOutputIndex = smelter.draggingOutputIndex;
            draggingFuelIndex = smelter.draggingFuelIndex;
        }
        if(storage != null)
        {
            draggingStorageIndex = storage.draggingIndex;

        }
        Consumable draggingConsumable = equipmentManager.draggingConsumable;
        RectTransform slotPanel = transform as RectTransform;
        if (RectTransformUtility.RectangleContainsScreenPoint(slotPanel, Input.mousePosition))
        {
            int to = transform.parent == InventoryUI.instance.baseParent
                ? transform.GetSiblingIndex()
                : transform.GetSiblingIndex() + InventoryUI.instance.baseParent.childCount;
            if (draggingItemIndex != -1)
            {
                inventory.SwapItems(to, draggingItemIndex);
            }
            else if (draggingEquipmentIndex != -1)
            {
                equipmentManager.Unequip(draggingEquipmentIndex, to);
            }
            else if (draggingConsumable != null)
            {
                equipmentManager.UnequipConsumable(draggingConsumable, to);
            }
            else if(draggingStorageIndex != -1)
            {
                Item item = storage.items[draggingStorageIndex];

                if (inventory.Add(item, item.amount, to))
                {
                    storage.Remove(item, item.amount, draggingStorageIndex);
                }
                else
                {
                    StorageUI.instance.UpdateStorage(storage.items,storage);
                }
                storage.draggingIndex = -1;
                Destroy(StorageUI.instance.copyIcon);
                StorageUI.instance.copyIcon = null;
                
            }

            else if (draggingInputIndex != -1 || draggingOutputIndex != -1 || draggingFuelIndex != -1)
            {
                Item item;
                if (station != null)
                {
                    if (draggingInputIndex != -1)
                    {
                        item = station.inputs[draggingInputIndex];
                    }
                    else
                    {
                        item = station.outputs[draggingOutputIndex];
                    }
                    inventory.Add(item, item.amount, transform.GetSiblingIndex());
                    station.DestroyItem(item);
                    station.draggingInputIndex = -1;
                    station.draggingOutputIndex = -1;
                    Destroy(ProductionUI.instance.copyIcon);
                }
                else
                {
                    if (draggingInputIndex != -1)
                    {
                        item = smelter.inputs[draggingInputIndex];
                    }
                    else if(draggingOutputIndex!=-1)
                    {
                        item = smelter.outputs[draggingOutputIndex];
                    }
                    else
                    {
                        item = smelter.fuels[draggingFuelIndex];
                    }
                    inventory.Add(item, item.amount, transform.GetSiblingIndex());
                    smelter.DestroyItem(item);
                    smelter.draggingInputIndex = -1;
                    smelter.draggingOutputIndex = -1;
                    smelter.draggingFuelIndex = -1;
                    Destroy(SmelterUI.instance.copyIcon);
                }
                
            }
        }
    }


}

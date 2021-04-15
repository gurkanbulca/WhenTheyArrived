using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemContainer
{
    bool hasEnoughSpaceForItem(Item item, int amount);
    bool IsContainsItem(Item item, int amount);
    bool Add(Item newItem, int amount , int inventorySlot = -1);
    void Remove(Item item, int amount = 1,int slotIndex = -1);
    bool IsFull();
    int ItemCount(Item item);

    bool DestroyItem(Item item, int amount = 0);

}

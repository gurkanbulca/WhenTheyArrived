using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public Vector2Int amountRange;
    int amount;
    public int experience;

    

    public void PickUp()
    {
        Debug.Log("Picking up " + item.name);
        bool isPickedUp;
        Item copyItem = Instantiate(item);
        if (item.isStackable)
        {
            amount = Random.Range(amountRange.x, amountRange.y+1);
        }

        isPickedUp = Inventory.instance.Add(copyItem,amount);
        if (isPickedUp)
        {
            StatManagement.instance.GainExperience(experience);
            Destroy(gameObject);
        }
    }
}

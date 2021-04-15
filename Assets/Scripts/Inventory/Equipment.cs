using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment",menuName ="Inventory/Equipment")]
public class Equipment : Item


{
    public EquipmentTypes equipSlot;
    public int armorModifier;
    public int damageModifier;


    public GameObject mesh;
    public GameObject secondMesh;

    [SerializeField]
    int maxDurability;
    int currentDurability;

    private void Awake()
    {
        currentDurability = maxDurability;
    }



    public override bool Use()
    {
        if (!base.Use())
        {
            return false;
        }
        // Remove it from the inventory
        RemoveFromInventory();
        // Equip it to equipment slot
        EquipmentManager.instance.Equip(this);
        return true;
    }

    public void SetCurrentDurability(int amount)
    {
        currentDurability = amount;
    }

    public int GetCurrentDurability()
    {
        return currentDurability;
    }

    public void SetMaxDurability(int amount)
    {
        maxDurability = amount;
    }

    public int GetMaxDurability()
    {
        return maxDurability;
    }


}


public enum EquipmentTypes
{
    Head,Chest,Legs, Foot,Weapon, Backpack,Pocket
}

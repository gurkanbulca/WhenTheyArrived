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



    public override void Use()
    {
        base.Use();
        // Remove it from the inventory
        RemoveFromInventory();
        // Equip it to equipment slot
        EquipmentManager.instance.Equip(this);
    }

}


public enum EquipmentTypes
{
    Head,Chest,Legs, Foot,Weapon, Backpack,Pocket
}

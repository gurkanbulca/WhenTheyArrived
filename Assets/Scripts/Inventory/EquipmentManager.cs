using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton
    public static EquipmentManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    GameObject[] currentMeshes;
    Skeleton skeleton;
    Inventory inventory;
    StatManagement stats;
    GameObject[] visualEquipments;

    public Equipment[] currentEquipment;
    public Consumable[] pockets;
    public delegate void OnEquipmentChanged(Item newItem, Item oldItem,int slotIndex=-1);
    public OnEquipmentChanged onEquipmentChanged;
    public int draggingEquipmentIndex = -1;
    public int pocketCount=2;
    public Consumable draggingConsumable = null;

    private void Start()
    {
        inventory = Inventory.instance;
        stats = StatManagement.instance;
        int numSlots = System.Enum.GetNames(typeof(EquipmentTypes)).Length;
        currentEquipment = new Equipment[numSlots];
        visualEquipments = new GameObject[numSlots];
        currentMeshes = new GameObject[numSlots + 1];
        pockets = new Consumable[pocketCount];

        skeleton = GameObject.FindGameObjectWithTag("Player").GetComponent<Skeleton>();
    }

    public void VisualEquip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipSlot;
        if (visualEquipments[slotIndex] != null)
        {
            Destroy(visualEquipments[slotIndex]);
        }
        if (currentEquipment[slotIndex] != null)
        {
            GameObject oldItem = currentMeshes[slotIndex];
            oldItem.SetActive(false);
            if (currentEquipment[slotIndex].secondMesh != null)
            {
                currentMeshes[currentMeshes.Length - 1].SetActive(false);
            }
        }

        GameObject secondEquipmentBone;
        GameObject equipmentBone = skeleton.getBoneBySlotIndex(slotIndex, out secondEquipmentBone);
        GameObject newMesh = Instantiate(newItem.mesh, equipmentBone.transform);
        visualEquipments[slotIndex] = newMesh;
        if (secondEquipmentBone != null)
        {
            Destroy(visualEquipments[visualEquipments.Length - 1]);
            GameObject extraMesh = Instantiate(newItem.secondMesh, secondEquipmentBone.transform);
            visualEquipments[visualEquipments.Length - 1] = extraMesh;
        }
    }

    public void VisualUnequip(int slotIndex)
    {
        currentEquipment[slotIndex]?.mesh.SetActive(true);
        GameObject secondEquipmentBone;
        GameObject equipmentBone = skeleton.getBoneBySlotIndex(slotIndex, out secondEquipmentBone);
        if(secondEquipmentBone != null)
        {
            Destroy(visualEquipments[visualEquipments.Length-1]);
            visualEquipments[visualEquipments.Length - 1] = null;
        }
        Destroy(visualEquipments[slotIndex]);
        visualEquipments[slotIndex] = null;

        currentMeshes[slotIndex]?.SetActive(true);
        if(slotIndex == (int)EquipmentTypes.Foot)
        {
            currentMeshes[currentMeshes.Length-1].SetActive(true);
        }
    }

    public void Equip(Equipment newItem)
    {
            int slotIndex = (int)newItem.equipSlot;

            Equipment oldItem = null;

            if (currentEquipment[slotIndex] != null)
            {
                oldItem = currentEquipment[slotIndex];
                Unequip(slotIndex);
            }

            

            currentEquipment[slotIndex] = newItem;
            GameObject secondEquipmentBone;
            GameObject equipmentBone = skeleton.getBoneBySlotIndex(slotIndex, out secondEquipmentBone);
            GameObject newMesh = Instantiate(newItem.mesh, equipmentBone.transform);
            currentMeshes[slotIndex] = newMesh;
            if (secondEquipmentBone != null)
            {
                GameObject extraMesh = Instantiate(newItem.secondMesh, secondEquipmentBone.transform);
                currentMeshes[currentMeshes.Length - 1] = extraMesh;
            }

            // Update stats
            stats.UpdateDamageModifier(newItem.damageModifier);
            stats.UpdateArmorModifier(newItem.armorModifier);
            // Update EquipmentUI
            onEquipmentChanged?.Invoke(newItem, oldItem);

    }

    public void Unequip(int slotIndex, int inventorySlot = -1)
    {
        if (currentEquipment[slotIndex] != null)
        {
            if (currentMeshes[slotIndex] != null)
            {
                Destroy(currentMeshes[slotIndex].gameObject);
                if (slotIndex == (int)EquipmentTypes.Foot)
                {
                    Destroy(currentMeshes[currentMeshes.Length - 1].gameObject);

                }
            }
            Equipment oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem, 1, inventorySlot);

            currentEquipment[slotIndex] = null;


            // Update stats
            stats.UpdateDamageModifier(-oldItem.damageModifier);
            stats.UpdateArmorModifier(-oldItem.armorModifier);
            // Update EquipmentUI
            onEquipmentChanged?.Invoke(null, oldItem);

        }
    }

    public Equipment GetWeapon()
    {
        return currentEquipment[(int)EquipmentTypes.Weapon];
    }

    public void WeaponDurabilityDamage(int amount=1)
    {
        foreach(var equipment in currentEquipment)
        {
            if (equipment != null)
            {
                if (equipment.equipSlot == EquipmentTypes.Weapon)
                {
                    EquipmentDurabilityDamage(equipment,amount);
                }
            }
        }
    }

    public void ArmorDurabilityDamage(int amount=1)
    {
        EquipmentTypes[] armors = new EquipmentTypes[] {EquipmentTypes.Chest,EquipmentTypes.Foot,EquipmentTypes.Head,EquipmentTypes.Legs};
        foreach (var equipment in currentEquipment)
        {
            if (equipment != null)
            {
                if (Array.IndexOf(armors, equipment.equipSlot) > -1)
                {
                    EquipmentDurabilityDamage(equipment,amount);
                }
            }
        }
    }

    void EquipmentDurabilityDamage(Equipment equipment,int amount)
    {
        equipment.SetCurrentDurability(equipment.GetCurrentDurability() - amount);
        if (equipment.GetCurrentDurability() <= 0)
        {
            DestroyEquipment(equipment);
        }
        else
        {
            onEquipmentChanged?.Invoke(equipment, null);
        }
    }

    public void DestroyEquipment(Equipment equipment)
    {
        for(int i = 0; i < currentEquipment.Length; i++)
        {
            if (currentEquipment[i] == equipment)
            {
                currentEquipment[i] = null;
                onEquipmentChanged?.Invoke(null, equipment);
                stats.UpdateDamageModifier(-equipment.damageModifier);
                stats.UpdateArmorModifier(-equipment.armorModifier);
                Destroy(currentMeshes[(int)equipment.equipSlot]);
                if (equipment.equipSlot == EquipmentTypes.Foot)
                {
                    Destroy(currentMeshes[currentMeshes.Length - 1].gameObject);
                }
                return;
            }
        }
    }

    public void EquipConsumable(Consumable consumable, int slotIndex)
    {
        Consumable oldItem = null;
        if (pockets[slotIndex] != null)
        {
            oldItem = pockets[slotIndex];
            if (consumable.name.Equals(oldItem.name))
            {
                if (consumable.amount + oldItem.amount > consumable.stackSize)
                {
                    int overflowAmount = consumable.amount + oldItem.amount - consumable.stackSize;
                    oldItem.amount = oldItem.stackSize;
                    consumable.amount = overflowAmount;
                    Inventory.instance.onItemChangedCallback?.Invoke();
                }
                else
                {
                   oldItem.amount += consumable.amount;
                   Inventory.instance.DestroyItem(consumable);
                }
            }
            else
            {
                pockets[slotIndex] = consumable;
                Inventory.instance.DestroyItem(consumable);
                Inventory.instance.Add(oldItem,oldItem.amount);
            }
        }
        else
        {
            pockets[slotIndex] = consumable;
            Inventory.instance.DestroyItem(consumable);
        }
        onEquipmentChanged?.Invoke(pockets[slotIndex], consumable, slotIndex);

    }

    public void UnequipConsumable(Consumable item, int inventorySlot = -1)
    {
        for (int i = 0; i < pockets.Length; i++)
        {
            if (pockets[i] == item)
            {
                pockets[i] = null;
                inventory.Add(item, item.amount, inventorySlot);
                if (onEquipmentChanged != null)
                {
                    onEquipmentChanged?.Invoke(null, item, i);
                }
            }
        }
    }

    

    public void RemoveFromPockets(int slotIndex,int amount)
    {
        pockets[slotIndex].amount -= amount;
        if (pockets[slotIndex].amount<=0)
        {
            Item oldItem = pockets[slotIndex];
            pockets[slotIndex] = null;
            onEquipmentChanged.Invoke(null, oldItem,slotIndex);
        }
        else
        {
            onEquipmentChanged.Invoke(pockets[slotIndex], null,slotIndex);
        }
    }

    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnequipAll();
        }
    }

    public Equipment CurrentEquipmentByEquipmentType(EquipmentTypes type)
    {
        return currentEquipment[(int)type];
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton :MonoBehaviour
{
    public GameObject headBone;
    public GameObject chestBone;
    public GameObject leggingBone;
    public GameObject leftFootBone;
    public GameObject rightFootBone;
    public GameObject weaponHandBone;

    public GameObject getBoneBySlotIndex(int slotIndex, out GameObject extraBone)
    {
        extraBone = null;
        switch ((EquipmentTypes)slotIndex)
        {
            case EquipmentTypes.Head:
                return headBone;
            case EquipmentTypes.Chest:
                return chestBone;
            case EquipmentTypes.Legs:
                return leggingBone;
            case EquipmentTypes.Foot:
                extraBone = leftFootBone;
                return rightFootBone;
            case EquipmentTypes.Weapon:
                return weaponHandBone;
            default:
                return null;
        }
    }


}

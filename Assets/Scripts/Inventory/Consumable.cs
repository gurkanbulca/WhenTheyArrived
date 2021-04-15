using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    public Effect[] effects;
    public float nutrition;
    public float hydration;

    public override bool Use()
    {
        if (!base.Use())
        {
            return false;
        }
        foreach (Effect effect in effects)
        {
            switch (effect.effect)
            {
                case EffectTypes.Heal:
                    StatManagement.instance.Heal(effect.amount);
                    break;
                case EffectTypes.HealthBuff:
                case EffectTypes.DamageBuff:
                case EffectTypes.ArmorBuff:
                case EffectTypes.SpeedBuff:
                    Debug.Log(effect.effect +" " + effect.amount +" "+ effect.duration);
                    StatManagement.instance.ApplyBuff(effect.effect, effect.amount, effect.duration);
                    break;
                default:
                    break;
            }
        }
        Remove();


        StatManagement.instance.UpdateHunger(nutrition);
        StatManagement.instance.UpdateThirst(hydration);
        return true;
    }

    void Remove()
    {
        Item[] items = Inventory.instance.items;
        for (int i=0;i<items.Length;i++)
        {
            Consumable consumable = items[i]  as Consumable;
            if (consumable != null)
            {
                if(consumable == this)
                {
                    Debug.Log("test");
                    Inventory.instance.Remove(items[i], 1, i);
                    return;
                }
            }
        }
        Consumable[] pockets = EquipmentManager.instance.pockets;
        for(int i = 0; i < pockets.Length; i++)
        {
            if (pockets[i] == this)
            {
                EquipmentManager.instance.RemoveFromPockets(i, 1);
                return;
            }
        }
    }
}



public enum EffectTypes
{
    Heal,HealthBuff,DamageBuff,ArmorBuff,SpeedBuff
}

using UnityEngine;

[System.Serializable]
public class Effect
{
    public EffectTypes effect;
    public float amount;
    public int duration; 

    Effect(EffectTypes effect,float amount,int duration = 0)
    {
        this.effect = effect;
        this.amount = amount;
        this.duration = duration;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManagement : MonoBehaviour
{
    #region Singleton
    public static StatManagement instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public float maxHealth = 200;
    public float damageModifier = 10;
    public float armorModifier = 0;
    public float speedModifier = 1;
    public bool isDead = false;
    public float maxDefenceModifier = 70;
    public float hunger=100,thirst = 100;
    public float hungerRate = 0.1f, thirstRate = 0.1f;
    public int level = 1;
    public int currentExperience;
    public int baseExperience = 100;
    public int requiredExperience;


    public delegate void OnStatChange();
    public OnStatChange onStatChangedCallback;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        SetRequiredExperience();
        onStatChangedCallback?.Invoke();
    }

    private void Update()
    {
        // Basic hunger and thirst consume
        if (!isDead)
        {
            UpdateHunger(-Time.deltaTime * hungerRate);
        }
        if (!isDead)
        {
            UpdateThirst(-Time.deltaTime * thirstRate);
        }

    }

    void SetRequiredExperience()
    {
        requiredExperience = baseExperience * level;
    }

    void LevelUp()
    {
        level++;
        SetRequiredExperience();
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount;
        if(currentExperience> requiredExperience)
        {
            currentExperience -= requiredExperience;
            LevelUp();
        }
        onStatChangedCallback?.Invoke();
    }



    public void UpdateHunger(float amount)
    {
        hunger += amount;
        hunger = Mathf.Clamp(hunger,0, 100);
        if (hunger <= 0) Die();
        onStatChangedCallback?.Invoke();
    }

    public void UpdateThirst(float amount)
    {
        thirst += amount;
        thirst = Mathf.Clamp(thirst,0, 100);
        if (thirst <= 0) Die();
        onStatChangedCallback?.Invoke();
    }

    public void UpdateMaxHealth(float amount)
    {
        this.maxHealth += amount;
        Heal(amount);
        
        if (onStatChangedCallback != null)
        {
            onStatChangedCallback.Invoke();
        }
    }

    public void UpdateArmorModifier(float amount)
    {
        this.armorModifier += amount;
        if(onStatChangedCallback != null)
        {
            onStatChangedCallback.Invoke();
        }
    }

    public void UpdateDamageModifier(float amount)
    {
        this.damageModifier += amount;
        if (onStatChangedCallback != null)
        {
            onStatChangedCallback.Invoke();
        }
    }

    public void UpdateSpeedModifier(float amount)
    {
        this.speedModifier += amount;
        if (onStatChangedCallback != null)
        {
            onStatChangedCallback.Invoke();
        }
    }

    public float getCurrentHealth()
    {
        return this.currentHealth;
    }

    public void Heal(float amount)
    {
        this.currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        if (onStatChangedCallback != null)
        {
            onStatChangedCallback.Invoke();
        }
    }

    float ComputeDefenceMultiplier()
    {
        return 1 - (Mathf.Min(armorModifier, maxDefenceModifier) / 100);
    }

    public void ReceiveDamage(float damage)
    {
        float multiplier = ComputeDefenceMultiplier();
        damage *= multiplier;
        this.currentHealth -= damage;
        this.currentHealth = (float)System.Math.Round(this.currentHealth, 1);
        if(this.currentHealth <= 0)
        {
            this.currentHealth = 0;
            Die();
        }
        if (onStatChangedCallback != null)
        {
            onStatChangedCallback.Invoke();
        }
    }

    public void ApplyBuff(EffectTypes effect,float amount,int duration)
    {
        StartCoroutine(ApplyBuffCoroutine(effect, amount, duration));
    }

    IEnumerator ApplyBuffCoroutine(EffectTypes effect, float amount, int duration)
    {
        switch (effect)
        {
            case EffectTypes.HealthBuff:
                UpdateMaxHealth(amount);
                yield return new WaitForSeconds(duration);

                UpdateMaxHealth(-amount);
                break;
            case EffectTypes.DamageBuff:
                UpdateDamageModifier(amount);
                yield return new WaitForSeconds(duration);
                UpdateDamageModifier(-amount);
                break;
            case EffectTypes.ArmorBuff:
                UpdateArmorModifier(amount);
                yield return new WaitForSeconds(duration);
                UpdateArmorModifier(-amount);
                break;
            case EffectTypes.SpeedBuff:
                UpdateSpeedModifier(amount);
                yield return new WaitForSeconds(duration);
                UpdateSpeedModifier(-amount);
                break;
            default:
                break;
        }
    }

    void Die()
    {
        print("Player dead!");
        this.isDead = true;
        // do something on die condition
    }

}

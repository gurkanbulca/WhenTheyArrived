using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsUI : MonoBehaviour
{
    public GameObject statsUI;
    public Slider healthSlider;
    public TMP_Text healthText;
    public TMP_Text damageText;
    public TMP_Text armorText;
    public TMP_Text speedText;
    public TMP_Text hungerText;
    public TMP_Text thirstText;
    public TMP_Text levelText;
    public ProgressBarPro experienceBar;
    public ProgressBarPro healthBar;


    StatManagement stats;

    private void Start()
    {
        stats = StatManagement.instance;
        stats.onStatChangedCallback += UpdateUI;
    }

    void UpdateUI()
    {
        // Health
        healthText.text = ((int)stats.getCurrentHealth()).ToString();
        healthSlider.maxValue = stats.maxHealth;
        healthSlider.value = (int)stats.getCurrentHealth();
        healthBar.SetValue((int)stats.getCurrentHealth(), stats.maxHealth);
        // RPG Stats
        damageText.text = stats.damageModifier.ToString();
        armorText.text = stats.armorModifier.ToString();
        speedText.text = stats.speedModifier.ToString();
        // Survival Stats
        hungerText.text = ((int)stats.hunger).ToString();
        thirstText.text = ((int)stats.thirst).ToString();
        // Level and Experience
        levelText.text = stats.level.ToString("00");
        experienceBar.SetValue(stats.currentExperience, stats.requiredExperience);
    }
}

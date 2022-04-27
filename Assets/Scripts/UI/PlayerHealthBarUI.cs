using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    public GameObject Player;

    Image healthSlider;

    Text healthText;

    Text levelText;

    CharacterStats currentStats;


    void Awake()
    {
        currentStats = Player.GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdatePlayerHealthBar;
    }

    void OnEnable()
    {
        healthSlider = gameObject.transform.GetChild(0).GetComponent<Image>();
        healthText = gameObject.transform.GetChild(1).GetComponent<Text>();
        levelText = gameObject.transform.GetChild(1).GetComponent<Text>();
    }

    void Start()
    {
        healthSlider.fillAmount = 1f;
        healthText.text = currentStats.CurrentHealth + "/" + currentStats.MaxHealth;
    }

    private void UpdatePlayerHealthBar(int currentHealth, int maxHealth)
    {
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;

        healthText.text = currentHealth + "/" + currentStats.MaxHealth;
    }
}

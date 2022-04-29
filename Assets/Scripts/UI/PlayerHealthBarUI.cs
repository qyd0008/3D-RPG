using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    public GameObject Player;

    Image healthSlider;

    Image expSlider;

    Text healthText;

    Text levelText;

    Text expText;

    CharacterStats currentStats;


    void Awake()
    {
        currentStats = Player.GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdatePlayerHealthBar;
        currentStats.UpdateExpBarOnAttack += UpdatePlayerExpBar;
        currentStats.UpdateLevelOnAttack += UpdatePlayerLevel;
    }

    void OnEnable()
    {
        healthSlider = gameObject.transform.GetChild(0).GetComponent<Image>();
        healthText = gameObject.transform.GetChild(1).GetComponent<Text>();
        levelText = gameObject.transform.GetChild(2).GetComponent<Text>();
        expSlider = gameObject.transform.GetChild(3).GetComponent<Image>();
        expText = gameObject.transform.GetChild(4).GetComponent<Text>();
    }

    void Start()
    {
        healthSlider.fillAmount = (float)currentStats.CurrentHealth / currentStats.MaxHealth;;
        healthText.text = currentStats.CurrentHealth + "/" + currentStats.MaxHealth;

        expSlider.fillAmount = (float)currentStats.CurrentExp / currentStats.BaseExp;
        expText.text = currentStats.CurrentExp + "/" + currentStats.BaseExp;

        levelText.text = currentStats.CurrentLevel + "";
    }

    private void UpdatePlayerHealthBar(int currentHealth, int maxHealth)
    {
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;

        healthText.text = currentHealth + "/" + currentStats.MaxHealth;
    }

    private void UpdatePlayerExpBar(int currentExp, int baseExp)
    {
        float sliderPercent = (float)currentExp / baseExp;
        expSlider.fillAmount = sliderPercent;

        expText.text = currentExp + "/" + baseExp;
    }

    private void UpdatePlayerLevel(int level)
    {
        levelText.text = level + "";
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject HealthUIPrefab;

    public Transform barPoint;

    public bool alwaysVisible;

    public float visibleTime;

    private float visibleLate;

    Image healthSlider;

    Text healthText;

    Transform UIbar;

    Transform cam;

    CharacterStats currentStats;

    void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            Debug.Log("canvas.name="+canvas.name);
            if (canvas.name == "HealthBar Canvas")
            {
                UIbar = Instantiate(HealthUIPrefab,canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                healthText = UIbar.GetChild(1).GetComponent<Text>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    void Start()
    {
        visibleLate = visibleTime;
        healthSlider.fillAmount = 1f;
        healthText.text = currentStats.MaxHealth + "/" + currentStats.MaxHealth;
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);

        visibleLate = visibleTime;
        UIbar.gameObject.SetActive(true);

        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;

        healthText.text = currentHealth + "/" + currentStats.MaxHealth;
    }

    void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;

            visibleLate -= Time.deltaTime;
            if (visibleLate <= 0 && !alwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }
        }
    }
}

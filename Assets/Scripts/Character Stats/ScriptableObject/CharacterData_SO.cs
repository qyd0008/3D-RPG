using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data ")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public string myName; //名称
    public int maxHealth; //最大血量
    public int currentHealth; //当前血量
    public int baseDefence; //基础防御
    public int currentDefence; //当前防御

    [Header("Kill")]
    public int killPoint;

    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;

    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    public bool UpdateExp(int point)
    {
        currentExp += point;

        if (currentExp >= baseExp)
        {
            currentExp = 0;
            LevelUp();
            return true;
        }

        return false;
    }

    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1,0,maxLevel);

        baseExp += (int)(baseExp*LevelMultiplier);

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

        int addDefence = Mathf.Max((int)(baseDefence * levelBuff),1);
        baseDefence += addDefence;
        currentDefence = baseDefence;

        Debug.Log("Level UP!");
    }
}

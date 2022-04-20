﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    #region Read From Data_SO
    public string MyName
    {
        get
        {
            if (characterData != null)
                return characterData.myName;
            return "";
        }
        set
        {
            if (characterData != null)
                characterData.myName = value;
        }
    }
    
    public int MaxHealth
    {
        get
        {
            if (characterData != null)
                return characterData.maxHealth;
            return 0;
        }
        set
        {
            if (characterData != null)
                characterData.maxHealth = value;
        }
    }
    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            return 0;
        }
        set
        {
            if (characterData != null)
                characterData.currentHealth = value;
        }
    }
    public int BaseDefence
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefence;
            return 0;
        }
        set
        {
            if (characterData != null)
                characterData.baseDefence = value;
        }
    }
    public int CurrentDefence
    {
        get
        {
            if (characterData != null)
                return characterData.currentDefence;
            return 0;
        }
        set
        {
            if (characterData != null)
                characterData.currentDefence = value;
        }
    }
    #endregion

    #region Read From Attack_SO
    public float AttackRange
    {
        get
        {
            if (attackData != null)
                return attackData.attackRange;
            return 0;
        }
        set
        {
            if (attackData != null)
                attackData.attackRange = value;
        }
    }

    public float SkillRange
    {
        get
        {
            if (attackData != null)
                return attackData.skillRange;
            return 0;
        }
        set
        {
            if (attackData != null)
                attackData.skillRange = value;
        }
    }

    public float CoolDown
    {
        get
        {
            if (attackData != null)
                return attackData.coolDown;
            return 0;
        }
        set
        {
            if (attackData != null)
                attackData.coolDown = value;
        }
    }

    public int MinDamage
    {
        get
        {
            if (attackData != null)
                return attackData.minDamage;
            return 0;
        }
        set
        {
            if (attackData != null)
                attackData.minDamage = value;
        }
    }

    public int MaxDamage
    {
        get
        {
            if (attackData != null)
                return attackData.maxDamage;
            return 0;
        }
        set
        {
            if (attackData != null)
                attackData.maxDamage = value;
        }
    }

    public float CriticalMultiplier
    {
        get
        {
            if (attackData != null)
                return attackData.criticalMultiplier;
            return 0;
        }
        set
        {
            if (attackData != null)
                attackData.criticalMultiplier = value;
        }
    }

    public float CriticalChance
    {
        get
        {
            if (attackData != null)
                return attackData.criticalChance;
            return 0;
        }
        set
        {
            if (attackData != null)
                attackData.criticalChance = value;
        }
    }

    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int currentDamage = attacker.CurrentDamage();
        int damage = Mathf.Max(currentDamage - defener.CurrentDefence, 0);
        defener.CurrentHealth = Mathf.Max(defener.CurrentHealth - damage, 0);

        //如果暴击了 防御者要播放hit受击动画
        if (isCritical)
        {
            Debug.Log(attacker.MyName + "触发暴击，暴击伤害:" + currentDamage + ",敌人防御：" + defener.CurrentDefence + ",实际造成伤害：" + damage);
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        //TODO: update ui
        //TODO: update exp
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(MinDamage,MaxDamage);

        if (isCritical)
        {
            coreDamage *= CriticalMultiplier;
        }

        return (int)coreDamage;
    }

    #endregion
}

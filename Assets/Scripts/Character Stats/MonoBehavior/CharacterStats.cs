using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int,int> UpdateHealthBarOnAttack;
    public event Action<int,int> UpdateExpBarOnAttack;
    public event Action<int> UpdateLevelOnAttack;
    public event Action<int,bool> BleedingOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }

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

    public int KillPoint
    {
        get
        {
            if (characterData != null)
                return characterData.killPoint;
            return 0;
        }
        set
        {
            if (characterData != null)
                characterData.killPoint = value;
        }
    }

    public int CurrentLevel
    {
        get
        {
            if (characterData != null)
                return characterData.currentLevel;
            return 0;
        }
        set
        {
            if (characterData != null)
                characterData.currentLevel = value;
        }
    }

    public int BaseExp
    {
        get
        {
            if (characterData != null)
                return characterData.baseExp;
            return 0;
        }
        set
        {
            if (characterData != null)
                characterData.baseExp = value;
        }
    }

    public int CurrentExp
    {
        get
        {
            if (characterData != null)
                return characterData.currentExp;
            return 0;
        }
        set
        {
            if (characterData != null)
                characterData.currentExp = value;
        }
    }

    public bool UpdateExp(int point)
    {
        if (characterData != null)
            return characterData.UpdateExp(point);

        return false;
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
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
            if (defener.MyName == "DogKnight")
            {
                AudioManager.Instance.PlaySound("Sound/beAttacked");
            }
        }
        defener.BleedingOnAttack?.Invoke(damage,attacker.isCritical);

        Debug.Log(attacker.MyName + "发起攻击，伤害为:" + currentDamage + ",受击者" + defener.MyName + "防御：" + defener.CurrentDefence + ",实际造成伤害：" + damage);
        Debug.Log("受击者" + defener.MyName + "剩余血量：" + defener.CurrentHealth);

        //update ui
        defener.UpdateHealthBarOnAttack?.Invoke(defener.CurrentHealth,defener.MaxHealth);
        //update exp
        if (defener.CurrentHealth <= 0)
        {
            bool isLevelUp = attacker.UpdateExp(defener.KillPoint);
            attacker.UpdateExpBarOnAttack?.Invoke(attacker.CurrentExp,attacker.BaseExp);
            //如果升级了 那么更新血条 因为升级血就满了
            if (isLevelUp)
            {
                attacker.UpdateHealthBarOnAttack?.Invoke(attacker.CurrentHealth,attacker.MaxHealth);
                attacker.UpdateLevelOnAttack?.Invoke(attacker.CurrentLevel);
            }
        }
    }

    public void TakeDamage(int damage, CharacterStats defener)
    {
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);
        defener.CurrentHealth = Mathf.Max(defener.CurrentHealth - currentDamage, 0);

        defener.BleedingOnAttack?.Invoke(currentDamage,false);

        Debug.Log("受到攻击 例如石头等，伤害为:" + damage + ",受击者" + defener.MyName + "防御：" + defener.CurrentDefence + ",实际造成伤害：" + currentDamage);
        Debug.Log("受击者" + defener.MyName + "剩余血量：" + defener.CurrentHealth);

        // update ui
        UpdateHealthBarOnAttack?.Invoke(defener.CurrentHealth,defener.MaxHealth);
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

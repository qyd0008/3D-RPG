using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange; //近战攻击距离
    public float skillRange; //技能或者远战攻击距离
    public float coolDown; //cd时间
    public int minDamage; //最小损伤
    public int maxDamage; //最大损伤

    public float criticalMultiplier; //暴击倍数
    public float criticalChance; //暴击概率
}

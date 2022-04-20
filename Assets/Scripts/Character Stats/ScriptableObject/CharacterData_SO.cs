﻿using System.Collections;
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
}

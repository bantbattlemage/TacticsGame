using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : GameData
{
    public int ID;
    public string Name;

    public override GameDataType DataType => GameDataType.Player;
}
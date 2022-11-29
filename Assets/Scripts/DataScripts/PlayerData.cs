using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum GamePlayerState
{
	Idle_InactivePlayer,
	Idle_ActivePlayer,
	UnitMoveAction,
	UnitAttackAction
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : GameData
{
	public int ID;
	public string Name;
	public GamePlayerState State;

	public override GameDataType DataType => GameDataType.Player;
}
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
	UnitAttackAction,
	BuildingBuyAction
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : GameData
{
	public int ID;
	public string Name;
	public int Money;

	public GamePlayerState State;

	public override GameDataType DataType => GameDataType.Player;
}
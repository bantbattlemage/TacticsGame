using NesScripts.Controls.PathFind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEntityState
{
	NeutralPlayerControlled,
	InactivePlayerControlled,
	ActiveAndReady,
	ActiveNoActionsAvailable
}

[CreateAssetMenu(fileName = "GameEntityData", menuName = "ScriptableObjects/GameEntityData", order = 1)]
public class GameEntityData : GameData
{
	public GameDefinition Definition;
	public int Owner = GamePlayer.NEUTRAL_PLAYER_ID;
	public Point Location;
	public virtual int RemainingActions { get { return 0; } }
	public GameEntityState State;

	public override GameDataType DataType
	{
		get
		{
			return GameDataType.Entity;
		}
	}

	public override GameDataType OutputDataToString(out string info)
	{
		info = "";

		return DataType;
	}
}
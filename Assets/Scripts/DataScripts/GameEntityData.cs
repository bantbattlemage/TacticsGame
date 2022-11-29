using NesScripts.Controls.PathFind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEntityData", menuName = "ScriptableObjects/GameEntityData", order = 1)]
public class GameEntityData : GameData
{
	public GameDefinition Definition;
	public int Owner = -1;
	public Point Location;

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
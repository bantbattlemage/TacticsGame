using System.Collections;
using System.Collections.Generic;
using TacticGameData;
using UnityEngine;

public class GameDataObject : ScriptableObject, IGameData, IGameDataProvider<GameData>
{
	public virtual GameDataType DataType
	{
		get
		{
			return GameDataType.UNASSIGNED;
		}
	}

	public int Sender { get; set; }

	public virtual GameData GetData()
	{
		GameData data = new GameData();

		return data;
	}

	public virtual GameDataType OutputDataToString(out string info)
	{
		info = "";

		return DataType;
	}
}
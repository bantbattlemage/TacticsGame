using System.Collections;
using System.Collections.Generic;
using TacticGameData;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectTooltip : MonoBehaviour
{
	public GameData DataSource
	{
		get
		{
			if (GetComponent<GameTile>())
			{
				return GetComponent<GameTile>().TileData;
			}

			if (GetComponent<GameEntity>())
			{
				return GetComponent<GameEntity>().Data;
			}

			return null;
		}
	}

}

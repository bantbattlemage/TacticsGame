using System.Collections;
using System.Collections.Generic;
using TacticGameData;
using UnityEngine;

public class GameDefinitionObject : GameDataObject, IGameDefinition, IGameDataProvider<GameDefinition>
{
	[field: SerializeField]
	public int BaseHealth { get; set; }

	public GameObject Prefab;

	public virtual GameEntityType EntityType { get { return GameEntityType.UNASSIGNED; } }

	public virtual new GameDefinition GetData()
	{
		return new GameDefinition();
	}
}
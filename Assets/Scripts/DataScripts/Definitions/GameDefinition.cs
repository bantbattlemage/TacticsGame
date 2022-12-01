using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDefinition : GameData
{
	public int BaseHealth;

	public virtual GameEntityType EntityType { get { return GameEntityType.UNASSIGNED; } }
	public GameObject Prefab;
}
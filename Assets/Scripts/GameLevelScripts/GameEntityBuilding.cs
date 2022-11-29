using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityBuilding : GameEntity
{
	public BuildingData TypedData { get { return Data as BuildingData; } }
}
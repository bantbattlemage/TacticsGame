using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/BuildingData", order = 1)]
public class BuildingData : GameEntityData
{
	public BuildingDefinition TypedDefinition
	{
		get
		{
			return Definition as BuildingDefinition;
		}
	}
}
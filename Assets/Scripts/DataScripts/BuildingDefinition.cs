using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDefinition", menuName = "ScriptableObjects/BuildingDefinition", order = 1)]
public class BuildingDefinition : GameDefinition
{
	public GameBuildingType BuildingType;
	public int BaseBuyActions = 0;

	public override GameDataType DataType => GameDataType.Entity;
	public override GameEntityType EntityType => GameEntityType.Building;
}

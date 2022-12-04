using System.Collections;
using System.Collections.Generic;
using TacticGameData;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDefinition", menuName = "ScriptableObjects/BuildingDefinition", order = 1)]
public class BuildingDefinitionObject : GameDefinitionObject, IBuildingDefinition
{
	[field: SerializeField]
	public GameBuildingType BuildingType { get; set; }

	[field: SerializeField]
	public int BaseBuyActions { get; set; }

	[field: SerializeField]
	public int BaseIncomeValue { get; set; }

	public override GameDataType DataType => GameDataType.Entity;
	public override GameEntityType EntityType => GameEntityType.Building;

	public static List<GameBuildingType> ShopBuildings
	{
		get
		{
			return new List<GameBuildingType> { GameBuildingType.HQ, GameBuildingType.Barracks };
		}
	}

	public BuildingDataObject Instantiate()
	{
		BuildingDataObject newUnit = CreateInstance<BuildingDataObject>();
		newUnit.Definition = GetData();
		return newUnit;
	}

	public static BuildingDataObject Instantiate(BuildingDefinition definition)
	{
		BuildingDataObject newUnit = CreateInstance<BuildingDataObject>();
		newUnit.Definition = definition;
		return newUnit;
	}
	public new BuildingDefinition GetData()
	{
		return new BuildingDefinition();
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameBuildingType
{
	HQ,
	City,
	Barracks
}

[CreateAssetMenu(fileName = "BuildingDefinition", menuName = "ScriptableObjects/BuildingDefinition", order = 1)]
public class BuildingDefinition : GameDefinition
{
	public GameBuildingType BuildingType;
	public int BaseBuyActions = 0;
	public int BaseIncomeValue = 0;

	public override GameDataType DataType => GameDataType.Entity;
	public override GameEntityType EntityType => GameEntityType.Building;

	public static List<GameBuildingType> ShopBuildings
	{
		get
		{
			return new List<GameBuildingType> { GameBuildingType.HQ, GameBuildingType.Barracks };
		}
	}
}
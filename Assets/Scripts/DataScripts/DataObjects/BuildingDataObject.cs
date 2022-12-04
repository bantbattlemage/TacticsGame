using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TacticGameData;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/BuildingData", order = 1)]
public class BuildingDataObject : GameEntityDataObject, IBuildingData
{
	public new BuildingDefinition Definition { get; set; }
	public int RemainingBuyActions { get; set; }
	public int IncomeValue { get { return Definition.BaseIncomeValue; } }

	public override int RemainingActions
	{
		get 
		{
			return RemainingBuyActions; 
		}
	}

	public BuildingDataObject Instantiate()
	{
		BuildingDataObject newBuilding = CreateInstance<BuildingDataObject>();
		newBuilding.Definition = Definition;
		newBuilding.RemainingBuyActions = RemainingBuyActions;

		return newBuilding;
	}

	public virtual new BuildingData GetData()
	{
		BuildingData data = new BuildingData();

		return data;
	}
}
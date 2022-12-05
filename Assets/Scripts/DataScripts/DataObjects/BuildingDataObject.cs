using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TacticGameData;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/BuildingData", order = 1)]
public class BuildingDataObject : GameEntityDataObject, IBuildingData
{
	public new BuildingDefinition Definition
	{
		get
		{
			return _definition;
		}
		set
		{
			base.Definition = value;
			_definition = value;
		}
	}

	private BuildingDefinition _definition;

	[field: SerializeField]
	public int RemainingBuyActions { get; set; }

	public int IncomeValue { get { return Definition.BaseIncomeValue; } }

	public override int RemainingActions
	{
		get 
		{
			return RemainingBuyActions; 
		}
	}

	public BuildingDataObject()
	{
		if(Definition == null && DefinitionObject != null)
		{
			Definition = (DefinitionObject as BuildingDefinitionObject).ToData();
		}
	}

	public BuildingDataObject Instantiate()
	{
		BuildingDataObject data = CreateInstance<BuildingDataObject>();
		data.DefinitionObject = DefinitionObject;
		data.Definition = Definition;
		data.RemainingBuyActions = RemainingBuyActions;
		data.Location = Location;
		data.Owner = Owner;
		data.State = State;

		if (Definition == null && DefinitionObject != null)
		{
			data.Definition = (DefinitionObject as BuildingDefinitionObject).ToData();
		}

		return data;
	}

	public static BuildingDataObject Instantiate(BuildingData buildingData)
	{
		BuildingDataObject data = CreateInstance<BuildingDataObject>();
		data.Definition = buildingData.Definition;
		data.RemainingBuyActions = buildingData.RemainingBuyActions;
		data.Location = buildingData.Location;
		data.Owner = buildingData.Owner;
		data.State = buildingData.State;

		return data;
	}

	public virtual new BuildingData ToData()
	{
		BuildingData data = new BuildingData();

		data.Definition = Definition;
		data.RemainingActions = RemainingBuyActions;
		data.Location = Location;
		data.Location = Location;
		data.Owner = Owner;
		data.State = State;

		return data;
	}
}
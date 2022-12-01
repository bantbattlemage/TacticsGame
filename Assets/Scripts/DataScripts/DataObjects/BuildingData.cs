using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/BuildingData", order = 1)]
public class BuildingData : GameEntityData
{
	public int RemainingBuyActions;
	public int IncomeValue { get { return TypedDefinition.BaseIncomeValue; } }

	public override int RemainingActions
	{
		get 
		{
			return RemainingBuyActions; 
		}
	}

	public BuildingDefinition TypedDefinition
	{
		get
		{
			return Definition as BuildingDefinition;
		}
	}
}
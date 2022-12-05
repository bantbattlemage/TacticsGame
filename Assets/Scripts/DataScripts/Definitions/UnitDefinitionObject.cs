using System.Collections;
using System.Collections.Generic;
using TacticGameData;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitDefinition", menuName = "ScriptableObjects/UnitDefinition", order = 1)]
public class UnitDefinitionObject : GameDefinitionObject, IUnitDefinition
{
	[field: SerializeField]
	public GameUnitType UnitType { get; set; }

	[field: SerializeField]
	public int BaseMovement { get; set; }

	[field: SerializeField]
	public int BaseAttackRange { get; set; }

	[field: SerializeField]
	public int BaseNumberOfAttacks { get; set; }

	[field: SerializeField]
	public int BaseAttackDamage { get; set; }

	[field: SerializeField]
	public int BasePurchaseCost { get; set; }

	public override GameDataType DataType => GameDataType.Entity;
	public override GameEntityType EntityType => GameEntityType.Unit;

	public UnitDataObject Instantiate()
	{
		UnitDataObject newUnit = CreateInstance<UnitDataObject>();
		newUnit.Definition = ToData();
		return newUnit;
	}

	public static UnitDataObject Instantiate(UnitDefinition definition)
	{
		UnitDataObject newUnit = CreateInstance<UnitDataObject>();
		newUnit.Definition = definition;
		return newUnit;
	}

	public new UnitDefinition ToData()
	{
		UnitDefinition unitDefinition = new UnitDefinition();

		unitDefinition.UnitType = UnitType;
		unitDefinition.BaseMovement = BaseMovement;
		unitDefinition.BaseAttackRange = BaseAttackRange;
		unitDefinition.BaseAttackDamage = BaseAttackDamage;
		unitDefinition.BaseNumberOfAttacks = BaseNumberOfAttacks;
		unitDefinition.BasePurchaseCost = BasePurchaseCost;

		return unitDefinition;
	}
}
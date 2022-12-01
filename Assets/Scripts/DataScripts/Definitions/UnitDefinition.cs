using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameUnitType
{
	Commander,
	Regular,
	Artillery
}

[CreateAssetMenu(fileName = "UnitDefinition", menuName = "ScriptableObjects/UnitDefinition", order = 1)]
public class UnitDefinition : GameDefinition
{
	public GameUnitType UnitType;
	public int BaseHealth;
	public int BaseMovement;
	public int BaseAttackRange;
	public int BaseNumberOfAttacks;
	public int BaseAttackDamage;
	public int BasePurchaseCost;

	public override GameDataType DataType => GameDataType.Entity;
	public override GameEntityType EntityType => GameEntityType.Unit;

	public UnitData Instantiate()
	{
		UnitData newUnit = CreateInstance<UnitData>();

		newUnit.Definition = this;

		return newUnit;
	}
}
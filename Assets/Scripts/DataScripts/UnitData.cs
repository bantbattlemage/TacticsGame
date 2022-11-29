using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
	InactivePlayerControlled,
	ActiveAndReady,
	ActiveNoActionsAvailable
}

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/UnitData", order = 1)]
public class UnitData : GameEntityData
{
	public int RemainingHealth = 0;
	public int RemainingMovement = 0;
	public int RemainingAttacks = 0;
	public int BaseAttackRange { get { return TypedDefinition.BaseAttackRange; } }
	public int BaseAttackDamage { get { return TypedDefinition.BaseAttackDamage; } }
	public UnitState State;

	public UnitDefinition TypedDefinition
	{
		get
		{
			return Definition as UnitDefinition;
		}
	}
}
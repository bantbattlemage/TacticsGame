using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/UnitData", order = 1)]
public class UnitData : GameEntityData
{
	public int RemainingHealth = 0;
	public int RemainingMovement = 0;
	public int RemainingAttacks = 0;
	
	public override int RemainingActions { get { return RemainingMovement + RemainingAttacks; } }

	public UnitDefinition TypedDefinition
	{
		get
		{
			return Definition as UnitDefinition;
		}
	}
}
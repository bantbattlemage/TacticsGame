using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/UnitData", order = 1)]
public class UnitData : GameEntityData
{
	public int RemainingMovement;
	public int RemainingAttacks;
	
	public override int RemainingActions { get { return RemainingMovement + RemainingAttacks; } }

	public UnitDefinition TypedDefinition
	{
		get
		{
			return Definition as UnitDefinition;
		}
	}
}
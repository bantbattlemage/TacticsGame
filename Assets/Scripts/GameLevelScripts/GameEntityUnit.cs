using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEntityUnit : GameEntity
{
	public UnitData TypedData { get { return Data as UnitData; } }

	public void SetRemainingHealth(int value)
	{
		if (value < 0)
		{
			value = 0;
		}

		if(value > TypedData.TypedDefinition.BaseHealth)
		{
			value = TypedData.TypedDefinition.BaseHealth;
		}

		TypedData.RemainingHealth = value;

		if (TypedData.RemainingHealth == 0)
		{
			//SetState(UnitState.ActiveNoActionsAvailable);
		}
	}

	public void SetRemainingMovement(int value)
	{
		if (value < 0)
		{
			value = 0;
		}

		TypedData.RemainingMovement = value;

		if (RemainingActions == 0)
		{
			SetState(GameEntityState.ActiveNoActionsAvailable);
		}
	}

	public void SetRemainingAttacks(int value)
	{
		if(value < 0)
		{
			value = 0;
		}

		TypedData.RemainingAttacks = value;

		if(RemainingActions == 0)
		{
			SetState(GameEntityState.ActiveNoActionsAvailable);
		}
	}

	public override void RefreshEntity()
	{
		SetRemainingAttacks(TypedData.TypedDefinition.BaseNumberOfAttacks);
		SetRemainingMovement(TypedData.TypedDefinition.BaseMovement);
		SetState(GameEntityState.ActiveAndReady);
	}
}
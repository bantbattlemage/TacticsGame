using NesScripts.Controls.PathFind;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEntityUnit : GameEntity
{
	public UnitData TypedData { get { return Data as UnitData; } }

	public override void Initialize(GameEntityData data, Point location)
	{
		base.Initialize(data, location);

		SetRemainingHealth((data as UnitData).TypedDefinition.BaseHealth);
	}

	public override void SetState(GameEntityState state)
	{
		base.SetState(state);

		if(state == GameEntityState.ActiveNoActionsAvailable)
		{
			SetRemainingAttacks(0);
			SetRemainingMovement(0);
		}
	}

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

		if (RemainingActions == 0 && State != GameEntityState.ActiveNoActionsAvailable)
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

		if(RemainingActions == 0 && State != GameEntityState.ActiveNoActionsAvailable)
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
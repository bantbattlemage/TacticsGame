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
	}

	public override void SetRemainingHealth(int value)
	{
		base.SetRemainingHealth(value);

		if (TypedData.RemainingHealth <= 0)
		{
			GameMap.Instance.DestroyUnit(TypedData);
		}
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

	public void SetRemainingMovement(int value)
	{
		if (value < 0)
		{
			value = 0;
		}

		TypedData.RemainingMovement = value;
		CheckRemainingActions();
	}

	public void SetRemainingAttacks(int value)
	{
		if(value < 0)
		{
			value = 0;
		}

		TypedData.RemainingAttacks = value;
		CheckRemainingActions();
	}

	public override void RefreshEntity()
	{
		SetRemainingAttacks(TypedData.TypedDefinition.BaseNumberOfAttacks);
		SetRemainingMovement(TypedData.TypedDefinition.BaseMovement);
		SetState(GameEntityState.ActiveAndReady);
	}
}
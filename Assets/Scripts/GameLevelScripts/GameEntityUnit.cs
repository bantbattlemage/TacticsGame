using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEntityUnit : GameEntity
{
	public UnitData TypedData { get { return Data as UnitData; } }

	public UnitState State
	{
		get
		{
			return TypedData.State;
		}
	}

	public void SetState(UnitState state)
	{
		TypedData.State = state;

		if(State == UnitState.ActiveNoActionsAvailable)
		{
			GetComponentsInChildren<Renderer>().ToList().ForEach(x =>
			{
				Color greyedColor = x.material.GetColor("_Color");
				greyedColor.r *= 0.33f;
				greyedColor.g *= 0.33f;
				greyedColor.b *= 0.33f;
				x.material.SetColor("_Color", greyedColor);
			});
		}
		else if(State == UnitState.InactivePlayerControlled)
		{
			SetPlayerColor();
			GetComponentsInChildren<Renderer>().ToList().ForEach(x =>
			{
				Color greyedColor = x.material.GetColor("_Color");
				greyedColor.r *= 0.75f;
				greyedColor.g *= 0.75f;
				greyedColor.b *= 0.75f;
				x.material.SetColor("_Color", greyedColor);
			});
		}
		else
		{
			SetPlayerColor();
		}
	}

	public int CheckRemainingActions()
	{
		int totalRemainingActions = 0;

		totalRemainingActions += TypedData.RemainingAttacks;
		totalRemainingActions += TypedData.RemainingMovement;

		if(totalRemainingActions <= 0)
		{
			SetState(UnitState.ActiveNoActionsAvailable);
		}

		return totalRemainingActions;
	}

	public override void RefreshEntity()
	{
		TypedData.RemainingAttacks = TypedData.TypedDefinition.BaseNumberOfAttacks;
		TypedData.RemainingMovement = TypedData.TypedDefinition.BaseMovement;
		SetState(UnitState.ActiveAndReady);
	}
}
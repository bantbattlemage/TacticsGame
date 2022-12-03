using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityBuilding : GameEntity
{
	public BuildingData TypedData { get { return Data as BuildingData; } }

	public override int RemainingActions
	{
		get { return TypedData.RemainingBuyActions; }
	}

	public override void SetRemainingHealth(int value)
	{
		base.SetRemainingHealth(value);
	}

	public void SetRemainingBuyActions(int value)
	{
		if(value < 0)
		{
			value = 0;
		}

		TypedData.RemainingBuyActions = value;
		CheckRemainingActions();
	}

	public override void RefreshEntity()
	{
		SetRemainingBuyActions(TypedData.TypedDefinition.BaseBuyActions);
		SetState(GameEntityState.ActiveAndReady);
	}
}
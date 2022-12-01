using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntityBuilding : GameEntity
{
	public BuildingData TypedData { get { return Data as BuildingData; } }

	public override void SetRemainingHealth(int value)
	{
		base.SetRemainingHealth(value);
	}

	public void SetRemainingBuyActions(int remainingActions)
	{
		if(remainingActions < 0)
		{
			remainingActions = 0;
		}

		if(remainingActions > TypedData.TypedDefinition.BaseBuyActions)
		{
			remainingActions = TypedData.TypedDefinition.BaseBuyActions;
		}

		TypedData.RemainingBuyActions= remainingActions;
	}

	public override void RefreshEntity()
	{
		SetRemainingBuyActions(TypedData.TypedDefinition.BaseBuyActions);
		SetState(GameEntityState.ActiveAndReady);
	}
}
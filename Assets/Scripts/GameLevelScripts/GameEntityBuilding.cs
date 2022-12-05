using System.Collections;
using System.Collections.Generic;
using TacticGameData;
using UnityEngine;

public class GameEntityBuilding : GameEntity
{
	public new BuildingData Data
	{
		get
		{
			return _data;
		}
		set
		{
			base.Data = new GameEntityData<GameDefinition>();
			base.Data.Definition = value.Definition;
			base.Data.Owner = value.Owner;
			base.Data.Location = value.Location;
			base.Data.RemainingHealth = value.RemainingHealth;
			base.Data.RemainingActions = value.RemainingActions;

			_data = value;
		}
	}

	private BuildingData _data;

	public override int RemainingActions
	{	
		get { return Data.RemainingBuyActions; }
	}

	public void Initialize(BuildingData data, Point location)
	{
		Data = data;
		base.Initialize(location);
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

		Data.RemainingBuyActions = value;
		CheckRemainingActions();
	}

	public override void RefreshEntity()
	{
		SetRemainingBuyActions(Data.Definition.BaseBuyActions);
		SetState(GameEntityState.ActiveAndReady);
	}
}
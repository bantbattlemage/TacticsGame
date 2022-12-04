using TacticGameData;

public class GameEntityUnit : GameEntity
{
	public new UnitData Data { get; set; }

	public void Initialize(UnitData data, Point location)
	{
		Data = data;
		base.Initialize(location);
	}

	public override void SetRemainingHealth(int value)
	{
		base.SetRemainingHealth(value);

		if (Data.RemainingHealth <= 0)
		{
			GameMap.Instance.DestroyUnit(Data);
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

		Data.RemainingMovement = value;
		CheckRemainingActions();
	}

	public void SetRemainingAttacks(int value)
	{
		if(value < 0)
		{
			value = 0;
		}

		Data.RemainingAttacks = value;
		CheckRemainingActions();
	}

	public override void RefreshEntity()
	{
		SetRemainingAttacks(Data.Definition.BaseNumberOfAttacks);
		SetRemainingMovement(Data.Definition.BaseMovement);
		SetState(GameEntityState.ActiveAndReady);
	}
}
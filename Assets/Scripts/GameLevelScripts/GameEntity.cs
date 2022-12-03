using NesScripts.Controls.PathFind;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
	public GameEntityData Data;
	public virtual int RemainingActions { get { return Data.RemainingActions; } }

	public GameEntityState State
	{
		get
		{
			return Data.State;
		}
	}

	public virtual void Initialize(GameEntityData data, Point location)
	{
		Data = data;
		Data.Location = location;
		SetPlayerColor();
		SetRemainingHealth(data.Definition.BaseHealth);
		CheckRemainingActions();
	}

	public virtual void SetOwner(int playerID)
	{
		Data.Owner= playerID;
		SetPlayerColor();
	}

	public virtual void SetRemainingHealth(int value)
	{
		if (value < 0)
		{
			value = 0;
		}

		if (value > Data.Definition.BaseHealth)
		{
			value = Data.Definition.BaseHealth;
		}

		Data.RemainingHealth = value;
	}

	public virtual void SetState(GameEntityState state)
	{
		Data.State = state;

		if (State == GameEntityState.ActiveNoActionsAvailable)
		{
			SetPlayerColor();
		}
		else if (State == GameEntityState.InactivePlayerControlled)
		{
			SetPlayerColor();
		}
		else
		{
			SetPlayerColor();
		}
	}

	public virtual void SetPlayerColor()
	{
		Material material = null;

		switch (Data.Owner)
		{
			case -1:
				material = Resources.Load<Material>("Materials/NeutralMaterial");
				break;
			case 0:
				material = Resources.Load<Material>("Materials/Player1Material");
				break;
			case 1:
				material = Resources.Load<Material>("Materials/Player2Material");
				break;
			case 2:
				material = Resources.Load<Material>("Materials/Player3Material");
				break;
			case 3:
				material = Resources.Load<Material>("Materials/Player4Material");
				break;
		}

		GetComponentsInChildren<Renderer>().ToList().ForEach(x =>
		{
			x.material = material;
		});

		if (State == GameEntityState.ActiveNoActionsAvailable)
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
		else if (State == GameEntityState.InactivePlayerControlled)
		{
			GetComponentsInChildren<Renderer>().ToList().ForEach(x =>
			{
				Color greyedColor = x.material.GetColor("_Color");
				greyedColor.r *= 0.75f;
				greyedColor.g *= 0.75f;
				greyedColor.b *= 0.75f;
				x.material.SetColor("_Color", greyedColor);
			});
		}
	}

	public virtual void CheckRemainingActions()
	{
		if (RemainingActions == 0 && Data.Owner != -1)
		{
			if (State != GameEntityState.ActiveNoActionsAvailable)
			{
				SetState(GameEntityState.ActiveNoActionsAvailable);
			}
			else
			{
				SetPlayerColor();
			}
		}
	}

	/// <summary>
	/// Reset any entity values that are to be reset at the beginning of a player's turn.
	/// </summary>
	public virtual void RefreshEntity()
	{
		SetRemainingHealth(Data.Definition.BaseHealth);
		SetState(GameEntityState.ActiveAndReady);
	}
}
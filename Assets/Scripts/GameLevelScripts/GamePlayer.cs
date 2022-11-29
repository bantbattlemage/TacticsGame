using NesScripts.Controls.PathFind;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class GamePlayer : MonoBehaviour
{
	public static int NEUTRAL_PLAYER_ID { get { return -1; } }

	public PlayerData GamePlayerData;
	public GameCamera PlayerCamera;
	public PlayerUI PlayerInterface;

	public GamePlayerState State { get { return GamePlayerData.State; } }
	public bool IsUsingUnitAction { get { return State == GamePlayerState.UnitMoveAction || State == GamePlayerState.UnitAttackAction; } }

	private int _cachedMaxMoveDistance = 0;
	private UnitData _activeSelectedUnit = null;
	private List<GameTile> _activeMoveTiles = new List<GameTile>();
	private List<Point> _cachedPath = new List<Point>();

	public delegate void RequestEndTurnEvent(GamePlayer sendingPlayer);
	public RequestEndTurnEvent RequestEndTurn;

	public void Initialize(PlayerData data, GameMap map)
	{
		GamePlayerData = data;
		PlayerCamera.Initialize(map);
		PlayerCamera.GetComponent<Camera>().depth = data.ID;
		PlayerInterface.Initialize(this);
		PlayerInterface.EndTurnButtonPressed += EndTurn;
		InitializePlayerEntities();
		SetState(GamePlayerState.Idle_InactivePlayer);
	}

	private void Update()
	{
		//  right click cancel
		if (IsUsingUnitAction && Input.GetMouseButtonDown(1))
		{
			CancelUnitCommand();
		}
	}

	public void SetState(GamePlayerState state)
	{
		GamePlayerData.State = state;

		PlayerInterface.EndTurnButton.gameObject.SetActive(state == GamePlayerState.Idle_ActivePlayer);
		if(state == GamePlayerState.Idle_ActivePlayer)
		{
			PlayerInterface.SetLock(false);
		}
		else if(state == GamePlayerState.Idle_InactivePlayer)
		{
			List<GameEntity> entities = GameController.Instance.CurrentGameMatch.Map.GetAllPlayerEntities(GamePlayerData.ID);

			foreach(GameEntity entity in entities)
			{
				switch (entity.Data.Definition.EntityType)
				{
					case GameEntityType.Unit:
						(entity as GameEntityUnit).SetState(UnitState.InactivePlayerControlled);
						break;
					case GameEntityType.Building:
						break;
				}
			}
		}
	}

	#region Player Turn Actions
	/// <summary>
	/// Set all player entities properties to default values.
	/// </summary>
	public void InitializePlayerEntities()
	{
		List<GameEntity> entities = GameController.Instance.CurrentGameMatch.Map.GetAllPlayerEntities(GamePlayerData.ID);

		foreach (GameEntity entity in entities)
		{
			switch (entity.Data.Definition.EntityType)
			{
				case GameEntityType.Unit:
					UnitData unitData = entity.Data as UnitData;
					unitData.RemainingHealth = unitData.TypedDefinition.BaseHealth;
					break;
				case GameEntityType.Building:
					BuildingData buildingData = entity.Data as BuildingData;
					break;
			}

			entity.RefreshEntity();
		}
	}

	/// <summary>
	/// Begins the players turn.
	/// </summary>
	public void BeginTurn(int roundNumber)
	{
		CancelUnitCommand();

		PlayerInterface.UpdateDisplayInfo(roundNumber);

		RefreshAllPlayerEntities();

		GameTile hqTile = GameController.Instance.CurrentGameMatch.Map.GetPlayerHQ(GamePlayerData.ID);
		if (hqTile != null)
		{
			PlayerCamera.PanTo(hqTile.transform);
		}

		SetState(GamePlayerState.Idle_ActivePlayer);
	}

	/// <summary>
	/// Ends the players turn.
	/// </summary>
	private void EndTurn()
	{
		CancelUnitCommand();
		SetState(GamePlayerState.Idle_InactivePlayer);
		RequestEndTurn(this);
	}

	/// <summary>
	/// Refresh all turn actions. (Called on turn start)
	/// </summary>
	public void RefreshAllPlayerEntities()
	{
		List<GameEntity> entities = GameController.Instance.CurrentGameMatch.Map.GetAllPlayerEntities(GamePlayerData.ID);

		foreach (GameEntity entity in entities)
		{
			entity.RefreshEntity();
		}
	}
	#endregion

	#region Unit Move Action
	/// <summary>
	/// Enter a Player Move Unit Action state
	/// </summary>
	public void BeginUnitMove(UnitData unit)
	{
		if (State != GamePlayerState.Idle_ActivePlayer)
		{
			return;
		}

		List<GameTile> tiles = new List<GameTile>();
		int range = unit.RemainingMovement;
		_cachedMaxMoveDistance = range;

		if (range <= 0)
		{
			return;
		}

		for (int x = -range; x <= range; x++)
		{
			for (int y = -range; y <= range; y++)
			{
				Point target = new Point(unit.Location.x + x, unit.Location.y + y);

				//  ignore tile already on
				if (target.x == unit.Location.x && target.y == unit.Location.y)
				{
					continue;
				}

				GameTile tile = GameController.Instance.CurrentGameMatch.Map.GetTile(target.x, target.y);

				bool validate = true;

				if (tile != null)
				{
					//  no walking on water
					if (tile.TileData.Type == TerrainType.Water)
					{
						validate = false;
					}

					if (tile.TileData.Entities != null && tile.TileData.Entities.Length > 0)
					{
						List<GameEntityData> entities = tile.TileData.Entities.ToList();
						if(entities.Count == 1 && entities[0].Definition.EntityType == GameEntityType.Building)
						{
							validate = true;
						}
						else
						{
							validate = false;
						}
					}
				}
				else
				{
					validate = false;
				}

				if (validate)
				{
					tiles.Add(tile);
				}
			}
		}

		List<GameTile> filteredTiles = GameMap.FilterTilesByDistance(tiles, unit.Location, unit.RemainingMovement);

		if (filteredTiles.Count == 0)
		{
			return;
		}

		foreach (GameTile tile in filteredTiles)
		{
			tile.EnableHilightForMovement(OnGameTileUnitMoveMouseEnterAction, OnGameTileUnitMoveClickAction);
		}

		_activeSelectedUnit = unit;
		_activeMoveTiles = filteredTiles;
		SetState(GamePlayerState.UnitMoveAction);
		PlayerInterface.SetLock(true);
		PlayerInterface.EnableTargetTooltip();
	}

	/// <summary>
	/// Clears the Move Unit State related variables. fullReset=true will exit the Move Unit state. false to just clear variables 
	/// </summary>
	private void CancelUnitCommand(bool fullReset = true)
	{
		if (!IsUsingUnitAction)
		{
			return;
		}

		if (_activeMoveTiles != null)
		{
			foreach (GameTile tile in _activeMoveTiles)
			{
				tile.DisableHilightForMovement();
			}
		}

		_activeSelectedUnit = null;
		_activeMoveTiles = null;

		if (fullReset)
		{
			SetState(GamePlayerState.Idle_ActivePlayer);
			PlayerInterface.EnableTargetTooltip(false);
		}
	}

	/// <summary>
	/// Called when mouse enters a tile that is active in a move command
	/// </summary>
	private void OnGameTileUnitMoveMouseEnterAction(GameTile sender)
	{
		List<GameTile> tiles = new List<GameTile>();

		Point from = new Point(_activeSelectedUnit.Location.x, _activeSelectedUnit.Location.y);
		Point to = new Point(sender.TileData.X, sender.TileData.Y);

		_cachedPath = GameController.Instance.CurrentGameMatch.Map.FindPath(from, to, _activeMoveTiles);

		foreach (Point p in _cachedPath)
		{
			GameTile t = GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y);

			if (t != null)
			{
				tiles.Add(t);
			}
		}

		if (_cachedPath.Count > _cachedMaxMoveDistance)
		{
			_cachedPath = new List<Point>();
			tiles = new List<GameTile>();
		}

		foreach (GameTile tile in _activeMoveTiles)
		{
			if (!tiles.Contains(tile))
			{
				tile.HilightRed();
			}
			else
			{
				tile.HilightGreen();
			}
		}
	}

	/// <summary>
	/// Confirms a unit movement command
	/// </summary>
	private void OnGameTileUnitMoveClickAction(GameTile sender)
	{
		if (_cachedPath != null && _cachedPath.Count > 0)
		{
			PlayerInterface.SetLock(true);

			foreach (Point p in _cachedPath)
			{
				GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).LockTile();
			}

			UnitData newUnitReference = _activeSelectedUnit;
			List<Point> newPoints = new List<Point>(_cachedPath);

			GameEntityData[] entities = sender.TileData.Entities;

			//	moving into a tile with a capturable building
			if (entities != null && entities.Length == 1 && entities[0].Definition.EntityType == GameEntityType.Building && entities[0].Owner != GamePlayerData.ID)
			{
				UnityAction[] buttonActions = new UnityAction[3];
				string[] buttonLabels = new string[] { "Capture", "Move", "Cancel" };
				int cancelIndex = 2;

				//	capture
				buttonActions[0] = () => 
				{
					ExecuteUnitMoveAction(newUnitReference, newPoints);
					ExecuteUnitBuildingCaptureAction(newUnitReference, entities[0] as BuildingData);

					SetState(GamePlayerState.Idle_ActivePlayer);
					PlayerInterface.EnableTargetTooltip(false);
					PlayerInterface.ConfirmBox.Disable();
				};
				//	move
				buttonActions[1] = () =>
				{
					ExecuteUnitMoveAction(newUnitReference, newPoints);
					SetState(GamePlayerState.Idle_ActivePlayer);
					PlayerInterface.EnableTargetTooltip(false);
					PlayerInterface.ConfirmBox.Disable();
				};
				//	cancel
				buttonActions[2] = () =>
				{
					SetState(GamePlayerState.Idle_ActivePlayer);
					PlayerInterface.EnableTargetTooltip(false);
					PlayerInterface.ConfirmBox.Disable();
					foreach (Point p in newPoints)
					{
						GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).UnlockTile();
					}
				};

				PlayerInterface.ConfirmBox.EnableBox(buttonActions, buttonLabels, cancelIndex);
			}
			//	normal movement
			else
			{
				PlayerInterface.ConfirmBox.EnableConfirmationBox(
				//	confirm
				() =>
				{
					ExecuteUnitMoveAction(newUnitReference, newPoints);
					SetState(GamePlayerState.Idle_ActivePlayer);
					PlayerInterface.EnableTargetTooltip(false);
				},
				//	cancel
				() =>
				{
					SetState(GamePlayerState.Idle_ActivePlayer);
					PlayerInterface.EnableTargetTooltip(false);
					foreach (Point p in newPoints)
					{
						GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).UnlockTile();
					}
				});
			}
		}

		CancelUnitCommand(false);
	}

	/// <summary>
	/// Move the given unit along the target path.
	/// </summary>
	private void ExecuteUnitMoveAction(UnitData unit, List<Point> points)
	{
		if(points.Count > unit.RemainingMovement)
		{
			throw new Exception(string.Format("attempted to move unit {0} greater than its remaining movement", unit.name));
		}

		unit.RemainingMovement -= points.Count;
		GameController.Instance.CurrentGameMatch.Map.MoveEntity(unit, points);
		(GameController.Instance.CurrentGameMatch.Map.GetEntity(unit) as GameEntityUnit).CheckRemainingActions();

		foreach (Point p in points)
		{
			GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).UnlockTile();
		}
	}

	/// <summary>
	/// Capture the target building using the given unit.
	/// </summary>
	private void ExecuteUnitBuildingCaptureAction(UnitData unit, BuildingData building)
	{
		//	capture neutral building
		if (building.Owner == NEUTRAL_PLAYER_ID)
		{
			building.Owner = GamePlayerData.ID;
			GameController.Instance.CurrentGameMatch.Map.GetEntity(building).SetPlayerColor();
		}
		//	set other player's building to neutral
		else
		{
			building.Owner = NEUTRAL_PLAYER_ID;
			GameController.Instance.CurrentGameMatch.Map.GetEntity(building).SetPlayerColor();
		}
	}
	#endregion

	#region Unit Attack Action
	/// <summary>
	/// Enter a Player Unit Attack Atack state
	/// </summary>
	public void BeginUnitAttack(UnitData unit)
	{
		if (State != GamePlayerState.Idle_ActivePlayer || unit.RemainingAttacks <= 0)
		{
			return;
		}

		List<GameTile> tiles = new List<GameTile>();
		int range = unit.BaseAttackRange;
		_cachedMaxMoveDistance = range;

		if (range <= 0)
		{
			return;
		}

		for (int x = -range; x <= range; x++)
		{
			for (int y = -range; y <= range; y++)
			{
				Point target = new Point(unit.Location.x + x, unit.Location.y + y);

				//  ignore tile already on
				if (target.x == unit.Location.x && target.y == unit.Location.y)
				{
					continue;
				}

				GameTile tile = GameController.Instance.CurrentGameMatch.Map.GetTile(target.x, target.y);

				if (tile != null)
				{
					tiles.Add(tile);
				}
			}
		}

		List<GameTile> filteredTiles = GameMap.FilterTilesByDistance(tiles, unit.Location, range);

		if (filteredTiles.Count == 0)
		{
			return;
		}

		foreach (GameTile tile in filteredTiles)
		{
			tile.EnableHilightForMovement(OnGameTileUnitAttackMouseEnterAction, OnGameTileUnitAttackClickAction);
		}

		_activeSelectedUnit = unit;
		_activeMoveTiles = filteredTiles;
		SetState(GamePlayerState.UnitAttackAction);
		PlayerInterface.SetLock(true);
		PlayerInterface.EnableTargetTooltip();
	}

	/// <summary>
	/// Called when mouse enters a tile that is active in an attack command
	/// </summary>
	private void OnGameTileUnitAttackMouseEnterAction(GameTile sender)
	{
		List<GameTile> tiles = new List<GameTile>();

		Point from = new Point(_activeSelectedUnit.Location.x, _activeSelectedUnit.Location.y);
		Point to = new Point(sender.TileData.X, sender.TileData.Y);

		_cachedPath = GameController.Instance.CurrentGameMatch.Map.FindPath(from, to, _activeMoveTiles);

		foreach (Point p in _cachedPath)
		{
			GameTile tile = GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y);

			if (tile != null)
			{
				bool validate = false;

				if (tile != null)
				{
					if (tile.TileData.Entities != null && tile.TileData.Entities.Length > 0)
					{
						if (tile.TileData.Entities.Any(x => x.Owner != GamePlayerData.ID && x.Definition.EntityType == GameEntityType.Unit))
						{
							validate = true;
						}
					}
				}

				if (validate)
				{
					tiles.Add(tile);
				}
			}
		}

		if (_cachedPath.Count > _cachedMaxMoveDistance)
		{
			_cachedPath = new List<Point>();
			tiles = new List<GameTile>();
		}

		foreach (GameTile tile in _activeMoveTiles)
		{
			if (!tiles.Contains(tile))
			{
				tile.HilightRed();
			}
			else
			{
				tile.HilightGreen();
			}
		}
	}

	/// <summary>
	/// Confirms a unit attack command
	/// </summary>
	private void OnGameTileUnitAttackClickAction(GameTile sender)
	{
		if (_cachedPath != null && _cachedPath.Count > 0)
		{
			UnitData target = null;

			if (sender.TileData.Entities.Length > 0)
			{
				try
				{
					target = sender.TileData.Entities.First(x => x.Owner != GamePlayerData.ID && x.Definition.EntityType == GameEntityType.Unit) as UnitData;
				}
				catch (Exception ex)
				{
					Debug.LogWarning(ex);
					return;
				}
			}

			if (target != null)
			{
				GameController.Instance.CurrentGameMatch.Map.GetTile(_cachedPath.Last().x, _cachedPath.Last().y).LockTile();
				UnitData newUnitReference = _activeSelectedUnit;
				List<Point> newPoints = new List<Point>() { _cachedPath.Last() };

				PlayerInterface.TargetTooltip.UpdateTooltip(target);
				PlayerInterface.SetLock(true);

				PlayerInterface.ConfirmBox.EnableConfirmationBox(() =>
				{
					ExecuteUnitAttackAction(newUnitReference, target, newPoints);
					PlayerInterface.EnableTargetTooltip(false);
					SetState(GamePlayerState.Idle_ActivePlayer);
				},
				() =>
				{
					PlayerInterface.EnableTargetTooltip(false);
					SetState(GamePlayerState.Idle_ActivePlayer);

					foreach (Point p in newPoints)
					{
						GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).UnlockTile();
					}
				});

				CancelUnitCommand(false);
			}
			else
			{
				CancelUnitCommand();
			}
		}
		else
		{
			CancelUnitCommand();
		}
	}

	/// <summary>
	/// Execute a unit attack command
	/// </summary>
	private void ExecuteUnitAttackAction(UnitData unit, UnitData target, List<Point> points)
	{
		if(unit.RemainingAttacks <= 0)
		{
			throw new Exception(string.Format("attempted attack with unit {0} but it has no attacks left", unit.name));
		}

		unit.RemainingAttacks--;
		target.RemainingHealth -= unit.BaseAttackDamage;
		if(target.RemainingHealth <= 0)
		{
			target.RemainingHealth = 0;
		}

		(GameController.Instance.CurrentGameMatch.Map.GetEntity(unit) as GameEntityUnit).CheckRemainingActions();

		foreach (Point p in points)
		{
			GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).UnlockTile();
		}
	}
	#endregion
}
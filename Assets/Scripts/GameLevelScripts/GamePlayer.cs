using NesScripts.Controls.PathFind;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    public PlayerData GamePlayerData;
    public GameCamera PlayerCamera;
    public PlayerUI PlayerInterface;

    private bool _isMovingUnit = false;
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
    }

    private void Update()
    {
        //  right click cancel
        if(_isMovingUnit && Input.GetMouseButtonDown(1))
        {
            CancelMoveUnit();
        }
    }

    public void BeginTurn(int roundNumber)
    {
        CancelMoveUnit();

        PlayerInterface.UpdateDisplayInfo(roundNumber);

        RefreshAllPlayerEntities();

        GameTile hqTile = GameController.Instance.CurrentGameMatch.Map.GetPlayerHQ(GamePlayerData.ID);
        if(hqTile != null)
        {
            PlayerCamera.PanTo(hqTile.transform);
        }
    }

    /// <summary>
    /// Refresh all turn actions. (Called on turn start)
    /// </summary>
    public void RefreshAllPlayerEntities()
    {
        List<GameEntity> entities = GameController.Instance.CurrentGameMatch.Map.GetAllPlayerEntities(GamePlayerData.ID);

        foreach (GameEntity entity in entities)
        {
            switch (entity.Data.Definition.EntityType)
            {
                case GameEntityType.Unit:
                    UnitData unitData = entity.Data as UnitData;
                    UnitDefinition unitDefinition = unitData.Definition as UnitDefinition;
                    unitData.RemainingMovement = unitDefinition.BaseMovement;
                    unitData.RemainingAttacks = unitDefinition.BaseNumberOfAttacks;
                    break;
                case GameEntityType.Building:
                    break;
            }
        }
    }

    public void BeginMoveUnit(UnitData unit)
    {
        if (_isMovingUnit)
        {
            return;
        }

        List<GameTile> tiles = new List<GameTile>();
        int range = unit.RemainingMovement;
        _cachedMaxMoveDistance = range;

        if(range <= 0)
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
                    if(tile.TileData.Type == TerrainType.Water)
                    {
                        validate = false;
                    }

                    //  cannot move into tiles occupied by anything else
                    if (tile.TileData.Entities != null && tile.TileData.Entities.Length > 0)
                    {
                        validate = false;
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
            tile.EnableHilightForMovement(OnGameTileMoveUnitEnterAction, OnGameTileMoveClickAction);
        }

        _activeSelectedUnit = unit;
        _activeMoveTiles = filteredTiles;
        _isMovingUnit = true;
        PlayerInterface.ToggleLock();
    }

    private void CancelMoveUnit()
    {
        if(!_isMovingUnit)
        {
            return;
        }

        foreach (GameTile tile in _activeMoveTiles)
        {
            tile.DisableHilightForMovement();
        }

        _activeSelectedUnit = null;
        _activeMoveTiles = null;
        _isMovingUnit = false;

        PlayerInterface.SetLock(false);
    }

    /// <summary>
    /// Called when mouse enters a tile that is active in a move command
    /// </summary>
    private void OnGameTileMoveUnitEnterAction(GameTile sender)
    {
        List<GameTile> tiles = new List<GameTile>();

        Point from = new Point(_activeSelectedUnit.Location.x, _activeSelectedUnit.Location.y);
        Point to = new Point(sender.TileData.X, sender.TileData.Y);
      
        _cachedPath = GameController.Instance.CurrentGameMatch.Map.FindPath(from, to, _activeMoveTiles);

        foreach (Point p in _cachedPath)
        {
            GameTile t = GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y);

            if(t != null)
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
    /// Confirms a unit movement command.
    /// </summary>
    private void OnGameTileMoveClickAction(GameTile sender)
    {
        if(_cachedPath != null && _cachedPath.Count > 0)
        {
            PlayerInterface.SetLock(true);

            foreach(Point p in _cachedPath)
            {
                GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).LockTile();
            }

            UnitData newUnitReference = _activeSelectedUnit;
            List<Point> newPoints = new List<Point>(_cachedPath);

            PlayerInterface.ConfirmBox.EnableConfirmationBox(() => 
            {
                newUnitReference.RemainingMovement -= newPoints.Count;
                GameController.Instance.CurrentGameMatch.Map.MoveEntity(newUnitReference, newPoints);

                foreach (Point p in newPoints)
                {
                    GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).UnlockTile();
                }
            }, 
            () => 
            {
                foreach (Point p in newPoints)
                {
                    GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).UnlockTile();
                }
            });
        }

        CancelMoveUnit();
    }

    public void BeginUnitAttack(UnitData unit)
    {
        if(_isMovingUnit)
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

                bool validate = false;

                if (tile != null)
                {
                    validate = true;
                }

                if (validate)
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
            tile.EnableHilightForMovement(OnGameTileUnitAttackEnterAction, OnGameTileUnitAttackClickAction);
        }

        _activeSelectedUnit = unit;
        _activeMoveTiles = filteredTiles;
        _isMovingUnit = true;
        PlayerInterface.ToggleLock();
    }

    private void OnGameTileUnitAttackEnterAction(GameTile sender)
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
                        if (tile.TileData.Entities.Any(x => x.Owner != GamePlayerData.ID))
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

    private void OnGameTileUnitAttackClickAction(GameTile sender)
    {
        if (_cachedPath != null && _cachedPath.Count > 0)
        {
            UnitData target = null;

            if(sender.TileData.Entities.Length > 0)
            {
                target = sender.TileData.Entities.First(x => x.Owner != GamePlayerData.ID) as UnitData;
            }

            if (target != null)
            {
                PlayerInterface.SetLock(true);

                foreach (Point p in _cachedPath)
                {
                    GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).LockTile();
                }

                UnitData newUnitReference = _activeSelectedUnit;
                List<Point> newPoints = new List<Point>(_cachedPath);

                PlayerInterface.ConfirmBox.EnableConfirmationBox(() =>
                {
                    newUnitReference.RemainingAttacks--;
                    target.RemainingHealth--;

                    foreach (Point p in newPoints)
                    {
                        GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).UnlockTile();
                    }
                },
                () =>
                {
                    foreach (Point p in newPoints)
                    {
                        GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y).UnlockTile();
                    }
                });
            }
        }

        CancelMoveUnit();
    }

    private void EndTurn()
    {
        CancelMoveUnit();
        RequestEndTurn(this);
    }
}
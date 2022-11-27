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
    private UnitData _activeMoveUnit = null;
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
        if(_isMovingUnit && Input.GetMouseButtonDown(1))
        {
            CancelMoveUnit();
        }
    }

    public void BeginTurn(int roundNumber)
    {
        PlayerInterface.UpdateDisplayInfo(roundNumber);

        GameTile hqTile = GameController.Instance.CurrentGameMatch.Map.GetPlayerHQ(GamePlayerData.ID);
        if(hqTile != null)
        {
            PlayerCamera.PanTo(hqTile.transform);
        }
    }

    public void BeginMoveUnit(UnitData unit)
    {
        PlayerInterface.ToggleLock();

        List<GameTile> tiles = new List<GameTile>();
        int range = 3;

        for(int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector2 target = new Vector2(unit.Location.x + x, unit.Location.y + y);

                if (target.x == unit.Location.x && target.y == unit.Location.y)
                {
                    continue;
                }

                GameTile tile = GameController.Instance.CurrentGameMatch.Map.GetTile((int)target.x, (int)target.y);

                bool validate = true;

                //  cannot walk on water
                if (tile != null && tile.TileData.Type != TerrainType.Water)
                {
                    //  cannot move into tiles occupied by anything else
                    if (tile.TileData.Entities != null && tile.TileData.Entities.Length > 0)
                    {
                        validate = false;
                    }
                }
                else
                {
                    validate= false;
                }

                if (validate)
                {
                    tiles.Add(tile);
                }
            }
        }

        foreach(GameTile tile in tiles)
        {
            tile.EnableHilightForMovement(OnGameTileMoveUnitEnterAction, OnGameTileMoveClickAction);
        }

        _activeMoveUnit = unit;
        _activeMoveTiles = tiles;
        _isMovingUnit = true;
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

        _activeMoveUnit = null;
        _activeMoveTiles = null;
        _isMovingUnit = false;

        PlayerInterface.ToggleLock();
    }

    private void OnGameTileMoveUnitEnterAction(GameTile sender)
    {
        List<GameTile> tiles = new List<GameTile>();

        bool[,] tilesMap = new bool[(int)GameController.Instance.CurrentGameMatch.Map.MapSize.x, (int)GameController.Instance.CurrentGameMatch.Map.MapSize.y];
        foreach (GameTile tile in _activeMoveTiles)
        {
            tilesMap[tile.TileData.X, tile.TileData.Y] = true;
        }

        NesScripts.Controls.PathFind.Grid grid = new NesScripts.Controls.PathFind.Grid(tilesMap);
        Point from = new Point((int)_activeMoveUnit.Location.x, (int)_activeMoveUnit.Location.y);
        Point to = new Point(sender.TileData.X, sender.TileData.Y); 
        List<Point> path = Pathfinding.FindPath(grid, from, to, Pathfinding.DistanceType.Manhattan);
        _cachedPath = path;

        foreach(Point p in path)
        {
            GameTile t = GameController.Instance.CurrentGameMatch.Map.GetTile(p.x, p.y);

            if(t != null)
            {
                tiles.Add(t);
            }
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

    private void OnGameTileMoveClickAction(GameTile sender)
    {
        GameController.Instance.CurrentGameMatch.Map.MoveEntity(_activeMoveUnit, _cachedPath);
        CancelMoveUnit();
    }

    private void EndTurn()
    {
        CancelMoveUnit();
        RequestEndTurn(this);
    }
}
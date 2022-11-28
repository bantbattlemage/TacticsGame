using NesScripts.Controls.PathFind;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    public MapData mapData;

    private GameTile[,] activeTiles;

    public static int TileSize = 10;

    public Vector2 MapSize
    {
        get
        {
            return new Vector2(activeTiles.GetLength(0), activeTiles.GetLength(1));
        }
    }

    public GameTile GetTile(int x, int y)
    {
        if (x >= 0 && x < activeTiles.GetLength(0) && y >= 0 && y < activeTiles.GetLength(1))
        {
            return activeTiles[x, y];
        }
        else
        {
            return null;
        }
    }

    public GameTile[,] GameTiles
    {
        get
        {
            return activeTiles;
        }
    }

    public List<GameEntity> GetAllPlayerEntities(int playerId)
    {
        List<GameEntity> gameEntities = new List<GameEntity>();

        foreach(GameTile tile in activeTiles)
        {
            foreach(GameEntity e in tile.SpawnedEntities)
            {
                if(e.Data.Owner == playerId)
                {
                    gameEntities.Add(e);
                }
            }
        }

        return gameEntities;
    }

    public GameTile GetPlayerHQ(int playerId)
    {
        for(int x = 0; x < activeTiles.GetLength(0); x++)
        {
            for (int y = 0; y < activeTiles.GetLength(1); y++)
            {
                TileData tile = activeTiles[x, y].TileData;

                if (tile.Entities != null && tile.Entities.Length > 0)
                {
                    try
                    {
                        foreach (BuildingData buildingData in tile.Entities)
                        {
                            if (buildingData.Owner == playerId && ((BuildingDefinition)buildingData.Definition).BuildingType == GameBuildingType.HQ)
                            {
                                return activeTiles[x, y];
                            }
                        }
                    }
                    catch
                    {
                        //  fail silently on non-HQ entities
                    }
                }
            }
        }

        return null;
    }

    public void MoveEntity(GameEntityData entity, List<Point> path)
    {
        GameTile startTile = activeTiles[(int)entity.Location.x, (int)entity.Location.y];
        GameTile finalTile = activeTiles[path.Last().x, path.Last().y];

        startTile.RemoveEntity(entity);
        finalTile.AddEntity(entity);
        finalTile.SpawnEntity(entity);
    }

    public void LoadMap(string mapToLoadPath = "Data/MapData/New/", string mapName = "New")
    {
        string fullMapPathOriginal = mapToLoadPath + mapName;
        MapData activeMapData = Instantiate(Resources.Load<MapData>(fullMapPathOriginal));
        mapData = activeMapData;

        List<PlayerData> activePlayers = new List<PlayerData>();
        foreach (PlayerData playerData in activeMapData.MapPlayers)
        {
            string originalPath = mapToLoadPath + "PlayerData/" + playerData.name;
            PlayerData activePlayerData = Instantiate(Resources.Load<PlayerData>(originalPath));
            activePlayers.Add(activePlayerData);
        }
        activeMapData.MapPlayers = activePlayers.ToArray();

        int xLength = activeMapData.MapTiles.Max(x => x.X) + 1;
        int yLength = activeMapData.MapTiles.Max(x => x.Y) + 1;
        activeTiles = new GameTile[xLength, yLength];

        GameObject root = new GameObject();
        root.name = "root";
        root.transform.parent = transform;
        GameObject basicTile = Resources.Load<GameObject>("Prefabs/Tile");

        foreach(TileData tileData in activeMapData.MapTiles)
        {
            string originalPath = mapToLoadPath + "TileData/tile" + tileData.X + tileData.Y;
            TileData activeData = Instantiate(Resources.Load<TileData>(originalPath));

            List<GameEntityData> activeEntities = new List<GameEntityData>();
            foreach(GameEntityData entityData in activeData.Entities)
            {
                originalPath = mapToLoadPath + "EntityData/" + entityData.name;
                GameEntityData activeEntityData = Instantiate(Resources.Load<GameEntityData>(originalPath));
                activeEntities.Add(activeEntityData);
            }
            activeData.Entities = activeEntities.ToArray();

            GameObject newlyInstantiatedTile = Instantiate(basicTile, new Vector3(activeData.X * TileSize, 0, activeData.Y * TileSize), new Quaternion(), root.transform);
            GameTile tile = newlyInstantiatedTile.GetComponent<GameTile>();
            tile.Initialize(activeData);

            if (activeData.Entities != null && activeData.Entities.Length > 0)
            {
                foreach (GameEntityData data in activeData.Entities)
                {
                    tile.SpawnEntity(data);
                }
            }

            newlyInstantiatedTile.name = string.Format("{0},{1}", activeData.X, activeData.Y);
            activeTiles[activeData.X, activeData.Y] = tile;
        }
    }
}
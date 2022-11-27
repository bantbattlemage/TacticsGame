using NesScripts.Controls.PathFind;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    public MapData mapData;

    private GameTile[,] activeTiles;

    public static int TileSize = 10;

    private void OnApplicationQuit()
    {
        ClearLoadedMapCache();
    }

    public void MoveEntity(GameEntityData entity, List<Point> path)
    {
        GameTile startTile = activeTiles[(int)entity.Location.x, (int)entity.Location.y];
        GameTile finalTile = activeTiles[path.Last().x, path.Last().y];

        startTile.RemoveEntity(entity);
        finalTile.AddEntity(entity);
        finalTile.SpawnEntity(entity);
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
                            if (buildingData.Owner == playerId && buildingData.Type == BuildingType.HQ)
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

    public void ClearLoadedMapCache()
    {
        string[] pathsToDelete = new string[]
        {
            "Assets/LoadedLevel/",
            "Assets/LoadedLevel/EntityData",
            "Assets/LoadedLevel/TileData",
        };

        AssetDatabase.DeleteAssets(pathsToDelete, new List<string>());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void LoadMap(string mapToLoadPath = "Assets/Data/MapData/New/")
    {
        ClearLoadedMapCache();

        AssetDatabase.CreateFolder("Assets", "LoadedLevel");

        AssetDatabase.CreateFolder("Assets/LoadedLevel", "EntityData");
        AssetDatabase.CreateFolder("Assets/LoadedLevel", "TileData");

        AssetDatabase.CopyAsset(mapToLoadPath + mapData.name + ".asset", "Assets/LoadedLevel/" + mapData.name + ".asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        int xLength = mapData.MapTiles.Max(x => x.X) + 1;
        int yLength = mapData.MapTiles.Max(x => x.Y) + 1;
        activeTiles = new GameTile[xLength, yLength];

        GameObject root = new GameObject();
        root.name = "root";
        root.transform.parent = transform;
        GameObject basicTile = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tile.prefab");

        foreach(TileData tileData in mapData.MapTiles)
        {
            string originalPath = mapToLoadPath + "TileData/tile" + tileData.X + tileData.Y + ".asset";
            string path = "Assets/LoadedLevel/TileData/tile" + tileData.X + tileData.Y + ".asset";
            AssetDatabase.CopyAsset(originalPath, path);
            TileData activeData = AssetDatabase.LoadAssetAtPath<TileData>(path);

            List<GameEntityData> activeEntities = new List<GameEntityData>();
            foreach(GameEntityData entityData in activeData.Entities)
            {
                originalPath = mapToLoadPath + "EntityData/" + entityData.name + ".asset";
                path = "Assets/LoadedLevel/EntityData/tile" + entityData.name + ".asset";
                AssetDatabase.CopyAsset(originalPath, path);
                GameEntityData activeEntityData = AssetDatabase.LoadAssetAtPath<GameEntityData>(path);
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

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public Vector2 MapSize
    {
        get
        {
            return new Vector2(activeTiles.GetLength(0), activeTiles.GetLength(1));
        }
    }

    public GameTile GetTile(int x, int y)
    {
        if(x >= 0 && x < activeTiles.GetLength(0) && y >= 0 && y < activeTiles.GetLength(1))
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
}
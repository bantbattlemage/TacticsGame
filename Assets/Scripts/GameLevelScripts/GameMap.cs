using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    public MapData mapData;

    private GameTile[,] activeTiles;

    public static int TileSize = 10;

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

    public void LoadMap()
    {
        int xLength = mapData.MapTiles.Max(x => x.X) + 1;
        int yLength = mapData.MapTiles.Max(x => x.Y) + 1;
        activeTiles = new GameTile[xLength, yLength];

        GameObject root = new GameObject();
        root.name = "root";
        root.transform.parent = transform;
        GameObject basicTile = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tile.prefab");

        foreach(TileData tileData in mapData.MapTiles)
        {
            GameObject newlyInstantiatedTile = Instantiate(basicTile, new Vector3(tileData.X * TileSize, 0, tileData.Y * TileSize), new Quaternion(), root.transform);
            GameTile tile = newlyInstantiatedTile.GetComponent<GameTile>();
            tile.Initialize(tileData);

            if (tileData.Entities != null && tileData.Entities.Length > 0)
            {
                foreach (GameEntityData data in tileData.Entities)
                {
                    GameObject newEntity = Instantiate(data.Prefab, newlyInstantiatedTile.transform);
                    newEntity.transform.localPosition = new Vector3(0, 0, 0);
                    newEntity.GetComponent<GameEntity>().Initialize(data, new Vector2(tileData.X, tileData.Y));
                }
            }

            newlyInstantiatedTile.name = string.Format("{0},{1}", tileData.X, tileData.Y);
            activeTiles[tileData.X, tileData.Y] = tile;
        }
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
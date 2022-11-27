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
            tile.TileData = tileData;
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
        return activeTiles[x, y];
    }
}
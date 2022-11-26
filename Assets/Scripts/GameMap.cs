using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    public MapData mapData;

    private GameTile[,] activeTiles;

    public static int TileSize = 10;

    public static GameMap Instance
    {
        get
        {
            return FindObjectOfType<GameMap>();
        }
    }

    private void Start()
    {
        LoadMap();
    }

    public void LoadMap()
    {
        int xLength = mapData.MapTiles.GetLength(0);
        int yLength = mapData.MapTiles.GetLength(1);
        activeTiles = new GameTile[xLength, yLength];

        GameObject root = new GameObject();
        root.name = "root";
        root.transform.parent = transform;

        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                GameTile tile = mapData.MapTiles[x, y];
                Object newlyInstantiatedTile = Instantiate(tile, new Vector3(x * TileSize, 0, y * TileSize), new Quaternion(), root.transform);
                newlyInstantiatedTile.name = string.Format("{0},{1}", x, y);
            }
        }

        GameCamera.Instance.Initialize();
    }

    public Vector2 MapSize
    {
        get
        {
            return new Vector2(activeTiles.GetLength(0), activeTiles.GetLength(1));
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapData : ScriptableObject
{
    public GameTile[,] MapTiles;

    public void OnEnable()
    {
        GameTile basicTile = AssetDatabase.LoadAssetAtPath<GameTile>("Assets/Prefabs/Tile.prefab");
        MapTiles = new GameTile[10,10];
        for(int x = 0; x < MapTiles.GetLength(0); x++)
        {
            for(int y = 0; y < MapTiles.GetLength(1); y++)
            {
                MapTiles[x, y] = basicTile;
            }
        }
    }
}
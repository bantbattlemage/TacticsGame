using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapData : ScriptableObject
{
    public TileData[] MapTiles;
}
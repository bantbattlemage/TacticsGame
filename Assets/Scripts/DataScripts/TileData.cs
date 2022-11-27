using UnityEngine;

public class TileData : ScriptableObject
{
    public int X; 
    public int Y;
    public TerrainType Type;
    public GameEntityData[] Entities;
}
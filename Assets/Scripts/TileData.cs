using UnityEngine;

public class TileData : ScriptableObject
{
    public int X; 
    public int Y;
    public bool Blocked;
    public GameEntityData[] Entities;
}
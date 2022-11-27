using UnityEngine;

public class TileData : GameData
{
    public int X; 
    public int Y;
    public TerrainType Type;
    public GameEntityData[] Entities;

    public override GameDataType DataType
    {
        get
        {
            return GameDataType.Tile;
        }
    }

    public override GameDataType OutputDataToString(out string info)
    {
        info = "";

        return DataType;
    }
}
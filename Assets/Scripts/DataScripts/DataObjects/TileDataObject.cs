using TacticGameData;
using Unity.VisualScripting;
using UnityEngine;

public class TileDataObject : GameDataObject, ITileData
{
	[field: SerializeField]
	public int X { get; set; }

	[field: SerializeField]
	public int Y { get; set; }

	[field: SerializeField]
	public TerrainType Type { get; set; }

	public UnitData[] UnitEntities { get; set; }

	public BuildingData[] BuildingEntities { get; set; }

	public override GameDataType DataType
	{
		get
		{
			return GameDataType.Tile;
		}
	}

	public TileDataObject Instantiate()
	{
		TileDataObject newTile = CreateInstance<TileDataObject>();
		newTile.X = X;
		newTile.Y = Y;
		newTile.Type = Type;
		newTile.UnitEntities = UnitEntities;
		newTile.BuildingEntities = BuildingEntities;

		return newTile;
	}

	public static TileDataObject Instantiate(TileData tileData)
	{
		TileDataObject newTile = CreateInstance<TileDataObject>();
		newTile.X = tileData.X;
		newTile.Y = tileData.Y;
		newTile.Type = tileData.Type;
		newTile.UnitEntities = tileData.UnitEntities;
		newTile.BuildingEntities = tileData.BuildingEntities;

		return newTile;
	}

	public override GameDataType OutputDataToString(out string info)
	{
		info = "";

		return DataType;
	}

	public new TileData GetData()
	{
		return new TileData();
	}
}
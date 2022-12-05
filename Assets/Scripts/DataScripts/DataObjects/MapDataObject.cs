using System.Linq;
using TacticGameData;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapDataObject : GameDataObject, IMapData
{
	public PlayerDataObject[] MapPlayerObjects;
	public TileDataObject[] TileDataObjects;

	public PlayerData[] MapPlayers { get; set; }
	public TileData[] MapTiles { get; set; }

	public MapDataObject() 
	{
		if(MapPlayerObjects != null)
		{
			MapPlayers = MapPlayerObjects.Select(x => x.ToData()).ToArray();
		}

		if (TileDataObjects != null)
		{
			MapTiles = TileDataObjects.Select(x => x.ToData()).ToArray();
		}
	}

	public new MapData ToData()
	{
		MapData mapdata = new MapData();

		mapdata.MapPlayers = MapPlayers;
		mapdata.MapTiles = MapTiles;

		return mapdata;
	}
}
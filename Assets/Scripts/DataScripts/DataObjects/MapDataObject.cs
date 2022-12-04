using TacticGameData;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapDataObject : GameDataObject, IMapData
{
	public PlayerData[] MapPlayers { get; set; }
	public TileData[] MapTiles { get; set; }

	public new MapData GetData()
	{
		return new MapData();
	}
}
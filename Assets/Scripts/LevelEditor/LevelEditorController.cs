using System.Collections.Generic;
using System.Linq;
using TacticGameData;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class LevelEditorController : MonoBehaviour
{
	[Header("Generate New Map")]
	public bool GenerateTiles = false;
	public bool DeleteTiles = false;
	public int Length;
	public int Width;
	public int Players;

	[Header("Save")]
	public string Name = "NewMap";
	public bool SaveTiles = false;

	[Header("Load")]
	public bool LoadMap = false;
	public MapDataObject MapToLoad;

	[Header("Settings")]
	public GameObject BasicTile;
	public int TileSize = 10;

	public GameObject[,] LoadedTiles;

	private void Update()
	{
		if (GenerateTiles)
		{
			Delete();
			CreateMap();
			GenerateTiles = false;
		}

		if (SaveTiles)
		{
			SaveMap();
			SaveTiles = false;
		}

		if (DeleteTiles)
		{
			Delete();
			DeleteTiles = false;
		}

		if (LoadMap && MapToLoad != null)
		{
			Delete();
			CreateMap(MapToLoad);
			LoadMap = false;
		}
	}

	private void SaveMap()
	{
		if(LoadedTiles == null || LoadedTiles.Length == 0)
		{
			Debug.LogWarning("No tiles loaded");
			return;
		}

		string name = Name;
		AssetDatabase.DeleteAsset("Assets/" + name);
		AssetDatabase.CreateFolder("Assets", name);
		AssetDatabase.CreateFolder("Assets/" + name, "EntityData");
		string playerDataGuid = AssetDatabase.CreateFolder("Assets/" + name, "PlayerData");
		string tileDataGuid = AssetDatabase.CreateFolder("Assets/" + name, "TileData");
		string tileFolderPath = AssetDatabase.GUIDToAssetPath(tileDataGuid);
		string playerFolderPath = AssetDatabase.GUIDToAssetPath(playerDataGuid);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		MapDataObject newMapData = ScriptableObject.CreateInstance<MapDataObject>();
		AssetDatabase.CreateAsset(newMapData, "Assets/" + name + "/" + Name + ".asset");

		List<TileDataObject> newTiles = new List<TileDataObject>();
		for (int x = 0; x < LoadedTiles.GetLength(0); x++)
		{
			for (int y = 0; y < LoadedTiles.GetLength(1); y++)
			{
				TileData data = LoadedTiles[x, y].GetComponent<GameTile>().TileData;
				TileDataObject dataObject = TileDataObject.Instantiate(data);
				AssetDatabase.CreateAsset(dataObject, tileFolderPath + "/tile" + x + y + ".asset");
				newTiles.Add(dataObject);
			}
		}

		AssetDatabase.SaveAssets();

		newMapData.TileDataObjects = newTiles.ToArray();
		//newMapData.MapTiles = newTiles.Select(x=>x.ToData()).ToArray();
		newMapData.MapPlayers = new PlayerData[Players];

		for (int i = 0; i < Players; i++)
		{
			PlayerDataObject newPlayer = ScriptableObject.CreateInstance<PlayerDataObject>();
			newPlayer.ID = i;
			newPlayer.Name = string.Format("Player {0}", i + 1);
			newMapData.MapPlayers[i] = newPlayer.ToData();
			AssetDatabase.CreateAsset(newPlayer, playerFolderPath + "/Player" + newPlayer.ID + ".asset");
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = newMapData;
	}

	private void CreateMap(MapDataObject mapData = null)
	{
		if (mapData == null)
		{
			LoadedTiles = new GameObject[Length, Width];
		}
		else
		{
			int xLength = MapToLoad.TileDataObjects.Max(x => x.X) + 1;
			int yLength = MapToLoad.TileDataObjects.Max(x => x.Y) + 1;
			LoadedTiles = new GameObject[xLength, yLength];
		}

		GameObject root = new GameObject();
		root.name = "root";
		root.transform.parent = transform;
		LoadedTiles = new GameObject[Length, Width];

		for (int x = 0; x < Length; x++)
		{
			for (int y = 0; y < Width; y++)
			{
				GameObject newlyInstantiatedTile = Instantiate(BasicTile, new Vector3(x * TileSize, 0, y * TileSize), new Quaternion(), root.transform);
				newlyInstantiatedTile.name = string.Format("{0},{1}", x, y);
				newlyInstantiatedTile.transform.parent = root.transform;

				//	read existing map tile data
				if (mapData != null)
				{
					TileData tileData = mapData.TileDataObjects.First(tile => tile.X == x && tile.Y == y).ToData();
					newlyInstantiatedTile.GetComponent<GameTile>().TileData = tileData;

					if (tileData.UnitEntities != null && tileData.UnitEntities.Length > 0)
					{
						foreach (UnitData data in tileData.UnitEntities)
						{
							GameObject prefab = Resources.Load("Prefabs/Units/" + data.Definition.UnitType.ToString()) as GameObject;
							GameObject newEntity = Instantiate(prefab, newlyInstantiatedTile.transform);
							newEntity.GetComponent<GameEntityUnit>().Initialize(data, new Point(tileData.X, tileData.Y));
						}
					}
					if (tileData.BuildingEntities != null && tileData.BuildingEntities.Length > 0)
					{
						foreach (BuildingData data in tileData.BuildingEntities)
						{
							GameObject prefab = Resources.Load("Prefabs/Buildings/" + data.Definition.BuildingType.ToString()) as GameObject;
							GameObject newEntity = Instantiate(prefab, newlyInstantiatedTile.transform);
							newEntity.GetComponent<GameEntityBuilding>().Initialize(data, new Point(tileData.X, tileData.Y));
						}
					}

					newlyInstantiatedTile.GetComponent<GameTile>().Initialize(tileData);
				}
				//	initialize new tiles
				else
				{
					TileDataObject tileData = ScriptableObject.CreateInstance<TileDataObject>();
					tileData.X = x;
					tileData.Y = y;
					tileData.UnitEntities = new UnitData[0];
					tileData.BuildingEntities = new BuildingData[0];
					tileData.Type = TerrainType.Field;
					newlyInstantiatedTile.GetComponent<GameTile>().Initialize(tileData.ToData());
				}

				LoadedTiles[x, y] = newlyInstantiatedTile;
			}
		}

		if (mapData != null)
		{
			List<BuildingDataObject> buildingData = LoadBuildings();

			foreach (BuildingDataObject data in buildingData)
			{
				LoadedTiles[data.Location.X, data.Location.Y].GetComponent<GameTile>().AddEntity(data.ToData(), true);
			}

			List<UnitDataObject> unitData = LoadUnits();

			foreach (UnitDataObject data in unitData)
			{
				LoadedTiles[data.Location.X, data.Location.Y].GetComponent<GameTile>().AddEntity(data.ToData(), true);
			}
		}
	}

	private List<BuildingDataObject> LoadBuildings()
	{
		List<BuildingDataObject> loadedBuildings = new List<BuildingDataObject>();

		BuildingDataObject[] entities = Resources.LoadAll<BuildingDataObject>("Data/MapData/" + MapToLoad.name + "/EntityData");

		foreach (BuildingDataObject building in entities)
		{
			BuildingDataObject newBuilding = building.Instantiate();
			loadedBuildings.Add(newBuilding);
		}

		return loadedBuildings;
	}

	private List<UnitDataObject> LoadUnits()
	{
		List<UnitDataObject> loadedUnits = new List<UnitDataObject>();

		UnitDataObject[] entities = Resources.LoadAll<UnitDataObject>("Data/MapData/" + MapToLoad.name + "/EntityData");

		foreach (UnitDataObject unit in entities)
		{
			UnitDataObject newUnit = unit.Instantiate();
			loadedUnits.Add(newUnit);
		}

		return loadedUnits;
	}

	private void Delete()
	{
		if (LoadedTiles != null)
		{
			foreach (GameObject g in LoadedTiles)
			{
				DestroyImmediate(g);
			}
		}

		foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
		{
			if (t == null || t == transform)
			{
				continue;
			}

			DestroyImmediate(t.gameObject);
		}
	}
}
#endif
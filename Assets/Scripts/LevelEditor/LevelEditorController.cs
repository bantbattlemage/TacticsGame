using NesScripts.Controls.PathFind;
using System.Collections.Generic;
using System.Linq;
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
	public MapData MapToLoad;

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

		MapData newMapData = ScriptableObject.CreateInstance<MapData>();
		AssetDatabase.CreateAsset(newMapData, "Assets/" + name + "/" + Name + ".asset");

		List<TileData> newTiles = new List<TileData>();
		for (int x = 0; x < LoadedTiles.GetLength(0); x++)
		{
			for (int y = 0; y < LoadedTiles.GetLength(1); y++)
			{
				TileData data = LoadedTiles[x, y].GetComponent<GameTile>().TileData;
				AssetDatabase.CreateAsset(data, tileFolderPath + "/tile" + x + y + ".asset");
				newTiles.Add(data);
			}
		}

		AssetDatabase.SaveAssets();

		newMapData.MapTiles = newTiles.ToArray();
		newMapData.MapPlayers = new PlayerData[Players];

		for (int i = 0; i < Players; i++)
		{
			PlayerData newPlayer = ScriptableObject.CreateInstance<PlayerData>();
			newPlayer.ID = i;
			newPlayer.Name = string.Format("Player {0}", i + 1);
			newMapData.MapPlayers[i] = newPlayer;
			AssetDatabase.CreateAsset(newPlayer, playerFolderPath + "/Player" + newPlayer.ID + ".asset");
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = newMapData;
	}

	private void CreateMap(MapData mapData = null)
	{
		if (mapData == null)
		{
			LoadedTiles = new GameObject[Length, Width];
		}
		else
		{
			int xLength = MapToLoad.MapTiles.Max(x => x.X) + 1;
			int yLength = MapToLoad.MapTiles.Max(x => x.Y) + 1;
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

				if (mapData != null)
				{
					TileData tileData = mapData.MapTiles.First(tile => tile.X == x && tile.Y == y);
					newlyInstantiatedTile.GetComponent<GameTile>().TileData = tileData;
					if (tileData.Entities != null && tileData.Entities.Length > 0)
					{
						foreach (GameEntityData data in tileData.Entities)
						{
							GameObject newEntity = Instantiate(data.Definition.Prefab, newlyInstantiatedTile.transform);
							newEntity.GetComponent<GameEntity>().Initialize(data, new Point(tileData.X, tileData.Y));
						}
					}

					newlyInstantiatedTile.GetComponent<GameTile>().Initialize(tileData);
				}
				else
				{
					TileData tileData = ScriptableObject.CreateInstance<TileData>();
					tileData.X = x;
					tileData.Y = y;
					tileData.Entities = new GameEntityData[0];
					tileData.Type = TerrainType.Field;
					newlyInstantiatedTile.GetComponent<GameTile>().Initialize(tileData);
				}

				LoadedTiles[x, y] = newlyInstantiatedTile;
			}
		}
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
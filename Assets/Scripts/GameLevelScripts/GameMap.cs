using NesScripts.Controls.PathFind;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class GameMap : MonoBehaviour
{
	public MapData mapData;

	private GameTile[,] activeTiles;

	public static int TileSize = 10;

	public Vector2 MapSize
	{
		get
		{
			return new Vector2(activeTiles.GetLength(0), activeTiles.GetLength(1));
		}
	}

	public GameEntityUnit SpawnNewUnit(UnitDefinition unitToSpawn, Point location, GamePlayer owner)
	{
		GameEntityData newUnit = unitToSpawn.Instantiate();
		GameTile tile = GetTile(location);

		tile.AddEntity(newUnit);

		GameEntityUnit newEntity = tile.SpawnEntity(newUnit) as GameEntityUnit;
		newEntity.Initialize(newUnit, location);
		newEntity.SetOwner(owner.GamePlayerData.ID);

		return newEntity;
	}

	public void DestroyUnit(UnitData unitToDestroy)
	{
		GameTile tile = GetTile(unitToDestroy.Location);
		tile.RemoveEntity(unitToDestroy);
		Destroy(unitToDestroy);
	}

	public GameTile GetTile(Point location)
	{
		return GetTile(location.x, location.y);
	}

	public GameTile GetTile(int x, int y)
	{
		if (x >= 0 && x < activeTiles.GetLength(0) && y >= 0 && y < activeTiles.GetLength(1))
		{
			return activeTiles[x, y];
		}
		else
		{
			return null;
		}
	}

	public List<Point> FindPath(Point from, Point to, List<GameTile> tiles)
	{
		bool[,] tilesMap = new bool[(int)MapSize.x, (int)MapSize.y];
		for (int x = 0; x < tilesMap.GetLength(0); x++)
		{
			for (int y = 0; y < tilesMap.GetLength(1); y++)
			{
				tilesMap[x, y] = false;
			}
		}

		foreach (GameTile tile in tiles)
		{
			tilesMap[tile.TileData.X, tile.TileData.Y] = true;
		}

		NesScripts.Controls.PathFind.Grid grid = new NesScripts.Controls.PathFind.Grid(tilesMap);
		List<Point> path = Pathfinding.FindPath(grid, from, to, Pathfinding.DistanceType.Manhattan);

		return path;
	}

	/// <summary>
	/// Returns all tiles within 'walking' distance of the startPoint.
	/// </summary>
	public static List<GameTile> FilterTilesByDistance(List<GameTile> tiles, Point startPoint, int distance)
	{
		bool[,] tilesMap = new bool[(int)GameController.Instance.CurrentGameMatch.Map.MapSize.x, (int)GameController.Instance.CurrentGameMatch.Map.MapSize.y];
		for (int x = 0; x < tilesMap.GetLength(0); x++)
		{
			for (int y = 0; y < tilesMap.GetLength(1); y++)
			{
				tilesMap[x, y] = false;
			}
		}

		foreach (GameTile tile in tiles)
		{
			tilesMap[tile.TileData.X, tile.TileData.Y] = true;
		}

		List<GameTile> filteredTiles = new List<GameTile>();
		foreach (GameTile tile in tiles)
		{
			NesScripts.Controls.PathFind.Grid grid = new NesScripts.Controls.PathFind.Grid(tilesMap);
			Point from = new Point(startPoint.x, startPoint.y);
			Point to = new Point(tile.TileData.X, tile.TileData.Y);
			List<Point> path = Pathfinding.FindPath(grid, from, to, Pathfinding.DistanceType.Manhattan);

			if (path.Count <= distance && path.Count > 0)
			{
				filteredTiles.Add(tile);
			}
		}

		return filteredTiles;
	}

	public GameTile[,] GameTiles
	{
		get
		{
			return activeTiles;
		}
	}

	public GameEntity GetEntity(GameEntityData data)
	{
		GameEntity entity = null;

		foreach (GameTile tile in activeTiles)
		{
			if(tile.TileData.Entities.Contains(data))
			{
				entity = tile.SpawnedEntities.First(x => x.Data == data);
				break;
			}
		}

		return entity;
	}

	public List<GameEntity> GetAllPlayerEntities(int playerId)
	{
		List<GameEntity> gameEntities = new List<GameEntity>();

		foreach (GameTile tile in activeTiles)
		{
			foreach (GameEntity e in tile.SpawnedEntities)
			{
				if (e.Data.Owner == playerId)
				{
					gameEntities.Add(e);
				}
			}
		}

		return gameEntities;
	}

	public GameTile GetPlayerHQ(int playerId)
	{
		for (int x = 0; x < activeTiles.GetLength(0); x++)
		{
			for (int y = 0; y < activeTiles.GetLength(1); y++)
			{
				TileData tile = activeTiles[x, y].TileData;

				if (tile.Entities != null && tile.Entities.Length > 0)
				{
					try
					{
						foreach (BuildingData buildingData in tile.Entities)
						{
							if (buildingData.Owner == playerId && buildingData.TypedDefinition.BuildingType == GameBuildingType.HQ)
							{
								return activeTiles[x, y];
							}
						}
					}
					catch
					{
						//  fail silently on non-HQ entities
					}
				}
			}
		}

		return null;
	}

	public void MoveEntity(GameEntityData entity, List<Point> path)
	{
		GameTile startTile = activeTiles[(int)entity.Location.x, (int)entity.Location.y];
		GameTile finalTile = activeTiles[path.Last().x, path.Last().y];

		startTile.RemoveEntity(entity);
		finalTile.AddEntity(entity);
		finalTile.SpawnEntity(entity);
	}

	public void LoadMap(string mapToLoadPath, string mapName)
	{
		string fullMapPathOriginal = mapToLoadPath + mapName;
		MapData activeMapData = Instantiate(Resources.Load<MapData>(fullMapPathOriginal));
		mapData = activeMapData;

		List<PlayerData> activePlayers = new List<PlayerData>();
		foreach (PlayerData playerData in activeMapData.MapPlayers)
		{
			string originalPath = mapToLoadPath + "PlayerData/" + playerData.name;
			PlayerData activePlayerData = Instantiate(Resources.Load<PlayerData>(originalPath));
			activePlayers.Add(activePlayerData);
		}
		activeMapData.MapPlayers = activePlayers.ToArray();

		int xLength = activeMapData.MapTiles.Max(x => x.X) + 1;
		int yLength = activeMapData.MapTiles.Max(x => x.Y) + 1;
		activeTiles = new GameTile[xLength, yLength];

		GameObject root = new GameObject();
		root.name = "root";
		root.transform.parent = transform;
		GameObject basicTile = Resources.Load<GameObject>("Prefabs/Game/Tile");

		foreach (TileData tileData in activeMapData.MapTiles)
		{
			string originalPath = mapToLoadPath + "TileData/tile" + tileData.X + tileData.Y;
			TileData activeData = Instantiate(Resources.Load<TileData>(originalPath));

			List<GameEntityData> activeEntities = new List<GameEntityData>();
			foreach (GameEntityData entityData in activeData.Entities)
			{
				originalPath = mapToLoadPath + "EntityData/" + entityData.name;
				GameEntityData activeEntityData = Instantiate(Resources.Load<GameEntityData>(originalPath));
				activeEntities.Add(activeEntityData);
			}
			activeData.Entities = activeEntities.ToArray();

			GameObject newlyInstantiatedTile = Instantiate(basicTile, new Vector3(activeData.X * TileSize, 0, activeData.Y * TileSize), new Quaternion(), root.transform);
			GameTile tile = newlyInstantiatedTile.GetComponent<GameTile>();
			tile.Initialize(activeData);

			if (activeData.Entities != null && activeData.Entities.Length > 0)
			{
				foreach (GameEntityData data in activeData.Entities)
				{
					tile.SpawnEntity(data);
				}
			}

			newlyInstantiatedTile.name = string.Format("{0},{1}", activeData.X, activeData.Y);
			activeTiles[activeData.X, activeData.Y] = tile;
		}
	}
}
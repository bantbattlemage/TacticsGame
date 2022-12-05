using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TacticGameData;
using NesScripts.Controls.PathFind;

public class GameMap : MonoBehaviour
{
	public MapData mapData;

	private GameTile[,] activeTiles;

	public static int TileSize = 10;

	public delegate void GameMapUnitEvent(UnitData entityData, GameTile tile);
	public delegate void GameMapBuildingEvent(BuildingData entityData, GameTile tile);
	public GameMapUnitEvent UnitSpawnedEvent;
	public GameMapUnitEvent UnitDestroyedEvent;
	public GameMapUnitEvent UnitMovedEvent;
	public GameMapBuildingEvent BuildingSpawnedEvent;
	public GameMapBuildingEvent BuildingDestroyedEvent;
	public GameMapBuildingEvent BuildingOwnerChangedEvent;

	public Vector2 MapSize
	{
		get
		{
			return new Vector2(activeTiles.GetLength(0), activeTiles.GetLength(1));
		}
	}

	public GameEntityUnit SpawnNewUnit(UnitDefinition unitToSpawn, Point location, GamePlayer owner)
	{
		UnitDataObject newUnit = UnitDefinitionObject.Instantiate(unitToSpawn);
		UnitData data = newUnit.ToData();
		GameTile tile = GetTile(location);

		tile.AddEntity(data);

		GameEntityUnit newEntity = tile.SpawnEntity(data);
		newEntity.Initialize(data, location);
		newEntity.SetOwner(owner.GamePlayerData.ID);

		if(UnitSpawnedEvent != null)
		{
			UnitSpawnedEvent(data, tile);
		}

		return newEntity;
	}

	public GameEntityBuilding SpawnNewBuilding(BuildingDefinition buildingToSpawn, Point location, GamePlayer owner)
	{
		BuildingDataObject newBuilding = BuildingDefinitionObject.Instantiate(buildingToSpawn);
		BuildingData data = newBuilding.ToData();
		GameTile tile = GetTile(location);

		tile.AddEntity(data);

		GameEntityBuilding newEntity = tile.SpawnEntity(data);
		newEntity.Initialize(data, location);
		newEntity.SetOwner(owner.GamePlayerData.ID);

		if (BuildingSpawnedEvent != null)
		{
			BuildingSpawnedEvent(data, tile);
		}

		return newEntity;
	}

	public void DestroyUnit(UnitData unitToDestroy)
	{
		GameTile tile = GetTile(unitToDestroy.Location);
		tile.RemoveEntity(unitToDestroy);

		if (UnitDestroyedEvent != null)
		{
			UnitDestroyedEvent(unitToDestroy, tile);
		}

		//Destroy(unitToDestroy);
	}

	public GameTile GetTile(Point location)
	{
		return GetTile(location.X, location.Y);
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
		bool[,] tilesMap = new bool[(int)Instance.MapSize.x, (int)Instance.MapSize.y];
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
			Point from = new Point(startPoint.X, startPoint.Y);
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

	public GameEntityBuilding GetEntity(BuildingData data)
	{
		GameEntityBuilding entity = null;

		foreach (GameTile tile in activeTiles)
		{
			if(tile.TileData.BuildingEntities.Contains(data))
			{
				entity = tile.SpawnedBuildings.First(x => x.Data == data);
				break;
			}
		}

		return entity;
	}

	public GameEntityUnit GetEntity(UnitData data)
	{
		GameEntityUnit entity = null;

		foreach (GameTile tile in activeTiles)
		{
			if (tile.TileData.UnitEntities.Contains(data))
			{
				entity = tile.SpawnedUnits.First(x => x.Data == data);
				break;
			}
		}

		return entity;
	}

	public List<GameEntityUnit> GetAllPlayerUnitEntities(int playerId)
	{
		List<GameEntityUnit> gameEntities = new List<GameEntityUnit>();

		foreach (GameTile tile in activeTiles)
		{
			foreach (GameEntity e in tile.SpawnedUnits)
			{
				if (e.Data.Owner == playerId && e.Data.Definition.EntityType == GameEntityType.Unit)
				{
					gameEntities.Add(e as GameEntityUnit);
				}
			}
		}

		return gameEntities;
	}

	public List<GameEntityBuilding> GetAllPlayerBuildingEntities(int playerId)
	{
		List<GameEntityBuilding> gameEntities = new List<GameEntityBuilding>();

		foreach (GameTile tile in activeTiles)
		{
			foreach (GameEntity e in tile.SpawnedUnits)
			{
				if (e.Data.Owner == playerId && e.Data.Definition.EntityType == GameEntityType.Building)
				{
					gameEntities.Add(e as GameEntityBuilding);
				}
			}
		}

		return gameEntities;
	}

	public List<GameEntity> GetAllPlayerEntities(int playerId)
	{
		List<GameEntity> gameEntities = new List<GameEntity>();

		foreach (GameTile tile in activeTiles)
		{
			foreach (GameEntity e in tile.SpawnedUnits)
			{
				if (e.Data.Owner == playerId)
				{
					gameEntities.Add(e);
				}
			}
			foreach (GameEntity e in tile.SpawnedBuildings)
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

				if (tile.BuildingEntities != null && tile.BuildingEntities.Length > 0)
				{
					try
					{
						foreach (BuildingData buildingData in tile.BuildingEntities)
						{
							if (buildingData.Owner == playerId && BuildingDefinitionObject.ShopBuildings.Contains(buildingData.Definition.BuildingType))
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

	/// <summary>
	/// Move the given entity along the given path to the final point in the path. This function does NOT modify any unit data other than position, including Movement points.
	/// </summary>
	public void MoveEntity(UnitData entity, List<Point> path)
	{
		GameTile startTile = activeTiles[entity.Location.X, entity.Location.Y];
		GameTile finalTile = activeTiles[path.Last().X, path.Last().Y];

		startTile.RemoveEntity(entity);
		finalTile.AddEntity(entity);
		finalTile.SpawnEntity(entity);

		if(UnitMovedEvent != null)
		{
			UnitMovedEvent(entity, finalTile);
		}
	}

	public void LoadMap(string mapToLoadPath, string mapName)
	{
		//string fullMapPathOriginal = mapToLoadPath + mapName;
		//MapDataObject activeMapDataObject = Instantiate(Resources.Load<MapDataObject>(fullMapPathOriginal));
		//mapData = activeMapDataObject.ToData();

		//List<PlayerData> activePlayers = new List<PlayerData>();
		//foreach (PlayerData playerData in mapData.MapPlayers)
		//{
		//	//string originalPath = mapToLoadPath + "PlayerData/" + playerData.name;
		//	//PlayerDataObject activePlayerData = Instantiate(Resources.Load<PlayerDataObject>(originalPath));
		//	PlayerDataObject activePlayerData = PlayerDataObject.Instantiate(playerData);
		//	activePlayers.Add(activePlayerData.ToData());
		//}

		//mapData.MapPlayers = activePlayers.ToArray();

		//int xLength = mapData.MapTiles.Max(x => x.X) + 1;
		//int yLength = mapData.MapTiles.Max(x => x.Y) + 1;
		//activeTiles = new GameTile[xLength, yLength];

		//GameObject root = new GameObject();
		//root.name = "root";
		//root.transform.parent = transform;
		//GameObject basicTile = Resources.Load<GameObject>("Prefabs/Game/Tile");
		//string originalPath = mapToLoadPath + "EntityData/";
		//List<GameEntityDataObject> dataObjects = Resources.LoadAll<GameEntityDataObject>(originalPath).ToList();

		//foreach (TileData tileData in mapData.MapTiles)
		//{
		//	originalPath = mapToLoadPath + "TileData/tile" + tileData.X + tileData.Y;
		//	TileDataObject activeData = Instantiate(Resources.Load<TileDataObject>(originalPath));

		//	List<GameEntityData> activeEntities = new List<GameEntityData>();
		//	foreach (GameEntityData entityData in activeData.Entities)
		//	{
		//		GameEntityDataObject activeEntityData = Instantiate(dataObjects.First(x => x.ToData() == entityData));
		//		activeEntities.Add(activeEntityData.ToData());
		//		dataObjects.Remove(activeEntityData);
		//	}
		//	activeData.Entities = activeEntities.ToArray();

		//	GameObject newlyInstantiatedTile = Instantiate(basicTile, new Vector3(activeData.X * TileSize, 0, activeData.Y * TileSize), new Quaternion(), root.transform);
		//	GameTile tile = newlyInstantiatedTile.GetComponent<GameTile>();
		//	tile.Initialize(activeData);

		//	if (activeData.Entities != null && activeData.Entities.Length > 0)
		//	{
		//		foreach (GameEntityData data in activeData.Entities)
		//		{
		//			tile.SpawnEntity(data);
		//		}
		//	}

		//	newlyInstantiatedTile.name = string.Format("{0},{1}", activeData.X, activeData.Y);
		//	activeTiles[activeData.X, activeData.Y] = tile;
		//}
	}

	public static GameMap Instance
	{
		get
		{
			return GameMatch.Instance.Map;
		}
	}
}
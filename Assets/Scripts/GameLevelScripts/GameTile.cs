using System.Collections.Generic;
using System.Linq;
using TacticGameData;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class GameTile : MonoBehaviour
{
	public GameObject BasePlane;
	public GameObject HilightPlane;
	public Material HilightGreenMaterial;
	public Material HilightRedMaterial;
	public TileData TileData;

	public List<GameEntityUnit> SpawnedUnits = new List<GameEntityUnit>();
	public List<GameEntityBuilding> SpawnedBuildings = new List<GameEntityBuilding>();

	public delegate void GameTileMoveUnitEvent(GameTile sender);
	public GameTileMoveUnitEvent GameTileMoveUnitMouseOver;
	public GameTileMoveUnitEvent GameTileMoveUnitMouseClick;

	private bool _isBeingUsedForMovement = false;
	public bool _locked = false;

	public void Initialize(TileData data)
	{
		TileData = data;

		Material terrainMaterial = null;
		switch (data.Type)
		{
			case TerrainType.Field:
				terrainMaterial = Resources.Load<Material>("Terrain/TerrainGrass");
				break;
			case TerrainType.Forest:
				terrainMaterial = Resources.Load<Material>("Terrain/TerrainForest");
				break;
			case TerrainType.Water:
				terrainMaterial = Resources.Load<Material>("Terrain/TerrainWater");
				break;
			default:
				break;
		}
		BasePlane.GetComponentInChildren<Renderer>().material = terrainMaterial;

		BasePlane.SetActive(true);
		HilightPlane.SetActive(false);
	}

	public void LockTile()
	{
		_locked = true;
	}

	public void UnlockTile()
	{
		_locked = false;
		DisableHilightForMovement();
	}

	public void AddEntity(BuildingData entityToAdd)
	{
		List<BuildingData> entities = TileData.BuildingEntities.ToList();
		entities.Add(entityToAdd);
		TileData.BuildingEntities = entities.ToArray();
	}

	public void AddEntity(UnitData entityToAdd)
	{
		List<UnitData> entities = TileData.UnitEntities.ToList();
		entities.Add(entityToAdd);
		TileData.UnitEntities = entities.ToArray();
	}

	public GameEntityBuilding SpawnEntity(BuildingData entityToSpawn)
	{
		if (TileData.BuildingEntities.Contains(entityToSpawn))
		{			
			GameObject prefab = null;
			string path = "Data/Definitions/Building";
			prefab = Resources.LoadAll<BuildingDefinitionObject>(path).First(x => x.GetData() == entityToSpawn.Definition).GameObject();

			GameObject newEntity = Instantiate(prefab, transform);
			GameEntityBuilding entity = newEntity.GetComponent<GameEntityBuilding>();
			newEntity.transform.localPosition = new Vector3(0, 0, 0);
			entity.Initialize(entityToSpawn, new Point(TileData.X, TileData.Y));
			SpawnedBuildings.Add(entity);
			return entity;
		}
		else
		{
			Debug.LogError("Entity is not in tile!");
			return null;
		}
	}

	public GameEntityUnit SpawnEntity(UnitData entityToSpawn)
	{
		if (TileData.UnitEntities.Contains(entityToSpawn))
		{
			GameObject prefab = null;
			string path = "Data/Definitions/Building";
			prefab = Resources.LoadAll<UnitDefinitionObject>(path).First(x => x.GetData() == entityToSpawn.Definition).GameObject();

			GameObject newEntity = Instantiate(prefab, transform);
			GameEntityUnit entity = newEntity.GetComponent<GameEntityUnit>();
			newEntity.transform.localPosition = new Vector3(0, 0, 0);
			entity.Initialize(entityToSpawn, new Point(TileData.X, TileData.Y));
			SpawnedUnits.Add(entity);
			return entity;
		}
		else
		{
			Debug.LogError("Entity is not in tile!");
			return null;
		}
	}

	public void RemoveEntity(BuildingData entityToRemove)
	{
		if (TileData.BuildingEntities.Contains(entityToRemove))
		{
			List<BuildingData> entities = TileData.BuildingEntities.ToList();
			GameEntityBuilding target = SpawnedBuildings.First(x => x.Data == entityToRemove);

			Destroy(target.gameObject);
			SpawnedBuildings.Remove(target);

			entities.Remove(entityToRemove);
			TileData.BuildingEntities = entities.ToArray();
		}
		else
		{
			Debug.LogError("Entity is not in tile!");
		}
	}

	public void RemoveEntity(UnitData entityToRemove)
	{
		if (TileData.UnitEntities.Contains(entityToRemove))
		{
			List<UnitData> entities = TileData.UnitEntities.ToList();
			GameEntityUnit target = SpawnedUnits.First(x => x.Data == entityToRemove);

			Destroy(target.gameObject);
			SpawnedUnits.Remove(target);

			entities.Remove(entityToRemove);
			TileData.UnitEntities = entities.ToArray();
		}
		else
		{
			Debug.LogError("Entity is not in tile!");
		}
	}

	public void HilightGreen()
	{
		HilightPlane.GetComponent<MeshRenderer>().material = HilightGreenMaterial;
		HilightPlane.SetActive(true);
	}

	public void HilightRed()
	{
		HilightPlane.GetComponent<MeshRenderer>().material = HilightRedMaterial;
		HilightPlane.SetActive(true);
	}

	public void EnableHilightForMovement(GameTileMoveUnitEvent enterAction, GameTileMoveUnitEvent clickAction)
	{
		if (!_isBeingUsedForMovement)
		{
			HilightPlane.GetComponent<MeshRenderer>().material = HilightRedMaterial;
			HilightPlane.SetActive(true);

			GameTileMoveUnitMouseOver += enterAction;
			GameTileMoveUnitMouseClick += clickAction;

			_isBeingUsedForMovement = true;
		}
	}

	public void DisableHilightForMovement()
	{
		if (_isBeingUsedForMovement && !_locked)
		{
			HilightPlane.SetActive(false);
			_isBeingUsedForMovement = false;

			GameTileMoveUnitMouseOver = null;
			GameTileMoveUnitMouseClick = null;
		}
	}

	private void OnMouseDown()
	{
		if (_locked)
		{
			return;
		}

		if (_isBeingUsedForMovement)
		{
			GameTileMoveUnitMouseClick(this);
		}
		else
		{
			HilightPlane.GetComponent<MeshRenderer>().material = HilightRedMaterial;
		}

	}

	private void OnMouseUp()
	{
		if (_locked)
		{
			return;
		}

		if (_isBeingUsedForMovement)
		{

		}
		else
		{
			HilightPlane.GetComponent<MeshRenderer>().material = HilightGreenMaterial;
		}
	}

	private void OnMouseEnter()
	{
		if (_locked || PlayerUI.IsMouseOverInterface())
		{
			return;
		}

		if (_isBeingUsedForMovement)
		{
			HilightPlane.GetComponent<MeshRenderer>().material = HilightGreenMaterial;
			GameTileMoveUnitMouseOver(this);
		}
		else if (!HilightPlane.activeInHierarchy)
		{
			HilightGreen();
		}
	}

	private void OnMouseExit()
	{
		if (_locked)
		{
			return;
		}

		if (_isBeingUsedForMovement)
		{
			HilightPlane.GetComponent<MeshRenderer>().material = HilightRedMaterial;
		}
		else if (HilightPlane.activeInHierarchy)
		{
			HilightPlane.SetActive(false);
		}
	}
}

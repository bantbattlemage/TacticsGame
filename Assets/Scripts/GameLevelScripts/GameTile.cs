using NesScripts.Controls.PathFind;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.EventTrigger;

[System.Serializable]
public class GameTile : MonoBehaviour
{
	public GameObject BasePlane;
	public GameObject HilightPlane;
	public Material HilightGreenMaterial;
	public Material HilightRedMaterial;
	public TileData TileData;

	public List<GameEntity> SpawnedEntities = new List<GameEntity>();

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

	public void AddEntity(GameEntityData entityToAdd)
	{
		List<GameEntityData> entities = TileData.Entities.ToList();
		entities.Add(entityToAdd);
		TileData.Entities = entities.ToArray();
	}

	public void SpawnEntity(GameEntityData entityToSpawn)
	{
		if (TileData.Entities.Contains(entityToSpawn))
		{
			GameObject newEntity = Instantiate(entityToSpawn.Definition.Prefab, transform);
			GameEntity entity = newEntity.GetComponent<GameEntity>();
			newEntity.transform.localPosition = new Vector3(0, 0, 0);
			entity.Initialize(entityToSpawn, new Point(TileData.X, TileData.Y));
			SpawnedEntities.Add(entity);
		}
		else
		{
			Debug.LogError("Entity is not in tile!");
		}
	}

	public void RemoveEntity(GameEntityData entityToRemove)
	{
		if (TileData.Entities.Contains(entityToRemove))
		{
			List<GameEntityData> entities = TileData.Entities.ToList();

			GameEntity target = SpawnedEntities.First(x => x.Data == entityToRemove);

			Destroy(target.gameObject);
			SpawnedEntities.Remove(target);

			entities.Remove(entityToRemove);
			TileData.Entities = entities.ToArray();
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
		if (_locked)
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
			HilightPlane.SetActive(true);
			HilightPlane.GetComponent<MeshRenderer>().material = HilightGreenMaterial;
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

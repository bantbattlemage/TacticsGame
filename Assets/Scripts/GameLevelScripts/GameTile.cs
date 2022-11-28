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

    private bool _isBeingUsedForMovement = false;

    public delegate void GameTileMoveUnitEvent(GameTile sender);
    public GameTileMoveUnitEvent GameTileMoveUnitMouseOver;
    public GameTileMoveUnitEvent GameTileMoveUnitMouseClick;

    public void Initialize(TileData data)
    {
        TileData = data;

        Material terrainMaterial = null;
        switch (data.Type)
        {
            case TerrainType.Field:
                terrainMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Terrain/TerrainGrass.mat");
                break;
            case TerrainType.Forest:
                terrainMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Terrain/TerrainForest.mat");
                break;
            case TerrainType.Water:
                terrainMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Terrain/TerrainWater.mat");
                break;
            default:
                break;
        }
        BasePlane.GetComponentInChildren<Renderer>().material = terrainMaterial;

        BasePlane.SetActive(true);
        HilightPlane.SetActive(false);
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
            entity.Initialize(entityToSpawn, new Vector2(TileData.X, TileData.Y));
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
        if (_isBeingUsedForMovement)
        {
            HilightPlane.SetActive(false);
            _isBeingUsedForMovement = false;

            GameTileMoveUnitMouseOver = null;
            GameTileMoveUnitMouseClick = null;
        }
    }

    private void OnMouseDown()
    {
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
        if(_isBeingUsedForMovement)
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
        if (_isBeingUsedForMovement)
        {
            HilightPlane.GetComponent<MeshRenderer>().material = HilightRedMaterial;
        }
        else if(HilightPlane.activeInHierarchy)
        {
            HilightPlane.SetActive(false);
        }
    }
}

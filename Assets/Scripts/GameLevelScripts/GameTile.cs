using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GameTile : MonoBehaviour
{
    public GameObject BasePlane;
    public GameObject HilightPlane;
    public Material HilightGreenMaterial;
    public Material HilightRedMaterial;
    public TileData TileData;

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

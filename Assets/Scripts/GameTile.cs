using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameTile : MonoBehaviour
{
    public GameObject BasePlane;
    public GameObject HilightPlane;
    public Material HilightGreenMaterial;
    public Material HilightRedMaterial;
    public TileData TileData;

    public void Initialize(TileData data)
    {
        TileData = data;

        BasePlane.SetActive(true);
        HilightPlane.SetActive(false);
    }

    private void OnMouseDown()
    {
        HilightPlane.GetComponent<MeshRenderer>().material = HilightRedMaterial;
    }

    private void OnMouseUp()
    {
        HilightPlane.GetComponent<MeshRenderer>().material = HilightGreenMaterial;
    }

    private void OnMouseOver()
    {
        if (!HilightPlane.activeInHierarchy)
        {
            HilightPlane.SetActive(true);
            HilightPlane.GetComponent<MeshRenderer>().material = HilightGreenMaterial;
        }
    }

    private void OnMouseExit()
    {
        if(HilightPlane.activeInHierarchy)
        {
            HilightPlane.SetActive(false);
        }
    }
}

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

    public void Initialize()
    {
        BasePlane.SetActive(true);
        HilightPlane.SetActive(false);
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

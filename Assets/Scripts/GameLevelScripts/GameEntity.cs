using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    public GameEntityData Data;

    public virtual void Initialize(GameEntityData data, Vector2 location)
    {
        Data = data;
        Data.Location = location;
        SetPlayerColor();
    }

    public void SetPlayerColor()
    {
        Material material = null;

        switch(Data.Owner)
        {
            case 0:
                material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Player1Material.mat");
                break;
            case 1:
                material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Player2Material.mat");
                break;
            case 2:
                material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Player3Material.mat");
                break; 
            case 3:
                material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Player4Material.mat");
                break;
            default:
                break;
        }

        GetComponentsInChildren<Renderer>().ToList().ForEach(x =>
        {
            x.material = material;
        });
    }
}
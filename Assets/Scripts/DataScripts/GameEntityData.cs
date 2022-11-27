using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEntityData", menuName = "ScriptableObjects/GameEntityData", order = 1)]
public class GameEntityData : GameData
{
    public GameEntityType EntityType;
    public int Owner = -1;
    public GameObject Prefab;
    public Vector2 Location;

    public override GameDataType DataType
    {
        get
        {
            return GameDataType.Entity;
        }
    }

    public override GameDataType OutputDataToString(out string info)
    {
        info = "";

        return DataType;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameDataType
{
    UNASSIGNED,
    Tile,
    Entity,
    Map,
    Player,
    Match
}

public class GameData : ScriptableObject
{
    public virtual GameDataType DataType
    {
        get
        {
            return GameDataType.UNASSIGNED;
        }
    }

    public virtual GameDataType OutputDataToString(out string info)
    {
        info = "";

        return DataType;
    }
}
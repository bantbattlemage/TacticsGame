using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMatch : MonoBehaviour
{
    public GameMap Map;
    public GamePlayer[] Players;

    public void Initialize(GameMap map, GamePlayer[] players)
    {
        Map = map;
        Players = players;

        map.transform.parent = transform;
        map.LoadMap();

        foreach(GamePlayer p in players)
        {
            p.Initialize(map);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameMatch : MonoBehaviour
{
    public GameMap Map;
    public GamePlayer[] Players;

    [HideInInspector]
    public GamePlayer ActivePlayer;

    [HideInInspector]
    public int CurrentGameRound;

    public void Initialize(GameMap map, GamePlayer[] players)
    {
        Map = map;
        Players = players;

        map.transform.parent = transform;
        map.LoadMap();

        int index = 0;
        foreach(GamePlayer p in players)
        {
            string playerName = string.Format("Player {0}", index+1);
            p.Initialize(index, playerName, map);
            p.RequestEndTurn += OnEndTurnRequestRecieved;
            index++;
        }

        CurrentGameRound = 0;

        SetActivePlayer(players[0]);
    }

    private void OnEndTurnRequestRecieved(GamePlayer player)
    {
        if(player == ActivePlayer)
        {
            EndCurrentPlayerTurn();
        }
    }

    public void EndCurrentPlayerTurn()
    {
        int index = ActivePlayer.PlayerID;
        index++;
        if(index >= Players.Length)
        {
            index = 0;
            CurrentGameRound++;
        }

        SetActivePlayer(Players[index]);
    }

    public void SetActivePlayer(GamePlayer player)
    {
        ActivePlayer = player;

        foreach(GamePlayer p in Players)
        {
            p.gameObject.SetActive(p == ActivePlayer);
        }

        ActivePlayer.BeginTurn(CurrentGameRound);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameMatch : MonoBehaviour
{
    public MatchData matchData;

    public GameMap Map;
    public GamePlayer[] Players;

    public GamePlayer GetActivePlayer()
    {
        return Players[matchData.CurrentActivePlayer];
    }

    public void Initialize(GameMap map, GamePlayer[] players)
    {
        Map = map;
        Players = players;

        string mapToLoadPath = "Assets/Data/MapData/New/";
        string mapName = "New";

        Map.transform.parent = transform;
        Map.LoadMap(mapToLoadPath, mapName);

        int index = 0;
        foreach(GamePlayer p in players)
        {
            p.Initialize(Map.mapData.MapPlayers[index], Map);
            p.RequestEndTurn += OnEndTurnRequestRecieved;
            index++;
        }

        matchData.CurrentRound = 0;
        SetActivePlayer(0);
    }

    private void OnEndTurnRequestRecieved(GamePlayer player)
    {
        if(player.GamePlayerData.ID == matchData.CurrentActivePlayer)
        {
            EndCurrentPlayerTurn();
        }
    }

    public void EndCurrentPlayerTurn()
    {
        int index = matchData.CurrentActivePlayer;
        index++;
        
        if(index >= Players.Length)
        {
            index = 0;
            matchData.CurrentRound++;
        }

        SetActivePlayer(index);
    }

    public void SetActivePlayer(int playerID)
    {
        matchData.CurrentActivePlayer = playerID;

        foreach(GamePlayer p in Players)
        {
            p.gameObject.SetActive(p.GamePlayerData.ID == playerID);
        }

        Players[playerID].BeginTurn(matchData.CurrentRound);
    }
}
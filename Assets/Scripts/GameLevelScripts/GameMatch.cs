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

	public GamePlayer GetPlayer(int id)
	{
		return Players.First(x => x.GamePlayerData.ID == id);
	}

	public void Initialize(string mapName, GamePlayer[] players, GameMap map)
	{
		Map = map;
		Players = players;

		string mapToLoadPath = "Data/MapData/" + mapName + "/";

		Map.transform.parent = transform;
		Map.LoadMap(mapToLoadPath, mapName);

		int index = 0;
		foreach (GamePlayer p in players)
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
		if (player.GamePlayerData.ID == matchData.CurrentActivePlayer)
		{
			EndCurrentPlayerTurn();
		}
		else
		{
			throw new System.Exception("recieved end turn request from incorrect player!");
		}
	}

	private void EndCurrentPlayerTurn()
	{
		int index = matchData.CurrentActivePlayer;
		index++;

		if (index >= Players.Length)
		{
			index = 0;
			matchData.CurrentRound++;
		}

		SetActivePlayer(index);
	}

	private void SetActivePlayer(int playerID)
	{
		matchData.CurrentActivePlayer = playerID;

		foreach (GamePlayer p in Players)
		{
			p.gameObject.SetActive(p.GamePlayerData.ID == playerID);
		}

		Players[playerID].BeginTurn(matchData.CurrentRound);
	}

	public static GameMatch Instance
	{
		get
		{
			return GameController.Instance.CurrentGameMatch;
		}
	}
}
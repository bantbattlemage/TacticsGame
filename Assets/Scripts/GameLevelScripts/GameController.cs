using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{
	[HideInInspector]
	public GameMatch CurrentGameMatch;

	private static GameController _instance;
	public static GameController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<GameController>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public GameMatch StartMatch(int numberOfPlayers)
	{
		MatchData newMatchData = ScriptableObject.CreateInstance<MatchData>();
		GameMatch newMatch = Instantiate(Resources.Load<GameMatch>("Prefabs/Game/GameMatch"));
		newMatch.matchData = newMatchData;
		CurrentGameMatch = newMatch;

		GamePlayer[] players = new GamePlayer[numberOfPlayers];

		for (int i = 0; i < players.Length; i++)
		{
			players[i] = Instantiate(Resources.Load<GamePlayer>("Prefabs/Game/GamePlayer"));
			players[i].transform.parent = newMatch.transform;
		}

		newMatch.Initialize("TestMap", players, Instantiate(Resources.Load<GameMap>("Prefabs/Game/GameMap")));

		return newMatch;
	}
}
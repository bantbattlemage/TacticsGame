using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public StartMenu Menu;

	public string ServerUrl { get { return "https://localhost:5001/api/"; } }

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

	private void Start()
	{
		PulseCommunicator.Instance.Initialize();
	}

	public GameMatch StartMatch(int numberOfPlayers)
	{
		MatchDataObject newMatchData = ScriptableObject.CreateInstance<MatchDataObject>();
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

	public void EndCurrentGame()
	{
		Destroy(CurrentGameMatch.gameObject);
		Menu.gameObject.SetActive(true);
	}
}
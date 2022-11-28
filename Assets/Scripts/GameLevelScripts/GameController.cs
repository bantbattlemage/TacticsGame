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
            if(_instance == null)
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
        ClearLoadedGameCache();

        //AssetDatabase.CreateFolder("Assets/Cache", "LoadedMatch");
        MatchData newMatchData = ScriptableObject.CreateInstance<MatchData>();
        //AssetDatabase.SaveAssets();
        //AssetDatabase.CreateAsset(newMatchData, "Assets/Cache/LoadedMatch/gameMatchData.asset");

        GameMatch newMatch = Instantiate(Resources.Load<GameMatch>("Prefabs/GameMatch"));
        newMatch.matchData = newMatchData;
        CurrentGameMatch = newMatch;

        GamePlayer[] players = new GamePlayer[numberOfPlayers];

        for(int i = 0; i < players.Length; i++)
        {
            players[i] = Instantiate(Resources.Load<GamePlayer>("Prefabs/GamePlayer"));
            players[i].transform.parent = newMatch.transform;
        }

        newMatch.Initialize(Instantiate(Resources.Load<GameMap>("Prefabs/GameMap")), players);

        return newMatch;
    }

    public void ClearLoadedGameCache()
    {
        //string[] pathsToDelete = new string[]
        //{
        //    "Assets/Cache/LoadedMatch/"
        //};

        //AssetDatabase.DeleteAssets(pathsToDelete, new List<string>());
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    private void OnApplicationQuit()
    {
        ClearLoadedGameCache();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{
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
        GameMatch newMatch = Instantiate(AssetDatabase.LoadAssetAtPath<GameMatch>("Assets/Prefabs/GameMatch.prefab"));
        GamePlayer[] players = new GamePlayer[numberOfPlayers];

        for(int i = 0; i < players.Length; i++)
        {
            players[i] = Instantiate(AssetDatabase.LoadAssetAtPath<GamePlayer>("Assets/Prefabs/GamePlayer.prefab"));
            players[i].transform.parent = newMatch.transform;
        }

        newMatch.Initialize(Instantiate(AssetDatabase.LoadAssetAtPath<GameMap>("Assets/Prefabs/GameMap.prefab")), players);

        return newMatch;
    }
}
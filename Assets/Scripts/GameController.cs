using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        StartMatch();
    }

    public GameMatch StartMatch()
    {
        GamePlayer[] players = new GamePlayer[1];
        players[0] = Instantiate(AssetDatabase.LoadAssetAtPath<GamePlayer>("Assets/Prefabs/GamePlayer.prefab"));

        GameMatch newMatch = Instantiate(AssetDatabase.LoadAssetAtPath<GameMatch>("Assets/Prefabs/GameMatch.prefab"));
        newMatch.Initialize(Instantiate(AssetDatabase.LoadAssetAtPath<GameMap>("Assets/Prefabs/GameMap.prefab")), players);
        return newMatch;
    }
}
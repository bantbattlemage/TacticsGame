using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    public int PlayerID;
    public GameCamera PlayerCamera;
    public PlayerUI PlayerInterface;

    public delegate void RequestEndTurnEvent(GamePlayer sendingPlayer);
    public RequestEndTurnEvent RequestEndTurn;

    public void Initialize(int playerID, string playerName, GameMap map)
    {
        PlayerID = playerID;
        PlayerCamera.Initialize(map);
        PlayerCamera.GetComponent<Camera>().depth = playerID;
        PlayerInterface.Initialize(playerName);
        PlayerInterface.EndTurnButtonPressed += EndTurn;
    }

    public void BeginTurn(int roundNumber)
    {
        PlayerInterface.UpdateDisplayInfo(roundNumber);
    }

    private void EndTurn()
    {
        RequestEndTurn(this);
    }
}
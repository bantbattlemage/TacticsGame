using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    public int PlayerID;
    public string PlayerName;
    public GameCamera PlayerCamera;
    public PlayerUI PlayerInterface;

    public delegate void RequestEndTurnEvent(GamePlayer sendingPlayer);
    public RequestEndTurnEvent RequestEndTurn;

    public void Initialize(int playerID, string playerName, GameMap map)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        PlayerCamera.Initialize(map);
        PlayerCamera.GetComponent<Camera>().depth = playerID;
        PlayerInterface.Initialize(this);
        PlayerInterface.EndTurnButtonPressed += EndTurn;
    }

    public void BeginTurn(int roundNumber)
    {
        PlayerInterface.UpdateDisplayInfo(roundNumber);

        GameTile hqTile = GameController.Instance.CurrentGameMatch.Map.GetPlayerHQ(PlayerID);
        if(hqTile != null )
        {
            PlayerCamera.PanTo(hqTile.transform);
        }
    }

    private void EndTurn()
    {
        RequestEndTurn(this);
    }
}
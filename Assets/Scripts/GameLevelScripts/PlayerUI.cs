using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Camera PlayerUiCamera;

    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI RoundNumber;

    public Button EndTurnButton;

    private string _playerName;

    public delegate void ButtonPressedEvent();
    public ButtonPressedEvent EndTurnButtonPressed;

    public void Initialize(string playerName)
    {
        _playerName = playerName;
        PlayerName.text = _playerName;
    }

    public void UpdateDisplayInfo(int roundNumber)
    {
        RoundNumber.text = string.Format("Round: {0}", roundNumber+1);
    }

    public void OnEndTurnButtonPressed()
    {
        if(EndTurnButtonPressed != null)
        {
            EndTurnButtonPressed();
        }
    }
}

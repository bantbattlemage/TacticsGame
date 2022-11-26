using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Camera PlayerUiCamera;

    public TextMeshProUGUI PlayerName;

    public Button EndTurnButton;

    private string _playerName;

    public delegate void ButtonPressedEvent();
    public ButtonPressedEvent EndTurnButtonPressed;

    public void Initialize(string playerName)
    {
        _playerName = playerName;
        PlayerName.text = _playerName;
    }

    public void OnEndTurnButtonPressed()
    {
        if(EndTurnButtonPressed != null)
        {
            EndTurnButtonPressed();
        }
    }
}

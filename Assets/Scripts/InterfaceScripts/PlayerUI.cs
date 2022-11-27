using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Camera PlayerUiCamera;
    public PlayerTooltip Tooltip;

    [HideInInspector]
    public Camera PlayerGameCamera;

    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI RoundNumber;

    public Button EndTurnButton;

    private string _playerName;
    private RaycastHit _cachedMouseOver;

    public delegate void ButtonPressedEvent();
    public ButtonPressedEvent EndTurnButtonPressed;

    public void Initialize(GamePlayer player)
    {
        _playerName = player.PlayerName;
        PlayerGameCamera = player.PlayerCamera.GetComponent<Camera>();
        PlayerName.text = _playerName;
    }

    private void Update()
    {
        if(!isActiveAndEnabled)
        {
            return;
        }

        ProcessTooltip();
    }

    private void ProcessTooltip()
    {
        RaycastHit hit;
        Ray rayOrigin = PlayerGameCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayOrigin, out hit) && _cachedMouseOver.transform != hit.transform)
        {
            _cachedMouseOver = hit;
            ObjectTooltip toolTip = hit.transform.gameObject.GetComponent<ObjectTooltip>();
            if (toolTip != null)
            {
                Tooltip.UpdateTooltip(toolTip.DataSource);
            }
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

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
    private bool _lockMouseOver = false;

    public delegate void ButtonPressedEvent();
    public ButtonPressedEvent EndTurnButtonPressed;

    public void Initialize(GamePlayer player)
    {
        _playerName = player.PlayerName;
        PlayerGameCamera = player.PlayerCamera.GetComponent<Camera>();
        PlayerName.text = _playerName;
        Tooltip.Initialize(player);
    }

    private void Update()
    {
        if(!isActiveAndEnabled)
        {
            return;
        }

        ProcessTooltip();
        ProcessMouseClick();
    }

    public void ToggleLock()
    {
        _lockMouseOver = !_lockMouseOver;
    }

    private void SelectEntity(ObjectTooltip entityReference)
    {
        _lockMouseOver = false;
        ProcessTooltip();
        Tooltip.Select(entityReference.DataSource);
        _lockMouseOver = true;
    }

    private void ProcessMouseClick()
    {
        if(IsOverInterface())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray rayOrigin = PlayerGameCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(rayOrigin, out hit))
            {
                ObjectTooltip toolTip = hit.transform.gameObject.GetComponent<ObjectTooltip>();
                if (toolTip != null && toolTip.DataSource.DataType == GameDataType.Entity)
                {
                    SelectEntity(toolTip);
                }
            }
            else if(_lockMouseOver)
            {
                _lockMouseOver = false;
                Tooltip.Deselect();
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            _lockMouseOver = false;
            Tooltip.Deselect();
        }
    }

    private void ProcessTooltip()
    {
        if(_lockMouseOver)
        {
            return;
        }

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

    private bool IsOverInterface()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Where(x => x.gameObject.layer == 5).Count() > 0;
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

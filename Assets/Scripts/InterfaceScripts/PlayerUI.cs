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
    public PlayerTooltip TargetTooltip;
    public ConfirmationBox ConfirmBox;

    [HideInInspector]
    public Camera PlayerGameCamera;

    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI RoundNumber;

    public Button EndTurnButton;

    private string _playerName;
    private RaycastHit _cachedMouseOver;
    private bool _lockMouseOver = false;
    private int _playerId;

    public delegate void ButtonPressedEvent();
    public ButtonPressedEvent EndTurnButtonPressed;

    public void Initialize(GamePlayer player)
    {
        _playerId = player.GamePlayerData.ID;
        _playerName = player.GamePlayerData.Name;
        PlayerGameCamera = player.PlayerCamera.GetComponent<Camera>();
        PlayerName.text = _playerName;
        Tooltip.Initialize(player);
        TargetTooltip.Initialize(player);
        TargetTooltip.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!isActiveAndEnabled)
        {
            return;
        }

        ProcessTooltip();
        ProcessTargetTooltip();
        ProcessMouseClick();
        //ProcessTargetTooltipMouseClick();
    }

    public void SetLock(bool set)
    {
        _lockMouseOver = set;

        if (!_lockMouseOver)
        {
            DeselectEntity();
        }
        else if(TargetTooltip.gameObject.activeInHierarchy)
        {
            TargetTooltip.LockMouseOver();
        }
    }

    private void SelectEntity(ObjectTooltip entityReference)
    {
        _lockMouseOver = false;
        ProcessTooltip();
        Tooltip.Select(entityReference.DataSource);
        EndTurnButton.gameObject.SetActive(false);
        _lockMouseOver = true;
    }

    private void SelectEntityForTargetTooltip(ObjectTooltip entityReference)
    {
        if (!TargetTooltip.gameObject.activeInHierarchy)
        {
            return;
        }

        _lockMouseOver = false;
        ProcessTooltip();
        TargetTooltip.Select(entityReference.DataSource);
        EndTurnButton.gameObject.SetActive(false);
        _lockMouseOver = true;
    }

    private void DeselectEntity()
    {
        Tooltip.Deselect();

        if(!GameController.Instance.CurrentGameMatch.GetPlayer(_playerId).IsMovingUnit)
        {
            EndTurnButton.gameObject.SetActive(true);
            TargetTooltip.LockMouseOver(false);
            TargetTooltip.gameObject.SetActive(false);
        }
    }

    private void ProcessMouseClick()
    {
        if(IsMouseOverInterface() || _lockMouseOver)
        {
            if (!IsMouseOverInterface() && _lockMouseOver && !GameController.Instance.CurrentGameMatch.GetPlayer(_playerId).IsMovingUnit && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
            {
                _lockMouseOver = false;
                DeselectEntity();

                if (Input.GetMouseButtonDown(1))
                {
                    return;
                }
            }
            else
            {
                return;
            }
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
        }
    }

    private void ProcessTargetTooltipMouseClick()
    {
        if (IsMouseOverInterface() || !TargetTooltip.gameObject.activeInHierarchy)
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
                    SelectEntityForTargetTooltip(toolTip);
                }
            }
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

    public void EnableTargetTooltip(bool enable = true)
    {
        TargetTooltip.gameObject.SetActive(enable);
    }

    private void ProcessTargetTooltip()
    {
        if (!TargetTooltip.gameObject.activeInHierarchy)
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
                TargetTooltip.UpdateTooltip(toolTip.DataSource);
            }
        }
    }

    private bool IsMouseOverInterface()
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

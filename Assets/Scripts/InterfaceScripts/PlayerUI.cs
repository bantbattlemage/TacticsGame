using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Events;
using System;

public class PlayerUI : MonoBehaviour
{
	public Camera PlayerUiCamera;
	public PlayerTooltip Tooltip;
	public PlayerTooltip TargetTooltip;
	public ConfirmationBox ConfirmBox;
	public ShopWindow Shop;

	[HideInInspector]
	public Camera PlayerGameCamera;

	public TextMeshProUGUI PlayerName;
	public TextMeshProUGUI PlayerMoney;
	public TextMeshProUGUI RoundNumber;

	public Button EndTurnButton;

	public static int UI_LAYER = 5;

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
		Shop.gameObject.SetActive(false);
		Shop.ShopItemPurchaseRequest += OnShopItemPurchaseRequest;
	}

	private void Update()
	{
		if (!isActiveAndEnabled)
		{
			return;
		}

		ProcessTooltip();
		ProcessTargetTooltip();
		ProcessMouseClick();
	}

	public void EnableShop(BuildingData building, UnityAction onCompleteAction)
	{
		Shop.gameObject.SetActive(true);

		List<UnitDefinition> availableUnits = new List<UnitDefinition>();
		switch (building.TypedDefinition.BuildingType)
		{
			case GameBuildingType.HQ:
				availableUnits = new List<UnitDefinition>
				{
					Resources.Load<UnitDefinition>("Data/Definitions/Units/UnitDefinition_Regular"),
					Resources.Load<UnitDefinition>("Data/Definitions/Units/UnitDefinition_Artillery")
				};
				break;
		}

		Shop.Initialize(onCompleteAction);
		Shop.PopulateShop(availableUnits);
	}

	private void OnShopItemPurchaseRequest(ShopItem sender)
	{
		GamePlayer player = GameMatch.Instance.GetPlayer(_playerId);

		if(player.GamePlayerData.Money < sender.DisplayedUnit.BasePurchaseCost)
		{
			Debug.LogWarning("attempted to purchase item without enough money");
			return;
		}

		player.ExecuteBuildingBuyAction(sender.DisplayedUnit);
		Shop.DisableShop();
	}

	public void SetMoneyDisplay(int value)
	{
		PlayerMoney.text = value.ToString("C0");
	}

	public void SetLock(bool set)
	{
		_lockMouseOver = set;

		if (!_lockMouseOver)
		{
			DeselectEntity();
		}
		else if (TargetTooltip.gameObject.activeInHierarchy)
		{
			TargetTooltip.LockMouseOver();
		}
	}

	private void SelectEntity(ObjectTooltip entityReference)
	{
		_lockMouseOver = false;
		ProcessTooltip();
		Tooltip.Select(entityReference.DataSource);
		_lockMouseOver = true;
	}

	private void DeselectEntity()
	{
		Tooltip.Deselect();

		if (!GameMatch.Instance.GetPlayer(_playerId).IsUsingUnitAction)
		{
			TargetTooltip.LockMouseOver(false);
			TargetTooltip.gameObject.SetActive(false);
		}
	}

	private void ProcessMouseClick()
	{
		if (IsMouseOverInterface() || _lockMouseOver)
		{
			if (!IsMouseOverInterface() && _lockMouseOver && !GameMatch.Instance.GetPlayer(_playerId).IsUsingUnitAction && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
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

		//  Left Click
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

	private void ProcessTooltip()
	{
		if (_lockMouseOver || IsMouseOverInterface())
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
		if (!TargetTooltip.gameObject.activeInHierarchy || IsMouseOverInterface())
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

	public static bool IsMouseOverInterface()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Where(x => x.gameObject.layer == UI_LAYER).Count() > 0;
	}

	public void UpdateDisplayInfo(int roundNumber)
	{
		RoundNumber.text = string.Format("Round: {0}", roundNumber + 1);
	}

	public void EndTurnButtonAction()
	{
		if (EndTurnButtonPressed != null)
		{
			EndTurnButtonPressed();
		}
	}
}

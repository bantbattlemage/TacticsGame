using NesScripts.Controls.PathFind;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public static class DynamicButtons
{
	public static void UnitMoveButton(Button button, UnitData unitData)
	{
		button.gameObject.SetActive(true);
		button.GetComponentInChildren<TextMeshProUGUI>().text = "Move";
		button.onClick.AddListener(() =>
		{
			GameMatch.Instance.GetActivePlayer().BeginUnitMove(unitData);
		});
	}

	public static void UnitAttackButton(Button button, UnitData unitData)
	{
		button.gameObject.SetActive(true);
		button.GetComponentInChildren<TextMeshProUGUI>().text = "Attack";
		button.onClick.AddListener(() =>
		{
			GameMatch.Instance.GetActivePlayer().BeginUnitAttack(unitData);
		});
	}

	public static void HqBuyButton(Button button, BuildingData buildingData)
	{
		button.gameObject.SetActive(true);
		button.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
		button.onClick.AddListener(() =>
		{
			GameMatch.Instance.GetActivePlayer().BeginBuildingBuy(buildingData);
		});
	}

	/// <summary>
	/// Create a Capture button used to capture a building in the same tile as the unit.
	/// </summary>
	public static void UnitCaptureBuildingButton(Button button, GamePlayer player, UnitData unit, BuildingData building)
	{
		button.gameObject.SetActive(true);
		button.GetComponentInChildren<TextMeshProUGUI>().text = "Capture";

		button.onClick.AddListener(() => 
		{
			if(unit.RemainingAttacks <= 0)
			{
				return;
			}

			player.ExecuteUnitBuildingCaptureAction(unit, building);
			player.SetState(GamePlayerState.Idle_ActivePlayer);
			player.PlayerInterface.EnableTargetTooltip(false);
			player.PlayerInterface.ConfirmBox.Disable();
		});
	}

	/// <summary>
	/// Create a UnityAction used for a button that will perform a Move Unit action followed by a Capture Building action
	/// </summary>
	public static UnityAction UnitCaptureBuildingButton(GamePlayer player, UnitData unit, BuildingData building, List<Point> points)
	{
		return () =>
		{
			if (unit.RemainingAttacks <= 0)
			{
				return;
			}

			player.ExecuteUnitMoveAction(unit, points);
			player.ExecuteUnitBuildingCaptureAction(unit, building);
			player.SetState(GamePlayerState.Idle_ActivePlayer);
			player.PlayerInterface.EnableTargetTooltip(false);
			player.PlayerInterface.ConfirmBox.Disable();
		};
	}
}
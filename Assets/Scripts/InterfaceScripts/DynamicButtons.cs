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
			GameController.Instance.CurrentGameMatch.GetActivePlayer().BeginUnitMove(unitData);
		});
	}

	public static void UnitAttackButton(Button button, UnitData unitData)
	{
		button.gameObject.SetActive(true);
		button.GetComponentInChildren<TextMeshProUGUI>().text = "Attack";
		button.onClick.AddListener(() =>
		{
			GameController.Instance.CurrentGameMatch.GetActivePlayer().BeginUnitAttack(unitData);
		});
	}

	public static void HqBuyButton(Button button, BuildingData buildingData)
	{
		button.gameObject.SetActive(true);
		button.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
		button.onClick.AddListener(() =>
		{
			GameController.Instance.CurrentGameMatch.GetActivePlayer().BeginBuildingBuy(buildingData);
		});
	}

	public static UnityAction UnitCaptureBuildingButton(GamePlayer player, UnitData unit, BuildingData building, List<Point> points)
	{
		return () =>
		{
			player.ExecuteUnitMoveAction(unit, points);
			player.ExecuteUnitBuildingCaptureAction(unit, building);
			player.SetState(GamePlayerState.Idle_ActivePlayer);
			player.PlayerInterface.EnableTargetTooltip(false);
			player.PlayerInterface.ConfirmBox.Disable();
		};
	}
}
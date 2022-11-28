using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class DynamicButtons
{
    public static void UnitMoveButton(Button button, UnitData unitData)
    {
        button.gameObject.SetActive(true);
        button.GetComponentInChildren<TextMeshProUGUI>().text = "Move";
        button.onClick.AddListener(() =>
        {
            GameController.Instance.CurrentGameMatch.GetActivePlayer().BeginMoveUnit(unitData);
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
            Debug.Log("beep");
        });
    }
}
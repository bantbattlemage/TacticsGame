using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTooltip : MonoBehaviour
{
    public TextMeshProUGUI TooltipTitle;
    public TextMeshProUGUI TooltipContent;

    public Button ButtonOne;
    public Button ButtonTwo;
    public Button ButtonThree;
    public Button ButtonFour;

    private int _playerID;

    private void Awake()
    {
        DisableButtons();
    }

    public void Initialize(GamePlayer player)
    {
        _playerID = player.PlayerID;
    }

    private void DisableButtons()
    {
        ButtonOne.onClick.RemoveAllListeners();
        ButtonTwo.onClick.RemoveAllListeners();
        ButtonThree.onClick.RemoveAllListeners();
        ButtonFour.onClick.RemoveAllListeners();

        ButtonOne.gameObject.SetActive(false);
        ButtonTwo.gameObject.SetActive(false);
        ButtonThree.gameObject.SetActive(false);
        ButtonFour.gameObject.SetActive(false);
    }

    public void UpdateTooltip(GameData data)
    {
        string title = "";
        string content = "";

        switch (data.DataType)
        {
            case GameDataType.Tile:
                TileData tileData = data as TileData;
                title = string.Format("({0}, {1})", tileData.X, tileData.Y);
                content = tileData.Type.ToString();
                break;
            case GameDataType.Entity:
                GameEntityData entityData = data as GameEntityData;
                title = entityData.DataType.ToString();

                switch (entityData.EntityType)
                {
                    case GameEntityType.Unit:
                        UnitData unitData = data as UnitData;
                        content = unitData.Type.ToString();
                        content += "\nOwner PlayerID: " + unitData.Owner.ToString();
                        break;
                    case GameEntityType.Building:
                        BuildingData buildingData = data as BuildingData;
                        content = buildingData.Type.ToString();
                        content += "\nOwner PlayerID: " + buildingData.Owner.ToString();
                        break;
                    default:
                        Debug.LogWarning("invalid data on tooltip!");
                        break;
                }
                break;
            default:
                Debug.LogWarning("no data on tooltip!");
                break;
        }

        TooltipTitle.text = title;
        TooltipContent.text = content;
    }

    public void Select(GameData data)
    {
        DisableButtons();

        switch (data.DataType)
        {
            case GameDataType.Tile:
                break;
            case GameDataType.Entity:
                GameEntityData gameEntityData = data as GameEntityData;
                switch (gameEntityData.EntityType)
                {
                    case GameEntityType.Unit:
                        UnitData unitData = data as UnitData;
                        if (unitData.Owner == _playerID)
                        {
                            DynamicButtons.UnitMoveButton(ButtonOne, unitData);
                            DynamicButtons.UnitAttackButton(ButtonTwo, unitData);
                        }
                        break;
                    case GameEntityType.Building:
                        BuildingData buildingData = data as BuildingData;
                        if (buildingData.Owner == _playerID)
                        {
                            DynamicButtons.HqBuyButton(ButtonOne, buildingData);
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public void Deselect()
    {
        DisableButtons();
    }
}
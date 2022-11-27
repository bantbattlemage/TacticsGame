using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerTooltip : MonoBehaviour
{
    public TextMeshProUGUI TooltipTitle;
    public TextMeshProUGUI TooltipContent;

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
                        break;
                    case GameEntityType.Building:
                        BuildingData buildingData = data as BuildingData;
                        content = buildingData.Type.ToString();
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
}
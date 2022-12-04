using System.Collections.Generic;
using System.Linq;
using TacticGameData;
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

	private bool _lockMouseOver;

	private void Awake()
	{
		DisableButtons();
	}

	public void Initialize(GamePlayer player)
	{
		_playerID = player.GamePlayerData.ID;
	}

	public void LockMouseOver(bool setLock = true)
	{
		_lockMouseOver = setLock;
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

	public void UpdateTooltip(UnitData data)
	{
		if (_lockMouseOver)
		{
			return;
		}

		string title = "";
		string content = "";

		title = data.Definition.UnitType.ToString();
		content += "Owner PlayerID: " + data.Owner.ToString();
		content += string.Format("\nHealth: {0}/{1}", data.RemainingHealth, data.Definition.BaseHealth);
		content += string.Format("\nMovement: {0}/{1}", data.RemainingMovement, data.Definition.BaseMovement);
		content += string.Format("\nAttacks: {0}/{1}", data.RemainingAttacks, data.Definition.BaseNumberOfAttacks);
		content += string.Format("\nAttack Damage: {0}", data.Definition.BaseAttackDamage);
		content += string.Format("\nAttack Range: {0}", data.Definition.BaseAttackRange);

		TooltipTitle.text = title;
		TooltipContent.text = content;
	}

	public void UpdateTooltip(BuildingData data)
	{
		if (_lockMouseOver)
		{
			return;
		}

		string title = "";
		string content = "";

		title = data.Definition.BuildingType.ToString();
		content += "Owner PlayerID: " + data.Owner.ToString();
		content += string.Format("\nHealth: {0}/{1}", data.RemainingHealth, data.Definition.BaseHealth);
		content += string.Format("\nActions: {0}/{1}", data.RemainingBuyActions, data.Definition.BaseBuyActions);

		TooltipTitle.text = title;
		TooltipContent.text = content;
	}

	public void UpdateTooltip(TileData data)
	{
		if (_lockMouseOver)
		{
			return;
		}

		string title = string.Format("({0}, {1})", data.X, data.Y);
		string content = data.Type.ToString();

		TooltipTitle.text = title;
		TooltipContent.text = content;
	}

	public void Select(GameData data)
	{
		switch (data.DataType)
		{
			case GameDataType.Tile:
				Select(data as TileData);
				break;
			case GameDataType.Entity:
				GameEntityData<GameDefinition> entityData = data as GameEntityData<GameDefinition>;
				switch (entityData.Definition.EntityType)
				{
					case GameEntityType.Unit:
						Select(data as UnitData);
						break;
					case GameEntityType.Building:
						Select(data as BuildingData);
						break;
				}
				break;
			default:
			case GameDataType.UNASSIGNED:
				break;
		}
	}

	public void Select(UnitData data)
	{
		if (_lockMouseOver)
		{
			return;
		}

		DisableButtons();

		if (data.Owner == _playerID)
		{
			DynamicButtons.UnitMoveButton(ButtonOne, data);
			DynamicButtons.UnitAttackButton(ButtonTwo, data);

			//	check if we need a Capture button
			List<BuildingData> tileEntities = GameMap.Instance.GetTile(data.Location).TileData.BuildingEntities.ToList();
			try
			{
				BuildingData capturableBuilding = tileEntities.First(x => x.Owner != _playerID);
				if (capturableBuilding != null)
				{
					DynamicButtons.UnitCaptureBuildingButton(ButtonThree, GameMatch.Instance.GetPlayer(_playerID), data, capturableBuilding);
				}
			}
			catch
			{
				//	fail silently if there is no capturable building
			}
		}
	}

	public void Select(BuildingData data)
	{
		if (_lockMouseOver)
		{
			return;
		}

		DisableButtons();

		if (data.Owner == _playerID && BuildingDefinitionObject.ShopBuildings.Contains(data.Definition.BuildingType))
		{
			DynamicButtons.HqBuyButton(ButtonOne, data);
		}
	}

	public void Select(TileData data)
	{
		if (_lockMouseOver)
		{
			return;
		}

		DisableButtons();
	}

	public void Deselect()
	{
		if (_lockMouseOver)
		{
			return;
		}

		DisableButtons();
	}
}
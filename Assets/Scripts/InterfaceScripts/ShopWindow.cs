using System.Collections.Generic;
using System.Linq;
using TacticGameData;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopWindow : MonoBehaviour
{
	public GameObject ShopItemPrefab;
	public Transform ContentRoot;
	public Button CloseButton;
	public Button PurchaseButton;
	public TextMeshProUGUI SelectionInfo;
	public TextMeshProUGUI InfoTextBody;

	public ShopItem SelectedShopItem { get; private set; }

	private List<GameObject> _spawnedPrefabs = new List<GameObject>();

	public delegate void ShopItemPurchaseRequestEvent(ShopItem sender);
	public ShopItemPurchaseRequestEvent ShopItemPurchaseRequest;

	public void Initialize(UnityAction closeAction)
	{
		CloseButton.onClick.RemoveAllListeners();
		CloseButton.onClick.AddListener(() =>
		{
			closeAction();
			DisableShop();
		});

		PurchaseButton.onClick.RemoveAllListeners();
		PurchaseButton.onClick.AddListener(() =>
		{
			ShopItemPurchaseRequest(SelectedShopItem);
		});
	}

	private void Update()
	{
		//	close if clicking outside the UI
		if (gameObject.activeInHierarchy && (Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !PlayerUI.IsMouseOverInterface())
		{
			CloseButton.onClick.Invoke();
		}
	}

	public void PopulateShop(List<UnitDefinition> unitDefinitions)
	{
		int count = 0;
		float size = 0;
		foreach(UnitDefinition unitDefinition in unitDefinitions)
		{
			ShopItem newShopItem = Instantiate(ShopItemPrefab, ContentRoot).GetComponent<ShopItem>();
			size = newShopItem.GetComponent<RectTransform>().sizeDelta.y + 20;
			newShopItem.transform.localPosition = new Vector3(0, -(size) * count);
			_spawnedPrefabs.Add(newShopItem.gameObject);

			UnityAction onClickAction = () =>
			{
				SelectShopItem(newShopItem);
			};

			newShopItem.Initialize(unitDefinition, onClickAction);
			count++;
		}

		ContentRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentRoot.GetComponent<RectTransform>().sizeDelta.x, size * count);

		SelectShopItem(_spawnedPrefabs.First().GetComponent<ShopItem>());
	}

	private void SelectShopItem(ShopItem item)
	{
		UnitDefinition unitDefinition = item.DisplayedUnit;

		SelectionInfo.text = unitDefinition.UnitType.ToString();
		string unitInfo = "";
		unitInfo += string.Format("$: {0} \n", unitDefinition.BasePurchaseCost);
		unitInfo += string.Format("HP: {0} \n", unitDefinition.BaseHealth);
		unitInfo += string.Format("Movement: {0} \n", unitDefinition.BaseMovement);
		unitInfo += string.Format("Damage: {0} \n", unitDefinition.BaseAttackDamage);
		unitInfo += string.Format("Range: {0} \n", unitDefinition.BaseAttackRange);
		unitInfo += string.Format("Attacks: {0} \n", unitDefinition.BaseNumberOfAttacks);

		InfoTextBody.text = unitInfo;

		SelectedShopItem = item;
	}

	public void DisableShop()
	{
		foreach(GameObject g in _spawnedPrefabs)
		{
			Destroy(g);
		}

		_spawnedPrefabs = new List<GameObject>();

		CloseButton.onClick.RemoveAllListeners();

		gameObject.SetActive(false);
	}
}
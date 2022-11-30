using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopWindow : MonoBehaviour
{
	public GameObject ShopItemPrefab;
	public Transform ContentRoot;
	public Button CloseButton;

	private List<GameObject> _spawnedPrefabs = new List<GameObject>();

	public void Initialize(UnityAction closeAction)
	{
		CloseButton.onClick.AddListener(() =>
		{
			closeAction();
			DisableShop();
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
			newShopItem.Initialize(unitDefinition);
			size = newShopItem.GetComponent<RectTransform>().sizeDelta.y + 20;
			newShopItem.transform.localPosition = new Vector3(0, -(size) * count);
			_spawnedPrefabs.Add(newShopItem.gameObject);
			count++;
		}

		ContentRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentRoot.GetComponent<RectTransform>().sizeDelta.x, size * count);
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
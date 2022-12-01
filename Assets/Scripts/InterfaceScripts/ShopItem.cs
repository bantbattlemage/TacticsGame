using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
	public Button ItemButton;
	public TextMeshProUGUI Name;
	public TextMeshProUGUI Cost;
	public Image PreviewImage;

	public UnitDefinition DisplayedUnit { get; private set; }

	public void Initialize(UnitDefinition unit, UnityAction onSelectAction)
	{
		Name.text = unit.UnitType.ToString();
		Cost.text = unit.BasePurchaseCost.ToString("C0");

		ItemButton.onClick.RemoveAllListeners();
		ItemButton.onClick.AddListener(onSelectAction);

		DisplayedUnit = unit;
	}
}

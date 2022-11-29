using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationBox : MonoBehaviour
{
	public Button ButtonOne;
	public Button ButtonTwo;
	public Button ButtonThree;
	public Button ButtonFour;
	public Collider2D Blocker;

	private float _buttonHeight = 50;
	private int _cancelActionIndex = -1;
	private UnityAction[] _buttonActions = null;

	private Button[] _buttons { get { return new Button[] { ButtonOne, ButtonTwo, ButtonThree, ButtonFour }; } }

	private void Awake()
	{
		Disable();
	}

	private void Update()
	{
		if (_buttonActions != null && _cancelActionIndex >= 0)
		{
			if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
			{
				_buttonActions[_cancelActionIndex]();
			}
		}
	}

	public void Disable()
	{
		ButtonOne.onClick.RemoveAllListeners();
		ButtonTwo.onClick.RemoveAllListeners();
		ButtonThree.onClick.RemoveAllListeners();
		ButtonFour.onClick.RemoveAllListeners();

		ButtonOne.gameObject.SetActive(false);
		ButtonTwo.gameObject.SetActive(false);
		ButtonThree.gameObject.SetActive(false);
		ButtonFour.gameObject.SetActive(false);

		Blocker.gameObject.SetActive(false);

		gameObject.GetComponent<Image>().enabled = false;

		_cancelActionIndex = -1;

		_buttonActions = null;
	}

	public void EnableBox(UnityAction[] buttonActions, string[] buttonLabels, int cancelActionIndex)
	{
		if (buttonActions.Length > 4)
		{
			throw new Exception("too many buttons!");
		}

		Disable();

		gameObject.GetComponent<Image>().enabled = true;
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, _buttonHeight * buttonLabels.Length + 20);

		gameObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

		_buttonActions = buttonActions;

		for (int i = 0; i < buttonActions.Length; i++)
		{
			_buttons[i].gameObject.SetActive(true);
			_buttons[i].GetComponent<RectTransform>().localPosition = new Vector3(0, -(_buttonHeight + 10) * i, 0);
			_buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = buttonLabels[i];
			_buttons[i].onClick.AddListener(_buttonActions[i]);
		}

		Blocker.gameObject.SetActive(true);

		_cancelActionIndex = cancelActionIndex;
	}

	public void EnableConfirmationBox(UnityAction confirmAction, UnityAction cancelAction)
	{
		UnityAction[] buttonActions = new UnityAction[] {
			() =>
			{
				confirmAction();
				Disable();
			},
			() =>
			{
				cancelAction();
				Disable();
			}
		};

		string[] buttonNames = new string[] { "Confirm", "Cancel" };
		EnableBox(buttonActions, buttonNames, 1);
	}
}
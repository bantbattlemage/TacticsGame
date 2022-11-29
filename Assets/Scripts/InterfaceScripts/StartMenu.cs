using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
	public Button StartGameButton;

	public void OnStartGameButtonPressed()
	{
		GameController.Instance.StartMatch(2);
		gameObject.SetActive(false);
	}
}
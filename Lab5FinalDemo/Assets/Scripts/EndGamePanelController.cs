using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanelController : MonoBehaviour
{
	public GameObject endGamePanel;

	private void Start()
	{
		endGamePanel.SetActive(false);
	}

	// Method to activate the end game panel
	public void ActivatePanel()
	{
		endGamePanel.SetActive(true);
	}

	// Method to close the game
	public void CloseGame()
	{
#if UNITY_EDITOR
		EditorApplication.ExitPlaymode();
#endif
		Application.Quit();
	}
}

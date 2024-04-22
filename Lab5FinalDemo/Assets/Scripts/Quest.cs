using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Quest
{
	public string description;
	public string enemyType;
	public int requiredKills;
	public int currentKills;
	public bool isActive;
	public bool isCompleted;
	public int xpReward;
	public GameObject questPanel;
	public TMP_Text questText;
	public GameObject completeButton;
	public EndGamePanelController endGamePanelController;

	public void StartQuest(System.Action callback)
	{
		isActive = true;
		questPanel.SetActive(true);
		completeButton.SetActive(false);
		UpdateQuestStatus();
		callback?.Invoke();
	}

	public void CompleteQuest(System.Action callback)
	{
		if (isActive && isCompleted)
		{
			PlayerExperience.Instance.GainXP(xpReward);
			isActive = false;
			isCompleted = true;
			questPanel.SetActive(false);
			callback?.Invoke();

			// Check if it's the boss quest
			if (description == "Kill Ogre")
			{
				endGamePanelController.ActivatePanel();
			}
		}
	}

	public void UpdateQuestStatus()
	{
		if (isActive && !isCompleted)
		{
			questText.text = $"{currentKills} / {requiredKills} {description}";
			if (currentKills >= requiredKills)
			{
				isCompleted = true;
				completeButton.SetActive(true);
			}
		}
	}
}

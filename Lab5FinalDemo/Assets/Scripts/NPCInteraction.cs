using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;

public class NPCInteraction : MonoBehaviour
{
	public InputActionAsset inputActions;
	private InputAction interactAction;

	public GameObject interactionPanel;
	public TMP_Text dialogTextComponent;
	public NPCDialog npcDialog; // Custom class for managing NPC dialog and quest data

	private bool isPlayerNear = false;
	private bool isInteracting = false;

	private void Awake()
	{
		interactionPanel.SetActive(false);
		npcDialog.questButton.SetActive(false);
		interactAction = inputActions.FindActionMap("Gameplay").FindAction("Interact");
	}

	private void OnEnable()
	{
		interactAction.Enable();
		EnemyBehaviour.OnEnemyKilled += HandleEnemyKilled;
		BossBehaviour.OnBossKilled += HandleBossKilled;
	}

	private void OnDisable()
	{
		interactAction.Disable();
		EnemyBehaviour.OnEnemyKilled -= HandleEnemyKilled;
		BossBehaviour.OnBossKilled -= HandleBossKilled;
	}

	private void HandleEnemyKilled()
	{
		UpdateQuestProgress("spider");
	}

	private void HandleBossKilled()
	{
		UpdateQuestProgress("boss");
	}

	private void Update()
	{
		if (isPlayerNear && interactAction.WasPressedThisFrame())
		{
			if (!isInteracting)
			{
				OpenInteractionPanel();
			}
			else
			{
				CloseInteractionPanel();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerNear = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerNear = false;
			CloseInteractionPanel();
		}
	}

	public void OpenInteractionPanel()
	{
		isInteracting = true;
		interactionPanel.SetActive(true);
		Time.timeScale = 0;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		if (npcDialog != null)
		{
			string textToShow = npcDialog.dialogText;
			if (npcDialog.quest.isCompleted)
			{
				textToShow = npcDialog.completedQuestDialog;  // Change the dialogue if the quest is completed
			}
			StartCoroutine(ShowText(textToShow, dialogTextComponent));
		}
	}

	public void CloseInteractionPanel()
	{
		isInteracting = false;
		interactionPanel.SetActive(false);
		Time.timeScale = 1;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private IEnumerator ShowText(string text, TMP_Text textComponent)
	{
		textComponent.text = "";

		foreach (char c in text)
		{
			textComponent.text += c;
			yield return new WaitForSecondsRealtime(0.04f);
		}
		// Check and display buttons after text is shown
		CheckAndDisplayButtons();
	}

	private void CheckAndDisplayButtons()
	{
		if (npcDialog.isQuestGiver)
		{
			npcDialog.questButton.SetActive(!npcDialog.quest.isActive && !npcDialog.quest.isCompleted);
			if (!npcDialog.quest.isActive && !npcDialog.quest.isCompleted)
			{
				npcDialog.questButton.GetComponent<Button>().onClick.RemoveAllListeners();
				npcDialog.questButton.GetComponent<Button>().onClick.AddListener(() => npcDialog.quest.StartQuest(CloseInteractionPanel));
			}

			if (npcDialog.quest.isActive && npcDialog.quest.isCompleted)
			{
				npcDialog.quest.completeButton.SetActive(true);
				npcDialog.quest.completeButton.GetComponent<Button>().onClick.RemoveAllListeners();
				npcDialog.quest.completeButton.GetComponent<Button>().onClick.AddListener(() => npcDialog.quest.CompleteQuest(CloseInteractionPanel));
			}
			else
			{
				npcDialog.quest.completeButton.SetActive(false);
			}
		}
	}

	private void UpdateQuestProgress(string enemyType)
	{
		if (npcDialog.quest.isActive && !npcDialog.quest.isCompleted)
		{
			if (npcDialog.quest.enemyType == enemyType)
			{
				npcDialog.quest.currentKills++;
				npcDialog.quest.UpdateQuestStatus();
			}
		}
	}

	public void StartQuest()
	{
		if (!npcDialog.quest.isActive && !npcDialog.quest.isCompleted)
		{
			npcDialog.quest.isActive = true;
			npcDialog.quest.questPanel.SetActive(true);
			npcDialog.quest.completeButton.SetActive(false);
			CloseInteractionPanel();
		}
	}

	public void CompleteQuest(Quest quest)
	{
		if (quest.isActive && quest.isCompleted)
		{
			PlayerExperience.Instance.GainXP(quest.xpReward);
			quest.isActive = false;
			quest.isCompleted = true;
			quest.questPanel.SetActive(false);
			CloseInteractionPanel();
		}
	}
}

[System.Serializable]
public class NPCDialog
{
	public string dialogText;
	public string completedQuestDialog;
	public bool isQuestGiver;
	public GameObject questButton;
	public Quest quest;
}

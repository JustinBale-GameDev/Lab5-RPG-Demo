using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
	public InputActionAsset inputActions;
	private InputAction interactAction;

	public GameObject interactionPanel;
	public TMP_Text dialogTextComponent; // Assign this in the inspector
	public NPCDialog npcDialog; // Assign dialog and quest details in the inspector

	private bool isPlayerNear = false;
	private bool isInteracting = false;

	private void Awake()
	{
		interactionPanel.SetActive(false);
		npcDialog.questButton.SetActive(false);
		interactAction = inputActions.FindActionMap("Gameplay").FindAction("Interact");
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
		if (PlayerMovement.Instance != null && ThirdPersonCam.Instance != null)
		{
			Time.timeScale = 0;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			//PlayerMovement.Instance.enabled = false;
			//ThirdPersonCam.Instance.enabled = false;
		}
		if (npcDialog != null)
		{
			StartCoroutine(ShowText(npcDialog.dialogText, dialogTextComponent));
			
		}
	}
	public void CloseInteractionPanel()
	{
		isInteracting = false;
		interactionPanel.SetActive(false);
		if (PlayerMovement.Instance != null && ThirdPersonCam.Instance != null)
		{
			Time.timeScale = 1;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			//PlayerMovement.Instance.enabled = true;
			//ThirdPersonCam.Instance.enabled = true;
		}
	}

	private IEnumerator ShowText(string text, TMP_Text textComponent)
	{
		textComponent.text = "";

		foreach (char c in text)
		{
			textComponent.text += c;
			yield return new WaitForSecondsRealtime(0.02f);
		}

		if (npcDialog.isQuestGiver)
		{
			npcDialog.questButton.SetActive(true); // Show quest button if NPC is a quest giver
		}
		else
		{
			npcDialog.questButton.SetActive(false);
		}
	}

	private void OnEnable()
	{
		interactAction.Enable();
	}

	private void OnDisable()
	{
		interactAction.Disable();
	}
}


[System.Serializable]
public class NPCDialog
{
	public string dialogText;
	public bool isQuestGiver;
	public GameObject questButton; // This button can be enabled if the NPC is a quest giver
}

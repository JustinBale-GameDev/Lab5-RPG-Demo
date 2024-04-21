using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteractionNoQuest : MonoBehaviour
{
	public InputActionAsset inputActions;
	private InputAction interactAction;

	public GameObject interactionPanel;
	public TMP_Text dialogTextComponent;
	public NPCDialogNoQuest npcDialog; // Custom class for managing NPC dialog and quest data

	private bool isPlayerNear = false;
	private bool isInteracting = false;

	private void Awake()
	{
		interactionPanel.SetActive(false);
		interactAction = inputActions.FindActionMap("Gameplay").FindAction("Interact");
	}

	private void OnEnable()
	{
		interactAction.Enable();
	}

	private void OnDisable()
	{
		interactAction.Disable();
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

		if (isPlayerNear)
		{
			// Rotate towards the player
			Vector3 direction = (PlayerMovement.Instance.transform.position - transform.position).normalized;
			// Ignore Y-axis differences to keep the rotation strictly horizontal
			direction.y = 0;
			if (direction != Vector3.zero) // Check to avoid "Look rotation viewing vector is zero" error
			{
				Quaternion lookRotation = Quaternion.LookRotation(direction);
				transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Smooth rotation
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
	}
}
[System.Serializable]
public class NPCDialogNoQuest
{
	public string dialogText;
}

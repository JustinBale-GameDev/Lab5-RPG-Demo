using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
	[SerializeField] 
	private InputActionAsset inputActions;
    private InputAction pauseAction;

	[SerializeField]
	PlayerController playerController;

	[SerializeField]
	GameObject pauseMenu;

	bool isPaused = false;

	private void Awake()
	{
		pauseAction = inputActions.FindActionMap("Gameplay").FindAction("PauseMenu");
	}

	void Start()
    {
		isPaused = false;
	}

    // Update is called once per frame
    void Update()
    {
		if (pauseAction.WasPressedThisFrame())
		{
			
			isPaused = !isPaused;
			if (isPaused)
			{
				Debug.Log("Paused");
				Time.timeScale = 0;
				playerController.enabled = false;

				//Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else
			{
				Debug.Log("Un-Paused");
				Time.timeScale = 1;
				playerController.enabled = true;

				//Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		pauseMenu.SetActive(isPaused);
	}
	public void Resume_Button()
	{
		isPaused = !isPaused;
		if (isPaused)
		{
			Time.timeScale = 0;
			playerController.enabled = false;
		}
		else
		{
			Time.timeScale = 1;
			playerController.enabled = true;
		}
	}

	public void Quit_Button()
	{
		Application.Quit();
	}

	private void OnEnable()
	{
		pauseAction.Enable();
	}

	private void OnDisable()
	{
		pauseAction.Disable();
	}
}

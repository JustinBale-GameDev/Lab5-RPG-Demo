using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public InputActionAsset inputActions;
	private InputAction pauseAction;
	public PlayerMovement playerMovement;
	public GameObject pauseMenu;
	public AudioMixer audioMixer;
	bool isPaused = false;

	public GameObject controls;

	private void Awake()
	{
		pauseAction = inputActions.FindActionMap("Gameplay").FindAction("PauseMenu");
	}

	void Start()
	{
		Time.timeScale = 1;
		isPaused = false;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (pauseAction.WasPressedThisFrame())
		{

			isPaused = !isPaused;
			if (isPaused)
			{
				Time.timeScale = 0;
				playerMovement.enabled = false;

				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else
			{
				Time.timeScale = 1;
				playerMovement.enabled = true;

				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				Close_Controls_Button();
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
			playerMovement.enabled = false;
		}
		else
		{
			Time.timeScale = 1;
			playerMovement.enabled = true;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			Close_Controls_Button();
		}
	}

	public void Controls_Button()
	{
		controls.SetActive(true);
	}
	public void Close_Controls_Button()
	{
		controls.SetActive(false);
	}

	public void RestartDemo()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene("LevelOne");
	}

	public void Quit_Button()
	{
#if UNITY_EDITOR
		EditorApplication.ExitPlaymode();
#endif
		Application.Quit();
	}

	public void MusicVolumeUpdate(float volume)
	{
		audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
	}
	public void SFXVolumeUpdate(float volume)
	{
		audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
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

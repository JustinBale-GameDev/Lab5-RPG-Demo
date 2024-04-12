using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlayerScene : MonoBehaviour
{
	private void Awake()
	{
		if (!SceneManager.GetSceneByName("PlayerScene").isLoaded)
		{
			SceneManager.LoadScene("PlayerScene", LoadSceneMode.Additive);
		}
	}
}

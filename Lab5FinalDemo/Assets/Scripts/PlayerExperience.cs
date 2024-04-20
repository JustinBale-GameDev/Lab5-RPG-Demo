using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
	public static PlayerExperience Instance { get; private set; }

	public Image healthUI;
	public Image attack2IconCover;
	public Image attack3IconCover;
	public TMP_Text xpText;
	public TMP_Text levelText;
	public float maxXP;
	private float currentXP;
	public int playerLevel = 1;

	PlayerMovement playerMovement;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		playerMovement = GetComponent<PlayerMovement>();
		currentXP = 0;
		UpdateXPUI();
		UpdateLevelUI();
	}

	public void GainXP(float xpAmount)
	{
		currentXP += xpAmount;

		// Check if the current XP has reached or exceeded the max XP
		if (currentXP >= maxXP)
		{
			LevelUp();
		}

		UpdateXPUI();
	}

	private void LevelUp()
	{
		currentXP -= maxXP; // Subtract maxXP to handle overflow
		playerLevel++; // Increase level
		maxXP += 500; // Increase the maxXP for the next level
		UpdateLevelUI(); // Update the level UI text

		playerMovement.UpdatePlayerLevel(); // Update playerMovement script to allow for new attack

		if (playerLevel >= 2)
		{
			attack2IconCover.enabled = false;
		}
		if (playerLevel >= 3)
		{
			attack3IconCover.enabled = false;
		}
	}

	private void UpdateXPUI()
	{
		// Update the scale of the health bar
		float xpBarScale = Mathf.Clamp(currentXP / maxXP, 0, 1);
		healthUI.transform.localScale = new Vector3(xpBarScale, 1, 1);

		// Update the XP text to show the current XP and the max XP
		xpText.text = $"{currentXP} / {maxXP} XP";
	}

	private void UpdateLevelUI()
	{
		levelText.text = $"{playerLevel}";
	}
}

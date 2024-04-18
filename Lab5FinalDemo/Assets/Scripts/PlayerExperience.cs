using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
	public Image healthUI;
	public TMP_Text xpText;
	public float maxXP;
	private float currentXP;

	// Start is called before the first frame update
	void Start()
	{
		currentXP = 0;
		UpdateXPUI();
	}

	public void GainXP(float xpAmount)
	{
		currentXP += xpAmount;
		currentXP = Mathf.Min(currentXP, maxXP);  // Cap currentXP at maxXP to avoid going over
		UpdateXPUI();
	}

	private void UpdateXPUI()
	{
		// Update the scale of the health bar
		float xpBarScale = Mathf.Clamp(currentXP / maxXP, 0, 1);
		healthUI.transform.localScale = new Vector3(xpBarScale, 1, 1);

		// Update the XP text to show the current XP and the max XP
		xpText.text = $"{currentXP} / {maxXP} XP";
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	public Image healthUI;
	public float maxHealth;
	[SerializeField] private float currentHealth;
	public bool isDead;

	// Start is called before the first frame update
	void Start()
    {
		currentHealth = maxHealth;
		isDead = false;
	}

	public void ApplyDamage(float damage)
	{
		Debug.Log("Damage applied");
		currentHealth -= damage; // Subtract damage from current health
		currentHealth = Mathf.Max(currentHealth, 0); // Current health does not drop below 0
		float healthScale = Mathf.Clamp(1 - currentHealth / maxHealth, 0, 1); // Update the health UI scale based on the current health proportion to max health
		healthUI.transform.localScale = new Vector3(healthScale, 1, 1);

		// Check if health has dropped to 0 or below
		if (currentHealth <= 0 && !isDead)
		{
			isDead = true;
		}
	}
}

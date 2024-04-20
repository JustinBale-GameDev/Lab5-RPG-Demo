using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
	private Animator animator;
	public GameObject healthCanvas;
	public Image healthUI;

	public int maxHealth;
	private int currentHealth;
	public float distanceToNoticePlayer = 15f;
	public int xpGainOnKill;

	private bool isDead = false;

	void Start()
	{
		maxHealth = 50;
		currentHealth = maxHealth;
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (!isDead)
		{
			// Check the distance to the player and play animation if within range
			float distanceToPlayer = Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position);
			if (distanceToPlayer <= distanceToNoticePlayer && !isDead)
			{
				// Rotate towards the player
				Vector3 direction = (transform.position - PlayerMovement.Instance.transform.position).normalized;
				// Ignore Y-axis differences to keep the rotation strictly horizontal
				direction.y = 0;
				if (direction != Vector3.zero) // Check to avoid "Look rotation viewing vector is zero" error
				{
					Quaternion lookRotation = Quaternion.LookRotation(direction);
					transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Smooth rotation
				}
			}
		}
	}

	public void ApplyDamage(int damage)
	{
		currentHealth -= damage;
		currentHealth = Mathf.Max(currentHealth, 0); // Prevent negative health values
		healthUI.transform.localScale = new Vector3((float)currentHealth / maxHealth, 1, 1);

		if (currentHealth <= 0 && !isDead)
		{
			Die();
		}
	}

	void Die()
	{
		isDead = true;
		animator.SetBool("isDead", true); // Death animation
		healthCanvas.SetActive(false); // Disable health bar

		// Gain player XP
		if (PlayerExperience.Instance != null)
		{
			PlayerExperience.Instance.GainXP(xpGainOnKill);
		}
	}
}

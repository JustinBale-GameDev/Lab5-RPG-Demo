using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
	public static event Action OnEnemyKilled;
	private Animator animator;
	public GameObject healthCanvas;
	public Image healthUI;

	public int maxHealth;
	private int currentHealth;
	public float distanceToNoticePlayer = 15f;
	public int xpGainOnKill;

	private bool isDead = false;

	public float attackDistance = 5f;
	public float attackCooldown = 7f;
	private float lastAttackTime = -7f;

	public SpiderDamage spiderDamage;
	public AudioSource deathSound;

	void Start()
	{
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
				if (direction != Vector3.zero)
				{
					Quaternion lookRotation = Quaternion.LookRotation(direction);
					transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Smooth rotation
				}

				// Attack if close enough and cooldown has passed
				if (distanceToPlayer <= attackDistance && Time.time > lastAttackTime + attackCooldown)
				{
					spiderDamage.AllowDamage();
					StartCoroutine(PerformAttack());
				}
			}
		}
	}

	private IEnumerator PerformAttack()
	{
		lastAttackTime = Time.time;
		animator.SetBool("attack", true);
		yield return new WaitForSeconds(1);
		animator.SetBool("attack", false);
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
		if (deathSound != null) // play death sound
		{
			deathSound.Play();
		}
		PlayerExperience.Instance.GainXP(xpGainOnKill); // Gain player XP
		OnEnemyKilled?.Invoke(); // Notify spider quest an enemy was killed
	}
}

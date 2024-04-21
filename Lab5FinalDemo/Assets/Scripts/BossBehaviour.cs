using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBehaviour : MonoBehaviour
{
	public static Action OnBossKilled;
	private Animator animator;
	public GameObject healthCanvas;
	public Image healthUI;

	public int xpGainOnKill;
	public int maxHealth;
	private int currentHealth;
	public float distanceToNoticePlayer = 15f;
	public float attackDistance = 5f;
	public float attackCooldown = 7f;
	private float lastAttackTime = -7f;
	[SerializeField] private int attackCounter = 0; // Counter to manage attack sequence

	private bool isDead = false;
	private bool isAttacking = false;

	public BossWeaponDamage bossWeaponDamage;


	void Start()
	{
		currentHealth = maxHealth;
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (!isDead)
		{
			float distanceToPlayer = Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position);
			if (distanceToPlayer <= distanceToNoticePlayer)
			{
				Vector3 direction = (PlayerMovement.Instance.transform.position - transform.position).normalized;
				direction.y = 0;
				if (direction != Vector3.zero)
				{
					Quaternion lookRotation = Quaternion.LookRotation(direction);
					transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
				}

				// Attack if close enough and cooldown has passed
				if (distanceToPlayer <= attackDistance && Time.time > lastAttackTime + attackCooldown)
				{
					StartCoroutine(PerformAttack());
				}
			}
		}
	}

	private IEnumerator PerformAttack()
	{
		isAttacking = true;
		lastAttackTime = Time.time;
		bossWeaponDamage.AllowDamage();

		if (attackCounter < 2)
		{
			animator.SetBool("attack1", true);
			bossWeaponDamage.SetDamage(false); // Normal damage for attack1
			attackCounter++;

			yield return new WaitForSeconds(1);
		}
		else
		{
			animator.SetBool("attack2", true);
			bossWeaponDamage.SetDamage(true); // Increased damage for attack2
			attackCounter = 0; // Reset the counter after attack2

			yield return new WaitForSeconds(2);
		}

		animator.SetBool("attack1", false);
		animator.SetBool("attack2", false);

		isAttacking = false;
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
		animator.SetBool("death", true);
		healthCanvas.SetActive(false);
		PlayerExperience.Instance.GainXP(xpGainOnKill);
		OnBossKilled?.Invoke();
	}
}

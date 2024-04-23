using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossBehaviour : MonoBehaviour
{
	public static Action OnBossKilled;
	private Animator animator;
	public GameObject healthCanvas;
	public Image healthUI;
	public NavMeshAgent agent;

	public int xpGainOnKill;
	public int maxHealth;
	private int currentHealth;
	//public float distanceToNoticePlayer = 10f;
	//public float followDistance = 25f;
	public float noticeDistance;
	public float attackDistance = 5f;
	public float attackCooldown = 7f;
	private float lastAttackTime = -7f;
	[SerializeField] private int attackCounter = 0; // Counter to manage attack sequence

	private Vector3 startPosition;

	private bool isDead = false;
	private bool isAttacking = false;

	public BossWeaponDamage bossWeaponDamage;

	public AudioSource attack1Sound;
	public AudioSource attack2Sound;
	public AudioSource deathSound;


	void Start()
	{
		currentHealth = maxHealth;
		animator = GetComponent<Animator>();
		startPosition = transform.position; // Store the initial position
		agent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		if (!isDead)
		{
			float distanceToPlayer = Vector3.Distance(PlayerMovement.Instance.transform.position, transform.position);
			float distanceFromStartToPlayer = Vector3.Distance(PlayerMovement.Instance.transform.position, startPosition);

			// Behavior based on distance from the start position to the player
			if (distanceFromStartToPlayer <= noticeDistance)
			{
				MoveTowardsPlayerToAttack(distanceToPlayer);
			}
			else
			{
				ReturnToStart();
			}
		}

		if (currentHealth <= 0 && !isDead)
		{
			Die();
		}
	}

	void MoveTowardsPlayerToAttack(float distanceToPlayer)
	{
		if (distanceToPlayer > attackDistance && !isAttacking)
		{
			agent.isStopped = false;
			agent.SetDestination(PlayerMovement.Instance.transform.position);
			animator.SetBool("isMoving", true);
		}
		else
		{
			agent.isStopped = true;
			animator.SetBool("isMoving", false);

			// Handle rotation towards the player
			Vector3 direction = (PlayerMovement.Instance.transform.position - transform.position).normalized;
			Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 50f);

			// Rotate towards the player just before attacking
			if (Time.time > lastAttackTime + attackCooldown && !isAttacking)
			{
				bossWeaponDamage.AllowDamage();
				StartCoroutine(PerformAttack());
			}
		}
	}

	void ReturnToStart()
	{
		if (Vector3.Distance(transform.position, startPosition) > 1f)
		{
			agent.isStopped = false;
			agent.SetDestination(startPosition);
			animator.SetBool("isMoving", true);
		}
		else
		{
			agent.isStopped = true;
			animator.SetBool("isMoving", false);
		}
	}

	private IEnumerator PerformAttack()
	{
		isAttacking = true;
		lastAttackTime = Time.time;
		agent.isStopped = true;
		animator.SetBool("isMoving", false);

		if (attackCounter < 2)
		{
			if (attack1Sound != null)
			{
				attack1Sound.Play();
			}
			animator.SetBool("attack1", true);
			bossWeaponDamage.SetDamage(false); // Normal damage for attack1
			attackCounter++;

			yield return new WaitForSeconds(1);
		}
		else
		{
			if (attack2Sound != null)
			{
				attack2Sound.Play();
			}
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

		animator.SetBool("hit", true);
		StartCoroutine(DelayHitAnimationBool());
	}

	private IEnumerator DelayHitAnimationBool()
	{
		yield return new WaitForSeconds(0.5f);
		animator.SetBool("hit", false);
	}

	void Die()
	{
		isDead = true;
		if (deathSound != null)
		{
			deathSound.Play();
		}
		animator.SetBool("death", true);
		healthCanvas.SetActive(false);
		PlayerExperience.Instance.GainXP(xpGainOnKill);
		OnBossKilled?.Invoke();
	}
}

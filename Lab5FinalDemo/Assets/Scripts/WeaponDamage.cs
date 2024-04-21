using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
	public int baseDamageAmount; // Default damage for normal attack
	public int strongAttackDamageAmount; // Increased damage for strong attack
	private int currentDamage;
	private bool canDamage = false;

	private void Awake()
	{
		currentDamage = baseDamageAmount;  // Initialize currentDamage with base damage
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy") && canDamage)
		{
			EnemyBehaviour enemy = other.GetComponent<EnemyBehaviour>();
			if (enemy != null)
			{
				enemy.ApplyDamage(currentDamage);
			}
			canDamage = false; // Prevent multiple damage applications
		}

		if (other.CompareTag("Boss") && canDamage)
		{
			BossBehaviour boss = other.GetComponent<BossBehaviour>();
			if (boss != null)
			{
				boss.ApplyDamage(currentDamage);
				
			}
			canDamage = false; // Prevent multiple damage applications
		}
	}

	// Call when the attack animation starts
	public void AllowDamage(bool isStrongAttack)
	{
		canDamage = true;
		currentDamage = isStrongAttack ? strongAttackDamageAmount : baseDamageAmount;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
	public int damageAmount;
	private bool canDamage = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy") && canDamage)
		{
			EnemyBehaviour enemy = other.GetComponent<EnemyBehaviour>();
			if (enemy != null)
			{
				enemy.ApplyDamage(damageAmount);
			}
			canDamage = false; // Prevent multiple damage applications
		}

		if (other.CompareTag("Boss") && canDamage)
		{
			BossBehaviour boss = other.GetComponent<BossBehaviour>();
			if (boss != null)
			{
				boss.ApplyDamage(damageAmount);
				
			}
			canDamage = false; // Prevent multiple damage applications
		}
	}

	// Call when the attack animation starts
	public void AllowDamage()
	{
		canDamage = true;
	}
}

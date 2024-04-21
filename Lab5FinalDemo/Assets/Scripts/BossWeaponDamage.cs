using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeaponDamage : MonoBehaviour
{
	public int baseDamageAmount;
	private int damageAmount;
	private bool canDamage = false;

	private void Awake()
	{
		damageAmount = baseDamageAmount;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && canDamage)
		{
			Debug.Log("Player hit");
			PlayerHealth.Instance.ApplyDamage(damageAmount);
			//PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
			//if (playerHealth != null)
			//{
			//	playerHealth.ApplyDamage(damageAmount);
			//}
			canDamage = false; // Prevent multiple damage applications
			//StartCoroutine(ResetDamage());
		}
	}

	// Call when the attack animation starts
	public void AllowDamage()
	{
		canDamage = true;
	}

	// Method to set damage based on attack type
	public void SetDamage(bool isStrongAttack)
	{
		if (isStrongAttack)
		{
			damageAmount = baseDamageAmount + 10; // Increase damage for strong attack
		}
		else
		{
			damageAmount = baseDamageAmount; // Normal damage
		}
	}
}

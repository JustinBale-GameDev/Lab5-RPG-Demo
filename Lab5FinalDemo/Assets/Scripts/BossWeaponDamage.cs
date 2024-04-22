using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeaponDamage : MonoBehaviour
{
	public int baseDamageAmount;
	private int damageAmount;
	[SerializeField] private bool canDamage = false;

	public ParticleSystem weaponHitEffect;

	private void Awake()
	{
		damageAmount = baseDamageAmount;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && canDamage)
		{
			PlayerHealth.Instance.ApplyDamage(damageAmount);
			canDamage = false; // Prevent multiple damage applications

			// Play hit effect
			if (weaponHitEffect != null)
			{
				weaponHitEffect.Play();
			}
		}
	}

	// Call when the attack animation starts
	public void AllowDamage()
	{
		canDamage = true;
		StartCoroutine(ResetCanDamage());
	}

	private IEnumerator ResetCanDamage()
	{
		yield return new WaitForSeconds(1.5f);
		canDamage = false;
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

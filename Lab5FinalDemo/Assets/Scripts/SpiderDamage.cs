using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderDamage : MonoBehaviour
{
	public int damageAmount;
	private bool canDamage = false;

	public AudioSource attacksound;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && canDamage)
		{
			if (attacksound != null )
			{
				attacksound.Play();
			}
			PlayerHealth.Instance.ApplyDamage(damageAmount);
			canDamage = false; // Prevent multiple damage applications
		}
	}

	// Call when the attack animation starts
	public void AllowDamage()
	{
		canDamage = true;
	}
}

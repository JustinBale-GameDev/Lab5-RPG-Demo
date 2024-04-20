using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
	public int damageAmount = 10;
	private bool canDamage = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy") && canDamage)
		{
			Debug.Log("Weapon hit!");
			EnemyBehaviour enemy = other.GetComponent<EnemyBehaviour>();
			if (enemy != null)
			{
				enemy.ApplyDamage(damageAmount);
			}
			canDamage = false; // Prevent multiple damage applications
		}
	}

	//private void OnCollisionEnter(Collision collision)
	//{
	//	if (collision.gameObject.CompareTag("Enemy") && canDamage)
	//	{
	//		Debug.Log("Weapon hit!");
	//		EnemyBehaviour enemy = collision.gameObject.GetComponent<EnemyBehaviour>();
	//		if (enemy != null)
	//		{
	//			enemy.ApplyDamage(damageAmount);
	//		}
	//		canDamage = false; // Prevent multiple damage applications
	//	}
	//}

	// Call this when the attack animation starts
	public void AllowDamage()
	{
		canDamage = true;
	}
}

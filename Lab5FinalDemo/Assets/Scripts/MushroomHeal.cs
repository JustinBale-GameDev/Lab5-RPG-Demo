using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomHeal : MonoBehaviour
{
	public int healAmount;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("Player hit");
			PlayerHealth.Instance.Heal(healAmount);
			gameObject.SetActive(false);
		}
	}
}

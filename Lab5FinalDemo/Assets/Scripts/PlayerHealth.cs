using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	public static PlayerHealth Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public Image healthUI;
	public float maxHealth;
	[SerializeField] private float currentHealth;
	public bool isDead;
	public float regenRate; // Health per second

	public AudioSource healthSound;
	public ParticleSystem healthParticles;

	public GameObject deathPanel;
	public AudioSource deathSound;
	public Animator animator;

	// Start is called before the first frame update
	void Start()
    {
		deathPanel.SetActive(false);

		currentHealth = maxHealth;
		isDead = false;
		UpdateHealthUI();
	}

	void Update()
	{
		if (!isDead && currentHealth < maxHealth)
		{
			RegenerateHealth();
		}
	}

	private void RegenerateHealth()
	{
		currentHealth += regenRate * Time.deltaTime; // health per second
		currentHealth = Mathf.Min(currentHealth, maxHealth); // health does not exceed maxHealth
		UpdateHealthUI();
	}

	public void ApplyDamage(float damage)
	{
		currentHealth -= damage;
		currentHealth = Mathf.Max(currentHealth, 0);
		UpdateHealthUI();

		// Check if health has dropped to 0 or below
		if (currentHealth <= 0 && !isDead)
		{
			isDead = true;
			StartCoroutine(Die());
		}
	}

	private IEnumerator Die()
	{
		// Disable player movement immediately
		PlayerMovement.Instance.DisablePlayerControls();

		// Play death sound
		deathSound.Play();

		// Play death animation
		animator.SetBool("isDead", true);

		// Wait for the animation and sound to finish
		yield return new WaitForSeconds(deathSound.clip.length);

		// Enable death screen
		deathPanel.SetActive(true);

		// Stop game time
		Time.timeScale = 0;

		// Enable cursor
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void UpdateHealthUI()
	{
		float healthScale = Mathf.Clamp(1 - currentHealth / maxHealth, 0, 1);
		healthUI.transform.localScale = new Vector3(healthScale, 1, 1);
	}

	public void Heal(int healthGain)
	{
		currentHealth += healthGain;
		currentHealth = Mathf.Min(currentHealth, maxHealth);
		UpdateHealthUI();

		//play sound
		if (healthSound != null)
		{
			healthSound.Play();
		}

		//play effect
		if (healthParticles != null)
		{
			healthParticles.Play();
		}
	}
}

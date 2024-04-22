using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
	public static PlayerMovement Instance { get; private set; }

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
		moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
		attackAction1 = inputActions.FindActionMap("Gameplay").FindAction("Attack1");
		attackAction2 = inputActions.FindActionMap("Gameplay").FindAction("Attack2");
	}

	[Header("UI Elements")]
	public Image attack1CooldownOverlay;
	public Image attack2CooldownOverlay;

	[Header("Movement")]
	public float moveSpeed;
	Vector2 currentMovementInput;
	Vector3 moveDirection;
	public Transform orientation;

	[Header("Input Actions")]
	public InputActionAsset inputActions;
	private InputAction moveAction, attackAction1, attackAction2;

	[Header("Sound")]
	public AudioSource walkingSound;
	private bool isWalking = false;
	public AudioSource attackSound;

	[Header("Attacking")]
	public WeaponDamage weaponDamage;
	private bool isAttackInitiated = false;
	private bool isAttack2OnCooldown = false;
	public float attackDuration;
	private float attackStartTime;
	public ParticleSystem attack1Effect1;
	public ParticleSystem attack1Effect2;
	public ParticleSystem attack1Effect3;

	public Animator animator;
	Rigidbody rb;
	public int playerLevel = 1;


	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;

		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		currentMovementInput = moveAction.ReadValue<Vector2>();
		SpeedControl();
		bool isIdle = IsIdle();

		animator.SetBool("isMoving", !isIdle);

		// Handle walking sound
		HandleWalkingSound(!isIdle);

		HandleAttackInput();
	}

	private void FixedUpdate()
	{
		MovePlayer();
	}

	private void MovePlayer()
	{
		// Calculate movement direction using the orientation
		moveDirection = orientation.forward * currentMovementInput.y + orientation.right * currentMovementInput.x;
		rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
	}

	public bool IsIdle()
	{
		return currentMovementInput.sqrMagnitude < 0.01;
	}

	private void SpeedControl()
	{
		Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);
		if (flatVel.magnitude > moveSpeed) // limit velocity
		{
			Vector3 limitedVel = flatVel.normalized * moveSpeed;
			rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
		}
	}

	private void HandleWalkingSound(bool isCurrentlyWalking)
	{
		if (isCurrentlyWalking && !isWalking)
		{
			walkingSound.Play();
			isWalking = true;
		}
		else if (!isCurrentlyWalking && isWalking)
		{
			walkingSound.Stop();
			isWalking = false;
		}
	}

	private void HandleAttackInput()
	{
		if (!isAttackInitiated)
		{
			if (attackAction1.WasPressedThisFrame())
			{
				InitiateAttack(false);  // false for normal attack
			}
			else if (playerLevel >= 2 && attackAction2.WasPressedThisFrame() && !isAttack2OnCooldown)
			{
				InitiateAttack(true);  // true for strong attack
				StartCoroutine(Attack2Cooldown(5f)); // Start cooldown coroutine for attack 2
			}
		}

		if (isAttackInitiated && Time.time > attackStartTime + attackDuration)
		{
			EndAttack();
		}
	}

	private void InitiateAttack(bool isStrongAttack)
	{
		weaponDamage.AllowDamage(isStrongAttack);
		moveAction.Disable();
		isAttackInitiated = true;
		animator.SetBool(isStrongAttack ? "isAttacking2" : "isAttacking1", true);
		attackStartTime = Time.time;

		// Play the particle effect when the attack is initiated
		StartCoroutine(AttackParticleDelay());

		//Play attack sound
		attackSound.Play();

		if (isStrongAttack)
		{
			StartCoroutine(AttackCooldown(attack2CooldownOverlay, 5f));  // Cooldown for strong attack
		}
		else
		{
			StartCoroutine(AttackCooldown(attack1CooldownOverlay, attackDuration));  // Cooldown for normal attack
		}
	}

	private void EndAttack()
	{
		isAttackInitiated = false;
		animator.SetBool("isAttacking1", false);
		animator.SetBool("isAttacking2", false);  // reset both attack bools
		moveAction.Enable();
	}

	private IEnumerator AttackParticleDelay()
	{
		yield return new WaitForSeconds(0.3f);
		attack1Effect1.Play();
		attack1Effect2.Play();
		attack1Effect3.Play();
	}

	// cooldown effect for attack 1
	private IEnumerator AttackCooldown(Image cooldownOverlay, float duration)
	{
		Vector3 startScale = new Vector3(1, 1, 1); // full scale
		Vector3 endScale = new Vector3(1, 0, 1); // zero y scale
		float timePassed = 0f;

		// Set the starting scale
		cooldownOverlay.transform.localScale = startScale;

		while (timePassed < duration)
		{
			timePassed += Time.deltaTime;
			float scaleProgress = 1 - (timePassed / duration);
			cooldownOverlay.transform.localScale = new Vector3(1, scaleProgress, 1); // Lerp the scale
			yield return null;
		}

		// Set the scale to zero to ensure it's fully closed at the end of the cooldown
		cooldownOverlay.transform.localScale = endScale;
	}

	// New coroutine for attack 2 cooldown
	private IEnumerator Attack2Cooldown(float cooldownTime)
	{
		isAttack2OnCooldown = true;
		attack2CooldownOverlay.gameObject.SetActive(true);
		attack2CooldownOverlay.transform.localScale = new Vector3(1, 1, 1); // Full scale

		float timePassed = 0f;
		while (timePassed < cooldownTime)
		{
			timePassed += Time.deltaTime;
			float scaleProgress = 1 - (timePassed / cooldownTime);
			attack2CooldownOverlay.transform.localScale = new Vector3(1, scaleProgress, 1); // Lerp the scale
			yield return null;
		}

		attack2CooldownOverlay.transform.localScale = new Vector3(1, 0, 1); // Set scale to zero
		attack2CooldownOverlay.gameObject.SetActive(false);
		isAttack2OnCooldown = false;
	}

	public void UpdatePlayerLevel()
	{
		playerLevel++;
	}

	public void DisablePlayerControls()
	{
		moveAction.Disable();
		attackAction1.Disable();
		attackAction2.Disable();
	}

	private void OnEnable()
	{
		moveAction.Enable();
		attackAction1.Enable();
		attackAction2.Enable();
	}

	private void OnDisable()
	{
		moveAction.Disable();
		attackAction1.Disable();
		attackAction2.Disable();
	}
}

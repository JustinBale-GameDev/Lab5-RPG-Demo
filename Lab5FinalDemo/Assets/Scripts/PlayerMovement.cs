using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement")]
	public float moveSpeed;
	Vector2 currentMovementInput;
	Vector3 moveDirection;

	[Header("Input Actions")]
	public InputActionAsset inputActions;
	private InputAction moveAction, attackAction;

	public Transform orientation;
	private Animator animator;
	Rigidbody rb;

	private bool isAttackInitiated = false;
	public float attackDuration;
	private float attackStartTime;

	private void Awake()
	{
		moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
		attackAction = inputActions.FindActionMap("Gameplay").FindAction("Attack");
	}

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

	private void HandleAttackInput()
	{
		// Initiate attack
		if (attackAction.WasPressedThisFrame() && !isAttackInitiated)
		{
			isAttackInitiated = true;
			animator.SetBool("isAttacking", true);
			attackStartTime = Time.time;
		}

		// Check if the attack animation duration has passed
		if (isAttackInitiated && Time.time > attackStartTime + attackDuration)
		{
			isAttackInitiated = false;
			animator.SetBool("isAttacking", false);
		}
	}

	private void OnEnable()
	{
		moveAction.Enable();
		attackAction.Enable();
	}

	private void OnDisable()
	{
		moveAction.Disable();
		attackAction.Disable();
	}
}

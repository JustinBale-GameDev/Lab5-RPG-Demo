using System.Collections;
using System.Collections.Generic;
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
	private InputAction moveAction;

	public Transform orientation;
	Rigidbody rb;

	private void Awake()
	{
		moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
	}

	private void Update()
	{
		currentMovementInput = moveAction.ReadValue<Vector2>();
		SpeedControl();
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

	private void SpeedControl()
	{
		Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);
		if (flatVel.magnitude > moveSpeed) // limit velocity
		{
			Vector3 limitedVel = flatVel.normalized * moveSpeed;
			rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
		}
	}

	private void OnEnable()
	{
		moveAction.Enable();
	}

	private void OnDisable()
	{
		moveAction.Disable();
	}
}

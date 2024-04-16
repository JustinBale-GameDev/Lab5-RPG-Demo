using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
	[Header("References")]
	public Transform orientation;
	public Transform player;
	public Transform playerObj;
	public Rigidbody rb;
	public GameObject thirdPersonCam;

	[Header("Camera Settings")]
	public float rotationSpeed;

	public InputActionAsset inputActions;
	private InputAction moveAction;

	private void Awake()
	{
		moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
	}

	//private void Update()
	//{
	//	// Rotate orientation based on the player's position
	//	Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
	//	orientation.forward = viewDir.normalized;

	//	// Read movement input
	//	Vector2 currentMovementInput = moveAction.ReadValue<Vector2>();
	//	float horizontalInput = currentMovementInput.x;
	//	float verticalInput = currentMovementInput.y;

	//	Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

	//	if (inputDir != Vector3.zero)
	//	{
	//		playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
	//	}
	//}

	//private void FixedUpdate()
	//{
	//	Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
	//	orientation.forward = viewDir.normalized;

	//	Vector2 currentMovementInput = moveAction.ReadValue<Vector2>();
	//	float horizontalInput = currentMovementInput.x;
	//	float verticalInput = currentMovementInput.y;
	//	Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

	//	if (inputDir != Vector3.zero)
	//	{
	//		playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
	//	}
	//}

	private void LateUpdate()
	{
		Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
		orientation.forward = viewDir.normalized;

		Vector2 currentMovementInput = moveAction.ReadValue<Vector2>();
		float horizontalInput = currentMovementInput.x;
		float verticalInput = currentMovementInput.y;
		Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

		if (inputDir != Vector3.zero)
		{
			playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
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

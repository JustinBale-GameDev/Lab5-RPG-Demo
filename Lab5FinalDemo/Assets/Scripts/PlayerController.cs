using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
	[SerializeField] private Animator animator;
	[SerializeField] private Camera mainCamera;

	private InputAction moveAction, attackAction, lookAction;
    private Vector2 currentMovementInput;
	private Vector2 currentLookInput;
	private Vector2 velocity = Vector2.zero; // Smooth damping
	[SerializeField] private float movementSpeed = 5.0f;
	[SerializeField] private float smoothTime; // Adjust this value to control the smoothing speed
	[SerializeField] private float rotationSpeed = 10.0f;

	private bool isAttackInitiated = false;
	private float attackDuration = 1.5f; // Duration of the attack animation
	private float attackStartTime;


	private void Awake()
	{
        moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
		attackAction = inputActions.FindActionMap("Gameplay").FindAction("Attack");
		lookAction = inputActions.FindActionMap("Gameplay").FindAction("Look");
		animator = GetComponent<Animator>();
	}

	void Start()
    {
		// Lock the cursor to the game window and make it invisible
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

    void Update()
    {
		currentMovementInput = moveAction.ReadValue<Vector2>();
		currentLookInput = lookAction.ReadValue<Vector2>();

		
		UpdateMovingAnimation(); // Update the animator parameters for movement blend tree
		//HandleAttackInput();

		
	}

	private void FixedUpdate()
	{
		MoveCharacter();
		HandleRotation();
	}

	private void HandleRotation()
	{
		transform.Rotate(0, currentLookInput.x * rotationSpeed * Time.deltaTime, 0);
	}

	private void HandleAttackInput()
	{
		// Initiate attack
		if (attackAction.WasPressedThisFrame() && !isAttackInitiated)
		{
			if (currentMovementInput.sqrMagnitude > 0.01f)
			{
				animator.SetBool("isAttackingInAir", true);
			}
			else
			{
				animator.SetBool("isAttackingOnGround", true);
			}
			isAttackInitiated = true;
			attackStartTime = Time.time;
		}

		// Check if the attack animation duration has passed
		if (isAttackInitiated && Time.time > attackStartTime + attackDuration)
		{
			animator.SetBool("isAttackingInAir", false);
			animator.SetBool("isAttackingOnGround", false);
			isAttackInitiated = false;
		}
	}

	private void MoveCharacter()
	{
		// Read the move input value
		currentMovementInput = moveAction.ReadValue<Vector2>();

		if (currentMovementInput.sqrMagnitude > 0.01f) // Check for minimal input to avoid drifting
		{
			Vector3 inputDirection = new(currentMovementInput.x, 0, currentMovementInput.y);
			Vector3 movement = mainCamera.transform.TransformDirection(inputDirection);
			movement.y = 0; // Keep movement horizontal

			transform.Translate(movementSpeed * Time.deltaTime * movement, Space.World);
		}
	}

	private void UpdateMovingAnimation()
	{
		bool isMoving = currentMovementInput.sqrMagnitude > 0.01f;
		animator.SetBool("isMoving", isMoving);

		if (isMoving)
		{
			Vector3 worldDirection = mainCamera.transform.TransformDirection(new Vector3(currentMovementInput.x, 0, currentMovementInput.y));
			worldDirection.y = 0; // Keep the direction horizontal
			Vector3 localDirection = transform.InverseTransformDirection(worldDirection).normalized;

			// Smoothly interpolate the MoveX and MoveY values
			float moveX = Mathf.SmoothDamp(animator.GetFloat("MoveX"), localDirection.x, ref velocity.x, smoothTime, Mathf.Infinity, Time.deltaTime);
			float moveY = Mathf.SmoothDamp(animator.GetFloat("MoveY"), localDirection.z, ref velocity.y, smoothTime, Mathf.Infinity, Time.deltaTime); // Use z for forward/backward

			animator.SetFloat("MoveX", moveX);
			animator.SetFloat("MoveY", moveY);
		}
		else
		{
			// Not moving, smoothly transition back to idle values
			float moveX = Mathf.SmoothDamp(animator.GetFloat("MoveX"), 0, ref velocity.x, smoothTime, Mathf.Infinity, Time.deltaTime);
			float moveY = Mathf.SmoothDamp(animator.GetFloat("MoveY"), 0, ref velocity.y, smoothTime, Mathf.Infinity, Time.deltaTime);

			animator.SetFloat("MoveX", moveX);
			animator.SetFloat("MoveY", moveY);
		}
	}

	private void OnEnable()
	{
		moveAction.Enable();
		attackAction.Enable();
		lookAction.Enable();
	}
	private void OnDisable()
	{
		// When the game is paused or the player leaves the window, unlock the cursor and make it visible
		//Cursor.lockState = CursorLockMode.None;
		//Cursor.visible = true;

		moveAction.Disable();
		attackAction.Disable();
		lookAction.Disable();
	}
}

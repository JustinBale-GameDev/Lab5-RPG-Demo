using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
	[SerializeField] private Animator animator;

	private InputAction moveAction, attackAction;
    private Vector2 currentMovementInput;
	[SerializeField] private float movementSpeed = 5.0f;

	[SerializeField] private float smoothTime; // Adjust this value to control the smoothing speed
	private Vector2 velocity = Vector2.zero; // Needed for smooth damping

	private void Awake()
	{
        moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
		attackAction = inputActions.FindActionMap("Gameplay").FindAction("Attack");
        animator = GetComponent<Animator>();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Read the move input value
        currentMovementInput = moveAction.ReadValue<Vector2>();

		// Update the animator parameters for the blend tree
		UpdateMovingAnimation();

		// Attack
		if (attackAction.WasPressedThisFrame())
		{
			StartCoroutine(Attacking());
		}
	}

	private void FixedUpdate()
	{
		// Move the character
		MoveCharacter(currentMovementInput);
	}

	IEnumerator Attacking()
	{
		animator.SetBool("isAttacking", true);
		yield return new WaitForSeconds(1.5f);
		animator.SetBool("isAttacking", false);
	}

	private void MoveCharacter(Vector2 direction)
	{
		// Convert the 2D input int a 3D movement
		Vector3 move = new Vector3(direction.x, 0, direction.y) * movementSpeed * Time.deltaTime;

		// Apply movement
		transform.Translate(move, Space.World);
	}

	private void UpdateMovingAnimation()
	{
		// Check if moving
		if (currentMovementInput.magnitude > 0)
		{
			// Normalize the input vector to make sure the blend tree operates within its expected range
			Vector2 normalizedInput = currentMovementInput.normalized;
			// Smoothly interpolate the MoveX and MoveY values
			float moveX = Mathf.SmoothDamp(animator.GetFloat("MoveX"), normalizedInput.x, ref velocity.x, smoothTime);
			float moveY = Mathf.SmoothDamp(animator.GetFloat("MoveY"), normalizedInput.y, ref velocity.y, smoothTime);

			// Set the parameters that the Blend Tree uses to pick the correct animation
			animator.SetFloat("MoveX", moveX);
			animator.SetFloat("MoveY", moveY);
		}
		else
		{
			float moveX = Mathf.SmoothDamp(animator.GetFloat("MoveX"), 0, ref velocity.x, smoothTime);
			float moveY = Mathf.SmoothDamp(animator.GetFloat("MoveY"), 0, ref velocity.y, smoothTime);

			// No movement, reset parameters to transition back to Idle
			animator.SetFloat("MoveX", moveX);
			animator.SetFloat("MoveY", moveY);
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

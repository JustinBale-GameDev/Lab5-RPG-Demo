using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;
	[SerializeField]
	private Animator animator;

	private InputAction moveAction;
    [SerializeField] private Vector2 currentMovementInput;
	private float movementSpeed = 5.0f;

	[SerializeField] private float smoothTime = 0.1f; // Adjust this value to control the smoothing speed
	private Vector2 velocity = Vector2.zero; // Needed for smooth damping

	private void Awake()
	{
        moveAction = inputActions.FindActionMap("Gameplay").FindAction("Move");
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

		// Move the character
		MoveCharacter(currentMovementInput);

        // Update the animator parameters
		UpdateAnimation();
	}

	private void MoveCharacter(Vector2 direction)
	{
		// Convert the 2D input int a 3D movement
		Vector3 move = new Vector3(direction.x, 0, direction.y) * movementSpeed * Time.deltaTime;

		// Apply movement
		transform.Translate(move, Space.World);
	}

	private void UpdateAnimation()
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
			animator.SetFloat("MoveX", normalizedInput.x);
			animator.SetFloat("MoveY", normalizedInput.y);
		}
		else
		{
			// No movement, reset parameters to transition back to Idle
			animator.SetFloat("MoveX", 0);
			animator.SetFloat("MoveY", 0);
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

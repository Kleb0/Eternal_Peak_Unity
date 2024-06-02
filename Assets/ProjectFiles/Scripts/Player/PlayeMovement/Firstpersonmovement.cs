using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

// This script took instructions from the InputConnect script, and transmit to playerStateManager the current state of the player 
// modified by the input on the keyboard. The script also manage the player's movement and the camera rotation.
// It also uses the PlayerAnimation script to manage the player's animation controller.

public class Firstpersonmovement : MonoBehaviour
{
	public GameObject playerMeshRig;
	private CharacterController controller;
	private Camera cam;
	private Collider coll;
	private float walkSpeed = 2f;
	private float sprintSpeed = 5f;

	private float mouseSensitivity = 300f;
	private float jumpForce = 5f;

	public bool isClimbing = false;

	private Vector2 moveDirection;

	private Vector2 forwardDirection;
	private Vector2 rightDirection;


	//gravity is equal to one defined in project settings
	private float gravity = Physics.gravity.y;
	private float xRotation;

	private float verticalVelocity;


	// ------- States management (privates variables) ------ //
	private PlayerAnimation playerAnimation;
	private PlayerStateManager playerStateManager;
	private PlayerState initialState;
	private PlayerState currentState;
	private bool canChangeState = false;

	float velocity = 0f;
	float acceleration = 1.0f;
	float deceleration = 0.5f;

	// 
	// Start is called before the first frame update
	void Start()
	{

		playerAnimation = GetComponent<PlayerAnimation>();
		playerStateManager = GetComponent<PlayerStateManager>();
		initialState = new PlayerState_Idle();
		currentState = initialState;
		playerStateManager.SetState(new PlayerState_Idle());
		controller = GetComponent<CharacterController>();
		cam = GetComponentInChildren<Camera>();	
		Cursor.lockState = CursorLockMode.Locked;
	}


	// Update is called once per frame
	void Update()
	{
		Move();		
		Look();	
		applyGravity();
		OnChangeState();				
	}

	public void SetMoveDirection(Vector2 direction)
	{
		moveDirection = direction;
	}

	public void SetForwardDirection(Vector2 fDirection)
	{
		forwardDirection = fDirection;
		//Debug.Log("Forward Direction: " + forwardDirection);
	}

	public void SetRightDirection(Vector2 rDirection)
	{
		rightDirection = rDirection;
		
	}
	public void UpdateCurrentStateDirections(Vector2 forward, Vector2 right)
	{
		
		if (currentState is PlayerState_Moving movingState)
		{
			Debug.Log("Update Directions");
			movingState.UpdateDirections(forward, right);
		}
	}

	void applyGravity()
	{
		if (controller.isGrounded)
		{
			verticalVelocity = 0f;
		}
		else
		{
			verticalVelocity += gravity * Time.deltaTime;
		}
		Vector3 gravityMove = new Vector3(0f, verticalVelocity, 0f);
		controller.Move(gravityMove * Time.deltaTime);
	}

	void applyJump()
	{
		if (controller.isGrounded)
		{
			Debug.Log("Jump");
			verticalVelocity = jumpForce;
		}		
	}

	void Look()
	{
	
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
		
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		playerMeshRig.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
		transform.Rotate(Vector3.up * mouseX);	
	}
	
	void Move()
	{

		float currentSpeed = 0f;		

		if (moveDirection != Vector2.zero)
		{			
			currentSpeed = walkSpeed;	
			
		}
		

		// --> the newState is null at first to be sure that the we define a new state
		PlayerState newState = null;
		
		// we are in the running state if the player is sprinting by pressing the left shift key
		if (Input.GetKey(KeyCode.LeftShift) && currentSpeed > 0f)
		{

			// The Move script is called in the PlayerState_Walking state
			// The running script script inherits from the walking script
			// We pass the controller and the sprint speed or the walk speed to the Move method depending
			// if we are walking or sprinting		

			newState = new PlayerState_Running(controller, moveDirection, sprintSpeed, moveDirection, forwardDirection, rightDirection) ;


			velocity = RunWalkBlending(velocity, acceleration, 1f);	
			playerAnimation.SetRunning(velocity);		
				
		}

		// we are in the walking state if the player is moving and not sprinting
		else if (currentSpeed > 0f)
		{					

			newState = new PlayerState_Walking(controller, moveDirection, walkSpeed, moveDirection, forwardDirection, rightDirection);
			velocity = RunWalkBlending(velocity, -deceleration, 0f);
			playerAnimation.SetWalking(true);
			playerAnimation.SetRunning(velocity);
	
		}

		// we are in the idle state if the player is not moving
		else
		{		
			
			newState = new PlayerState_Idle();
			velocity = RunWalkBlending(velocity, -deceleration, 0f);
			playerAnimation.SetWalking(false);
			playerAnimation.SetRunning(velocity);
		}	
			
		// if the new state type is different from the current state type, we get the type of the new state 
		// we set the current state to the new state and we can change the state	

		if (currentState == null || newState.GetType() != currentState.GetType())
		{
			currentState = newState;
			canChangeState = true;
		}

	}

	public float RunWalkBlending(float currentVelocity, float rate, float target)
	{
		
		currentVelocity += rate * Time.deltaTime;

		// if the rate is positive, we return the minimum value between the current velocity and the target
		// if the rate is negative, we return the maximum value between the current velocity and the target

		if (rate> 0)
		{
			return Mathf.Min(currentVelocity, target);
		}
		else
		{
			return Mathf.Max(currentVelocity, target);
		}
	}

	void OnChangeState()
	{
		if (canChangeState)
		{
			playerStateManager.SetState(currentState);
			canChangeState = false;
			//Debug.Log("New State type is: " + currentState.GetType());
		}
	}
}

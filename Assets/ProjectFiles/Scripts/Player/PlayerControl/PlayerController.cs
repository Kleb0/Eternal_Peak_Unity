using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using RootMotion.FinalIK;

// This script took instructions from the InputConnect script, and transmit to playerStateManager the current state of the player 
// modified by the input on the keyboard. The script also manage the player's movement and the camera rotation.
// It also uses the PlayerAnimation script to manage the player's animation controller.

public class PlayerController : MonoBehaviour
{
	public GameObject player;
	public GameObject playerMesh;
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
	public PlayerAnimation playerAnimation;
	private PlayerStateManager playerStateManager;
	private State playerInitialState;
	public State currentPlayerState;
	private bool canChangeState = false;

	float velocity = 0f;
	float acceleration = 1.0f;
	float deceleration = 0.5f;

	// --- HandsStateController --- //
	private HandsStateController handsStateController;
	private UIDEBUG uiDebug;
	public GameObject playerStateUI;
	public bool uiDebugActive;

	private LeftHandState previousLeftHandState;

	private LeftHandState currentLeftHandState;
	private PlayerSetDirection playerSetDirection;

	

	// --- Access the IK solver properties --- //

	[Header("IK Solvers")]
	// Header space 
	[Space(10)]
	public IKSolverArm leftIKSolverArm;
	public ArmIK leftArmIK;
	public GameObject leftArmIKTarget;
	public IKSolverArm rightIKSolverArm;
	public ArmIK rightArmIK;

	public GameObject rightArmIKTarget;
	public float reachingSpeed;

	private InputConnect inputConnect;



	// Start is called before the first frame update
	void Start()
	{
		// get the 
		playerAnimation = GetComponent<PlayerAnimation>();
		playerStateManager = GetComponent<PlayerStateManager>();
		handsStateController = GetComponent<HandsStateController>();
		playerSetDirection = GetComponent<PlayerSetDirection>();
		inputConnect = GetComponent<InputConnect>();
		controller = GetComponent<CharacterController>();
		cam = GetComponentInChildren<Camera>();	
		uiDebug = GetComponent<UIDEBUG>();

		// get the ArmIK component from the player object

		foreach (ArmIK armIK in player.GetComponents<ArmIK>())
		{
			if (armIK.solver.isLeft)
			{
				Debug.Log("Left Arm IK found");
				leftArmIK = armIK;
		
			}
			else
			{
				Debug.Log("Right Arm IK found");
				rightArmIK = armIK;
	
			}
		}
		// initialization of the player state with its hand states at the start of the game

		playerInitialState = new PlayerState_Idle();
		currentPlayerState = playerInitialState;

		handsStateController.ChangeLeftHandState(new LeftHandState_DoNothing(this, leftArmIKTarget, leftArmIK));
		handsStateController.ChangeRightHandState(new RightHandState_DoNothing(this, rightArmIKTarget, rightArmIK));

		// we set the player state to the initial state
		playerStateManager.SetState(new PlayerState_Idle());
		// setup the UI Debug

		uiDebug.setUiDebugActive(uiDebugActive);

		if (uiDebugActive)
		{
			uiDebug.UpdatePlayerStateUI(currentPlayerState.stateName);		
			uiDebug.UpdateLeftHandStateUI(handsStateController.GetCurrentLeftHandState().stateName);
			uiDebug.UpdateRightHandStateUI(handsStateController.GetCurrentRightHandState().stateName);			
		}

		leftArmIK.enabled = false;
		leftArmIKTarget.SetActive(false);

		rightArmIK.enabled = false;
		rightArmIKTarget.SetActive(false);

		Cursor.lockState = CursorLockMode.Locked;
	}

	// Update is called once per frame
	void Update()
	{
		Move();		
		Look();	
		applyGravity();					
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

	#region  Movement
	void Move()
	{
		// here we read the input from the keyboard that give a value between -1 and 1 used to defined a vector 
		// that will be used to move the player in the world space

		moveDirection = new Vector2(inputConnect.rightLeft, inputConnect.forwardBackward);
		float currentSpeed = 0f;		

		// if the move direction is not equal to zero, the player is moving
		if (moveDirection != Vector2.zero)
		{
			currentSpeed = walkSpeed;
		}

		// --> the newState is null at first to be sure that the we define a new state
		State newPlayerState = null;
		
		// we are in the running state if the player is sprinting by pressing the left shift key
		if (Input.GetKey(KeyCode.LeftShift) && currentSpeed > 0f)
		{

			// The Move script is called in the PlayerState_Walking state
			// The running script script inherits from the walking script
			// We pass the controller and the sprint speed or the walk speed to the Move method depending
			// if we are walking or sprinting		

			newPlayerState = new PlayerState_Running(controller, playerSetDirection.GetMoveDirection(), sprintSpeed, 
			playerSetDirection.GetMoveDirection(),playerSetDirection.GetForwardDirection(), playerSetDirection.GetRightDirection());

			velocity = playerAnimation.RunWalkBlending(velocity, acceleration, 1f);	
			playerAnimation.SetRunning(velocity);		
				
		}

		// we are in the walking state if the player is moving and not sprinting
		else if (currentSpeed > 0f)
		{					

			newPlayerState = new PlayerState_Walking(controller, playerSetDirection.GetMoveDirection(), walkSpeed, 
			playerSetDirection.GetMoveDirection(), playerSetDirection.GetForwardDirection(),  playerSetDirection.GetRightDirection());

			velocity = playerAnimation.RunWalkBlending(velocity, -deceleration, 0f);
			playerAnimation.SetWalking(true);
			playerAnimation.SetRunning(velocity);
	
		}

		// we are in the idle state if the player is not moving
		else
		{		
			
			newPlayerState = new PlayerState_Idle();
			velocity = playerAnimation.RunWalkBlending(velocity, -deceleration, 0f);			
			playerAnimation.SetWalking(false);
			playerAnimation.SetRunning(velocity);
		}	
			
		// if the new state type is different from the current state type, we get the type of the new state 
		// we set the current state to the new state and we can change the state	

		if (currentPlayerState == null || newPlayerState.GetType() != currentPlayerState.GetType())
		{
			canChangeState = true;
			currentPlayerState = newPlayerState;		
			OnPlayerStateChange();
			if (uiDebugActive)
			{
				uiDebug.UpdatePlayerStateUI(newPlayerState.stateName);
			}
						
		}

	}

	#endregion	
	

	void OnPlayerStateChange()
	{
		if (canChangeState)
		{
			playerStateManager.SetState(currentPlayerState);
			canChangeState = false;		
		}
	}

	public void ChangeLeftIKWeight(float weight)
	{
		leftIKSolverArm.IKPositionWeight = weight;
		leftIKSolverArm.IKRotationWeight = weight;
	}

	public void ChangeRightIKWeight(float weight)
	{
		rightIKSolverArm.IKPositionWeight = weight;
		rightIKSolverArm.IKRotationWeight = weight;
	}

	// control here the hands of the player
}
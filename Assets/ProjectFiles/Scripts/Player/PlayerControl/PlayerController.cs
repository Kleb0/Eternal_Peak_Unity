using UnityEngine;
using RootMotion.FinalIK;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine.Rendering.UI;
using UnityEditor.ShaderGraph.Internal;

// This script took instructions from the InputConnect script, and transmit to playerStateManager the current state of the player 
// modified by the input on the keyboard. The script also manage the player's movement and the camera rotation.
// It also uses the PlayerAnimation script to manage the player's animation controller.

public class PlayerController : MonoBehaviour
{	// ------- variables ------ //
	#region Variables
	
	public GameObject player;
	public GameObject playerMesh;
	public GameObject playerMeshRig;
	public CharacterController controller;
	public Camera cam;
	public float walkSpeed = 2f;
	private float sprintSpeed = 5f;
	private float mouseSensitivity = 300f;
	public bool isClimbing = false;
	private Vector2 moveDirection;

	private Vector2 forwardDirection;
	private Vector2 rightDirection;

	// --- Guiding --- //

	private bool isPreservingMouseDirection = false;
	// --- Gravity --- //
	[Header("Gravity Settings")]
	[Space(10)]
	//gravity is equal to one defined in project settings
	private float gravity = Physics.gravity.y;
	private float xRotation;

	// ------- States management (privates variables) ------ //
	public PlayerAnimation playerAnimation;
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
	public PlayerSetDirection playerSetDirection;

	// --- Jumping --- //
	[Header("Jumping parameters")]
	[Space(10)]

	public bool isGrounded;
	// public bool isInAir = false;
	public bool haspressedJump = false;

	public bool isInAir = false;

	public float jumpHeight = 0.01f;
	public float jumpSpeed = 0.01f;
	public float jumpEllapsedTime = 0f;
	public float verticalVelocity = 0f;
	public float groundCheckDistance = 0.5f;




	// --- Access the IK solver properties --- //

	[Header("IK Solvers Left")]
	// Header space 
	[Space(10)]
	public IKSolverArm leftIKSolverArm;
	public ArmIK leftArmIK;
	public GameObject leftArmIKTarget;
	public GameObject leftBendingIKTarget;
	public GameObject leftHandHoldingGrip;
	public float leftArmBendingValue;
	public GameObject leftPalm;
	public GameObject leftPalmRaycaster;

	[Header("IK Solvers Right")]
	// Header space 
	[Space(10)]

	public IKSolverArm rightIKSolverArm;
	public ArmIK rightArmIK;
	public GameObject rightArmIKTarget;
	public GameObject rightBendingIKTarget;
	public GameObject rightHandHoldingGrip;
	public float rightArmBendingValue;
	private InputConnect inputConnect;

	public bool leftHandHoldingAGrip = false;
	public bool rightHandHoldingAGrip = false;
	public bool bothHandsHoldAGrip = false;

	// -------- State Management -------- //

	public PlayerState newPlayerState;
	public PlayerState currentPlayerState;
	private PlayerStateManager playerStateManager;
	private PlayerState playerInitialState;

	private bool canChangeState = false;

	#endregion
	// ------------------------ //

	// ------- Functions Awake Start Update ------ //
	#region Awake Start Update	
	void Awake()
	{
		
		playerAnimation = GetComponent<PlayerAnimation>();
		playerStateManager = GetComponent<PlayerStateManager>();
		handsStateController = GetComponent<HandsStateController>();
		playerSetDirection = GetComponent<PlayerSetDirection>();
		inputConnect = GetComponent<InputConnect>();
		controller = GetComponent<CharacterController>();
		cam = GetComponentInChildren<Camera>();
		uiDebug = GetComponent<UIDEBUG>();
	}

	// Start is called before the first frame update
	void Start()
	{

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
		playerStateManager.SetState(currentPlayerState);

		handsStateController.ChangeLeftHandState(new LeftHandState_DoNothing(this, leftArmIKTarget, leftBendingIKTarget, leftArmIK));
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
	
		Look();	
		Move();		
		applyGravity();
		isGrounded = CheckIsGrounded();
		// if(isGrounded == false)
		// {
		// 	Debug.Log("Not Grounded");
		// 	isInAir = true;
		// 	canJump = false;
		// 	jumpLock = true;
		// 	haspressedJump = false;			
		// }		
		// else
		// {
		// 	Debug.Log("Grounded");
		// 	isInAir = false;
		// 	canJump = true;
		// 	jumpLock = false;
		// 	haspressedJump = false;
		// }
	}

	#endregion	
	// ------------------------ //


	#region looks
	void Look()
	{

		if(handsStateController.currentLeftHandState.stateName == "Is Being Guided" || 
		handsStateController.currentRightHandState.stateName == "Is Being Guided" && 
		!isPreservingMouseDirection)
		{
			mouseSensitivity = 25f;
			float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
			float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
			
			xRotation -= mouseY;
			xRotation = Mathf.Clamp(xRotation, -90f, 90f);

			playerMeshRig.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
			transform.Rotate(Vector3.up * mouseX);

			// StartCoroutine(LookWhileBeingGuided());
		}
		else if (handsStateController.currentLeftHandState.stateName != "Is Being Guided")
		{
			mouseSensitivity = 300f;
			// -- Here we will decrease the mouse Speed when the hand state will be " is being guide "
			float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
			float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
			
			xRotation -= mouseY;
			xRotation = Mathf.Clamp(xRotation, -90f, 90f);

			playerMeshRig.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
			transform.Rotate(Vector3.up * mouseX);

		}
				
	}
	#endregion

// -------------------------------------- //


// -------------------------------------- //
	#region  Movement
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
	
	// The method Move have been changed. Instead of returning void, it now returns a PlayerState
	// as each movement implements a different playerstate encapsulating its own logic.

	PlayerState Move()
	{
		
		moveDirection = new Vector2(inputConnect.rightLeft, inputConnect.forwardBackward);
		float currentSpeed = 0f;

		bothHandsHoldAGrip = leftHandHoldingAGrip && rightHandHoldingAGrip;

		if (moveDirection != Vector2.zero)
		{
			currentSpeed = walkSpeed;
		}
	
		newPlayerState = null;


		//begining of our if else if chain
		if(isGrounded == false)
		{
			newPlayerState = new PlayerState_isJumping(this, controller, handsStateController, 
			playerSetDirection.GetMoveDirection(), jumpSpeed);
		}

		//we don't really jump here, we start a state that will make the player begin the jump
		//then the jump state pursue the movement when player is not grounded in the start of the chain

		if(haspressedJump && isGrounded && currentPlayerState.stateName == "Against Wall")
		{
			// handsStateController.ChangeLeftHandState(new LeftHandState_HoldingGripWhileJumping());
			// handsStateController.ChangeRightHandState(new RightHandState_HoldingGripWhileJumping());
			Debug.Log(" now hand state are holding grip while jumping and we don't start jump");
	
		}
		if (haspressedJump && isGrounded)
		{	
			if(currentPlayerState.stateName != "Against Wall")
			{
				newPlayerState = new PlayerState_StartJumping(this, controller, handsStateController, playerSetDirection.GetMoveDirection(),
				jumpSpeed, jumpHeight, playerSetDirection.GetForwardDirection(), playerSetDirection.GetRightDirection());

			}	
		}

	
		else if (Input.GetKey(KeyCode.LeftShift) && currentSpeed > 0f && !haspressedJump)
		{
			newPlayerState = new PlayerState_Running(controller, playerSetDirection.GetMoveDirection(), sprintSpeed,
				playerSetDirection.GetMoveDirection(), playerSetDirection.GetForwardDirection(), playerSetDirection.GetRightDirection());

			velocity = playerAnimation.RunWalkBlending(velocity, acceleration, 1f);
			playerAnimation.SetRunning(velocity);

		
		}
		else if (currentSpeed > 0f && !haspressedJump)
		{
			newPlayerState = new PlayerState_Walking(controller, playerSetDirection.GetMoveDirection(), walkSpeed,
				playerSetDirection.GetMoveDirection(), playerSetDirection.GetForwardDirection(), playerSetDirection.GetRightDirection());

			velocity = playerAnimation.RunWalkBlending(velocity, -deceleration, 0f);
			playerAnimation.SetWalking(true);
			playerAnimation.SetRunning(velocity);

		}
		else if (!haspressedJump && currentSpeed == 0f && !haspressedJump)
		{
			newPlayerState = new PlayerState_Idle();
			velocity = playerAnimation.RunWalkBlending(velocity, -deceleration, 0f);
			playerAnimation.SetWalking(false);
			playerAnimation.SetRunning(velocity);
		}

		// If the player is holding a grip, we set the player state to "Against Wall" and break the else if chain
		if (currentPlayerState.stateName == "Seize Grip")
		{
			newPlayerState = new PlayerState_AgainstWall(this, controller, playerSetDirection.GetMoveDirection(), walkSpeed,
				playerSetDirection.GetMoveDirection(), playerSetDirection.GetForwardDirection(), playerSetDirection.GetRightDirection(),
				rightArmBendingValue, leftArmBendingValue, leftHandHoldingGrip, rightHandHoldingAGrip, bothHandsHoldAGrip);			
		}
		
		
		if (newPlayerState != null && (currentPlayerState == null || newPlayerState.GetType() != currentPlayerState.GetType()))
		{
			canChangeState = true;
			currentPlayerState = newPlayerState; 
			OnPlayerStateChange();
			if (uiDebugActive)
			{
				uiDebug.UpdatePlayerStateUI(newPlayerState.stateName);
			}
		}

		return currentPlayerState;
	}


	#endregion		
// -------------------------------------- //

	public void OnPlayerStateChange()
	{
		if (canChangeState)
		{
			// Debug.Log("Changing Player State to " + currentPlayerState.stateName);
			playerStateManager.SetState(currentPlayerState);
			canChangeState = false;		
		}
	}

	public void SetAgainstWallState()
	{
		currentPlayerState = new PlayerState_AgainstWall(this, controller, playerSetDirection.GetMoveDirection(), walkSpeed,
			playerSetDirection.GetMoveDirection(), playerSetDirection.GetForwardDirection(), playerSetDirection.GetRightDirection(),
			rightArmBendingValue, leftArmBendingValue, leftHandHoldingAGrip, rightHandHoldingAGrip, bothHandsHoldAGrip);
		OnPlayerStateChange();
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

	// -------------------------------------- //

	#region functions returning private variables
	private bool CheckIsGrounded()
	{
		// Distance de vérification réduite pour éviter les faux positifs
		float groundCheckDistance = 0.2f; 
		LayerMask groundMask = LayerMask.GetMask("Ground");

		// Point de départ du Raycast légèrement ajusté au bas du collider
		Vector3 rayOrigin = controller.bounds.center;
		rayOrigin.y = controller.bounds.min.y + 0.1f; // Légèrement au-dessus du bas du collider

		RaycastHit hit;
		bool isGrounded = Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundMask);

		// Visualisation du Raycast
		Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red, 0.1f);

		// Log détaillé pour débogage

		return isGrounded;
	}


	#endregion

	// private void OnDrawGizmos()
	// {
	// 	if (controller == null) return;

	// 	Gizmos.color = Color.blue; // Couleur du raycast dans l'éditeur
	// 	Vector3 rayOrigin = controller.transform.position + Vector3.up * 0.2f;
	// 	float groundCheckDistance = 0.2f;

	// 	// Dessine le raycast pour vérifier si le joueur est au sol
	// 	Gizmos.DrawRay(rayOrigin, Vector3.down * groundCheckDistance);
	// }
}
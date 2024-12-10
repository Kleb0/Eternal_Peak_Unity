using UnityEngine;
using RootMotion.FinalIK;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.Mathematics;

// This script took instructions from the InputConnect script, and transmit to playerStateManager the current state of the player 
// modified by the input on the keyboard. The script also manage the player's movement and the camera rotation.
// It also uses the PlayerAnimation script to manage the player's animation controller.

public class PlayerController : MonoBehaviour
{	// ------- variables ------ //
	#region Variables
	
	public GameObject player;
	public GameObject playerMesh;
	public GameObject playerMeshRig;
	private CharacterController controller;
	public Camera cam;
	private float walkSpeed = 2f;
	private float sprintSpeed = 5f;
	private float mouseSensitivity = 300f;
	public bool isClimbing = false;

	private Vector2 moveDirection;

	private Vector2 forwardDirection;
	private Vector2 rightDirection;

	// --- Guiding --- //

	private bool isPreservingMouseDirection = false;
	private Quaternion targetRotation;
	private float smoothSpeed = 5f;
	
	// private static readonly Vector2[] directions = new Vector2[]
	// {
	// 	new Vector2(0, 1),	// North
	// 	new Vector2(0, -1),	// South
	// 	new Vector2(1, 0),	// East
	// 	new Vector2(-1, 0), // West
	// 	new Vector2(1, 1).normalized,	// North-East
	// 	new Vector2(-1, 1).normalized,	// North-West
	// 	new Vector2(1, -1).normalized,	// South-East
	// 	new Vector2(-1, -1).normalized	// South-West

	// };


	// --- Gravity --- //


	//gravity is equal to one defined in project settings
	private float gravity = Physics.gravity.y;
	private float xRotation;

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
	public PlayerSetDirection playerSetDirection;

	// --- Jumping --- //
	[Header("Jumping parameters")]
	[Space(10)]

	public bool canJump = false;
	public bool isInAir = false;
	public bool haspressedJump = false;

	public float jumpHeight = 0.02f;
	public float jumpSpeed = 0.02f;

	public float verticalVelocity = 0f;

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

	#endregion
	// ------------------------ //
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
		canJump = true;
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
		// leftBendingIKTarget.transform.position = leftPalm.transform.position;
		Look();	
		Move();		
		applyGravity();
		// CheckIfGrounded();			
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

	#region look
	void Look()
	{

		if(handsStateController.currentLeftHandState.stateName == "Is Being Guided" && !isPreservingMouseDirection)
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
	
	void Move()
	{
		

		moveDirection = new Vector2(inputConnect.rightLeft, inputConnect.forwardBackward);
		float currentSpeed = 0f;

		if(leftHandHoldingAGrip && rightHandHoldingAGrip)
		{
			bothHandsHoldAGrip = true;
		}
		else
		{
			bothHandsHoldAGrip = false;
		}

		// if the move direction is not equal to zero, the player is moving
		if (moveDirection != Vector2.zero)
		{
			currentSpeed = walkSpeed;
		}

		// --> the newState is null at first to be sure that the we define a new state
		State newPlayerState = null;

		// we are in the running state if the player is sprinting by pressing the left shift key
		if (Input.GetKey(KeyCode.LeftShift) && currentSpeed > 0f && !isInAir)
		{

			// The Move script is called in the PlayerState_Walking state
			// The running script script inherits from the walking script
			// We pass the controller and the sprint speed or the walk speed to the Move method depending
			// if we are walking or sprinting		

			newPlayerState = new PlayerState_Running(controller, playerSetDirection.GetMoveDirection(), sprintSpeed, 
			playerSetDirection.GetMoveDirection(),playerSetDirection.GetForwardDirection(), playerSetDirection.GetRightDirection());

			velocity = playerAnimation.RunWalkBlending(velocity, acceleration, 1f);	
			playerAnimation.SetRunning(velocity);
			if(haspressedJump)
			{
				isInAir = true;
			}
					
		}

			// we are in the walking state if the player is moving and not sprinting
		else if (currentSpeed > 0f && !isInAir)
		{					

			newPlayerState = new PlayerState_Walking(controller, playerSetDirection.GetMoveDirection(), walkSpeed, 
			playerSetDirection.GetMoveDirection(), playerSetDirection.GetForwardDirection(),  playerSetDirection.GetRightDirection());

			velocity = playerAnimation.RunWalkBlending(velocity, -deceleration, 0f);
			playerAnimation.SetWalking(true);
			playerAnimation.SetRunning(velocity);
			if(haspressedJump)
			{
				isInAir = true;
			}
		
		}

		else if(haspressedJump == false && currentSpeed == 0f)
		{		
				
			newPlayerState = new PlayerState_Idle();
			velocity = playerAnimation.RunWalkBlending(velocity, -deceleration, 0f);			
			playerAnimation.SetWalking(false);
			playerAnimation.SetRunning(velocity);
			if(haspressedJump)
			{
				isInAir = true;
			}

		}


		else if(haspressedJump)
		{
	
			// Debug.Log("Handle Jumping");
			newPlayerState = new PlayerState_Jumping(this, controller, playerSetDirection.GetMoveDirection(), 
			jumpSpeed, jumpHeight, playerSetDirection.GetForwardDirection(), playerSetDirection.GetRightDirection());
		}
				
		if(handsStateController.currentLeftHandState.stateName == "Is Holding A Grip" || handsStateController.currentRightHandState.stateName == "Is Holding A Grip")
		{
			
			newPlayerState = new PlayerState_AgainstWall(this, controller, playerSetDirection.GetMoveDirection(), walkSpeed, 
			playerSetDirection.GetMoveDirection(),playerSetDirection.GetForwardDirection(), playerSetDirection.GetRightDirection(), rightArmBendingValue, leftArmBendingValue,
			leftHandHoldingGrip, rightHandHoldingAGrip, bothHandsHoldAGrip);	
		}	

		// if the player is against a wall, we set the player state to the against wall state			
		// if the new state type is different from the current state type, we get the type of the new state 
		// we set the current state to the new state and we can change the state

		if (currentPlayerState == null || newPlayerState.GetType() != currentPlayerState.GetType())
		{
			Debug.Log("Changing Player State to " + newPlayerState.stateName);
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
// -------------------------------------- //

	public void OnPlayerStateChange()
	{
		if (canChangeState)
		{
			Debug.Log("Changing Player State to " + currentPlayerState.stateName);
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


	//-------- Our coroutines are here --------//
	#region Coroutines

	public void StartTestCoroutine()
	{
		StartCoroutine(TestCoroutine());
	}

	public IEnumerator TestCoroutine()
	{
		yield return new WaitForSeconds(0.001f);
		leftPalm.SetActive(true);
		leftBendingIKTarget.SetActive(true);
		IKArmsControl.AlignBendingIKTargetCorrectly(leftArmIK, leftBendingIKTarget, leftPalm.transform.position);
		// Debug.Log("Coroutine appelée dans le PlayerController");


	}
	// private IEnumerator LookWhileBeingGuided()
	// {
	// 	isPreservingMouseDirection = true;
	// 	float reducedSensitivity = mouseSensitivity * 0.2f;

	// 	quaternion initialDirection = transform.rotation;
	// 	Vector2 preservedMouseDelta = Mouse.current.delta.ReadValue();

	// 	//convert the mouse delta to the nearest direction
	// 	Vector2 normalizdDelta = preservedMouseDelta.normalized;
	// 	Vector2 closestDirection = FindClosestDirection(normalizdDelta);

	// 	//conv the direction into horizotal and vertical angle
	// 	float horizontalAngle = Mathf.Atan2(closestDirection.y, closestDirection.x) * Mathf.Rad2Deg;
	// 	Quaternion targetRotationHorizontal = Quaternion.Euler(0f, horizontalAngle, 0f);

	// 	// Add a vertical rotation based on the delta
	// 	float mouseY = preservedMouseDelta.y * reducedSensitivity * Time.deltaTime;
	// 	xRotation -= mouseY;
	// 	xRotation = Mathf.Clamp(xRotation, -90f, 90f);
	// 	Quaternion targetRotationVertical = Quaternion.Euler(xRotation, 0f, 0f);

	// 	float elpasedTime = 0f;
	// 	float transitionDuration = 1.5f;

	// 	while (elpasedTime < transitionDuration)
	// 	{
	// 		elpasedTime += Time.deltaTime;
	// 		float t = elpasedTime / transitionDuration;

	// 		transform.rotation = Quaternion.Lerp(initialDirection, targetRotationHorizontal, t);
	// 		playerMeshRig.transform.localRotation = Quaternion.Lerp(playerMeshRig.transform.localRotation, targetRotationVertical, t);

	// 		yield return null;
	// 	}

	// 	transform.rotation = targetRotationHorizontal;
	// 	playerMeshRig.transform.localRotation = targetRotationVertical;
	// 	isPreservingMouseDirection = false;
	// 	// Debug.Log("Coroutine appelée dans le PlayerController");
	// }



	#endregion
	// -------------------------------------- //

	#region functions returning private variables

	// private Vector2 FindClosestDirection(Vector2 delta)
	// {
	// 	Vector2 closest = directions[0];
	// 	float maxDot = Vector2.Dot(delta.normalized, closest);

	// 	foreach (Vector2 dir in directions)
	// 	{
	// 		float dot = Vector2.Dot(delta.normalized, dir);
	// 		if (dot > maxDot)
	// 		{
	// 			maxDot = dot;
	// 			closest = dir;
	// 		}
	// 	}
	// 	return closest;
	// }
	#endregion

}
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputConnect : MonoBehaviour
{

// ---- Variables and References ---- //
#region variables
	private PlayerControls controls;
	private PlayerAnimation playerAnimation;
	private HandsStateController handsStateController;
	private PlayerController playerController;
	private PlayerSetDirection playerSetDirection;
	private LeftHandState leftHandStateRishingUP;

	private RightHandState rightHandStateRishingUP;

	private ArmIK leftArmIK;
	private ArmIK rightArmIK;
	public float forwardBackward;
	public float rightLeft;

	private float leftHand;

	private float rightHand;   
	private bool combinedInput;

	private bool isLeftHandActionActive = false;

	private bool isRightHandActionActive = false;

	private bool hasStoppedMouseScroll = false;

	private bool isScrolling = false;
	private bool isGuideHandActive = false;
	private Coroutine stopScrollCoroutine;
	private Coroutine guideHandCoroutine;
	private Vector2 scrollValue;
	private Vector2 mouseDirection;
	private Vector2 preservedMouseDirection;
	private bool isPreservingMouseDirection = false;

	private static readonly Vector2[] directions = new Vector2[]
	{
		new Vector2(0, 1),	// North
		new Vector2(0, -1),	// South
		new Vector2(1, 0),	// East
		new Vector2(-1, 0), // West
		new Vector2(1, 1).normalized,	// North-East
		new Vector2(-1, 1).normalized,	// North-West
		new Vector2(1, -1).normalized,	// South-East
		new Vector2(-1, -1).normalized	// South-West

	};

	private static readonly string[] directionNames = new string[]
	{
		"North",
		"South",
		"East",
		"West",
		"North-East",
		"North-West",
		"South-East",
		"South-West"
	};

#endregion
// ----------------------------------------- //

// ---- Initialize ---- //
#region Initializations
	private void Awake()
	{
		playerController = GetComponent<PlayerController>();  
		playerSetDirection = GetComponent<PlayerSetDirection>();
		handsStateController = GetComponent<HandsStateController>();

		// leftHandStateRishingUP = new LeftHandState_IsRisingUp(playerController, playerController.LeftIKSolverArm);

		controls = new PlayerControls();

		controls.PlayerInputMap.ForwardBackward.performed += ctx => OnForwardMovement(ctx);
		controls.PlayerInputMap.ForwardBackward.canceled += ctx => OnForwardMovement(ctx);

		controls.PlayerInputMap.RightLeft.performed += ctx => OnSideMovement(ctx);
		controls.PlayerInputMap.RightLeft.canceled += ctx => OnSideMovement(ctx);

		controls.PlayerInputMap.LeftHand.performed += ctx => OnLeftHandAction(ctx);
		controls.PlayerInputMap.LeftHand.canceled += ctx => OnLeftHandAction(ctx);

		controls.PlayerInputMap.RightHand.performed += ctx => OnRightHandAction(ctx);
		controls.PlayerInputMap.RightHand.canceled += ctx => OnRightHandAction(ctx);

		controls.PlayerInputMap.Jump.performed += ctx => OnPlayerJump(ctx);

		controls.PlayerInputMap.RiseHand.performed += ctx => OnMouseScroll(ctx);
		controls.PlayerInputMap.RiseHand.canceled += ctx => OnMouseScrollCanceled(ctx);

		controls.PlayerInputMap.GuideHand.performed += ctx => OnGuideHandStart(ctx);
		controls.PlayerInputMap.GuideHand.canceled += ctx => OnGuideHandStop(ctx);

		
	}

	private void OnEnable()
	{
		controls.Enable();
	}

	private void OnDisable()
	{
		controls.Disable();
	}

#endregion
// ----------------------------------------- //


	private void Update()
	{
		if(handsStateController.currentLeftHandState.stateName =="Is Being Guided" && !isPreservingMouseDirection)
		{
			// mouseDirection = Mouse.current.delta.ReadValue();
			// handsStateController.mouseDirection = mouseDirection;	
			StartCoroutine(PreserveMouseDirectionRoutine());
		}		
	}



// ---- Hand Actions ---- //
	#region Hand actions
	// Left Hand Action
	private void OnLeftHandAction(InputAction.CallbackContext context)
	{	
		if (context.phase == InputActionPhase.Performed)
		{
			if (isLeftHandActionActive)
			{
				OnLeftHandActionCompleted();
				isLeftHandActionActive = false;
			}
			else
			{
				float initialLeftIkWeight = 0f;
				float initialLeftIkRotationWeight = 0f;
				// leftArmIK = playerController.leftArmIK;
				playerController.ChangeLeftIKWeight(1f);
				leftHandStateRishingUP = new LeftHandState_IsRisingUp(playerController, playerController.leftArmIKTarget, playerController.leftBendingIKTarget, playerController.leftArmIK, playerController.leftIKSolverArm, initialLeftIkWeight, initialLeftIkRotationWeight, playerController.leftArmIK);
				handsStateController.ChangeLeftHandState(leftHandStateRishingUP);
				isLeftHandActionActive = true;
			}		
		} 
	}
	
	// Right Hand Action
	private void OnRightHandAction(InputAction.CallbackContext context)
	{

		if (context.phase == InputActionPhase.Performed)
		{

			if (isRightHandActionActive)
			{
				OnRightHandActionCompleted();
				isRightHandActionActive = false;
			}
			else
			{

				float initialRightIkWeight = 0f;
				float initialRightIkRotationWeight = 0f;
				playerController.ChangeRightIKWeight(1f);
				rightHandStateRishingUP = new RightHandState_IsRisingUp(playerController, playerController.rightArmIKTarget, playerController.rightArmIK, playerController.rightIKSolverArm, initialRightIkWeight, initialRightIkRotationWeight, playerController.rightArmIK);
				handsStateController.ChangeRightHandState(rightHandStateRishingUP);
				isRightHandActionActive = true;		
			}
		
		}
		
	}

	private void OnLeftHandActionCompleted()
	{	
		if (handsStateController != null)
		{
			handsStateController.RevertLeftHandState(new LeftHandState_DoNothing(playerController, playerController.leftArmIKTarget, playerController.leftBendingIKTarget, playerController.leftArmIK));
		}
	}

	private void OnRightHandActionCompleted()
	{
		if (handsStateController != null)
		{
			handsStateController.RevertRightHandState(new RightHandState_DoNothing(playerController, playerController.rightArmIKTarget, playerController.rightArmIK));
		}
		//playerController.OnPlayerRiseRightArmBack();
	} 

	#endregion
// ------------------------------------- //

// ---- Movement ---- //
#region movement	
	private void UpdateMovement()
	{
		Vector2 moveDirection = new Vector2(rightLeft, forwardBackward);
		playerSetDirection.SetMoveDirection(moveDirection);
		playerSetDirection.SetForwardDirection(new Vector2(0f, forwardBackward));
		playerSetDirection.SetRightDirection(new Vector2(rightLeft, 0f));
		playerSetDirection.UpdateCurrentMovingDirection(new Vector2(0f, forwardBackward), new Vector2(rightLeft, 0f));   
	}

	private void OnForwardMovement(InputAction.CallbackContext context)
	{
		forwardBackward = context.phase == InputActionPhase.Performed ? context.ReadValue<float>() : 0f;
		UpdateMovement();

	}

	private void OnSideMovement(InputAction.CallbackContext context)
	{
		rightLeft = context.phase == InputActionPhase.Performed ? context.ReadValue<float>() : 0f;
		UpdateMovement();   

	}	


	private void OnPlayerJump(InputAction.CallbackContext context)
	{
		Debug.LogWarning("Jump Input called in InputConnect.CS ");
		Debug.Log("Jump Input called in InputConnect.CS ");
		playerController.haspressedJump = true;
		playerController.canJump = true;				
	}
#endregion
// ------------------------------------- //

// ---- Mouse Scroll Input ----------------- //
#region ScrollInput
	private void OnMouseScroll(InputAction.CallbackContext context)
	{
		scrollValue = context.ReadValue<Vector2>();

		if (scrollValue.y != 0)
		{
			//Debug.Log("Mouse Scroll Detected in InputConnect.CS");
			isScrolling = true;
			handsStateController.changeLeftHandIkTargetOnScroll();
			
			if(stopScrollCoroutine != null)
			{
				StopCoroutine(stopScrollCoroutine);
				stopScrollCoroutine = null;
			}

			if(scrollValue.y < 0)
			{
				
				handsStateController.OnMouseScrollDetected(1f);
			}
			else if(scrollValue.y > 0)
			{
				handsStateController.OnMouseScrollDetected(-1f);

			}

			// Debug.Log("Mouse Scroll Value: " + scrollValue);
		}
		else
		{
			if (stopScrollCoroutine == null)
			{
				stopScrollCoroutine = StartCoroutine(WaitForScrollStop());
			}
			
		}

		// else if (scrollValue == Vector2.zero)
		// {
		// 	Debug.LogWarning("Mouse Scroll Stopped in InputConnect.CS");
		// 	handsStateController.StopMouseScroll();

		// }
	}
	private void OnMouseScrollCanceled(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Canceled)
		{
			if (stopScrollCoroutine == null)
			{
				stopScrollCoroutine = StartCoroutine(WaitForScrollStop());
			}
		}
	}

	//We use a Ienumerator to wait for a short time before we stop the scrolling, then we know that the player has stopped scrolling
	//and we can set the scrollValue to zero

#endregion   
// ----------------------------------------- //

// ---- Mouse Movement Input ----------------- //
#region Mouse Displacement Calculation and Logic

	private void OnGuideHandStart(InputAction.CallbackContext context)
	{

		if(guideHandCoroutine == null)
		{
			guideHandCoroutine = StartCoroutine(GuideHandHoldRoutine());
		}
	}

	private void OnGuideHandStop(InputAction.CallbackContext context)
	{

		if(guideHandCoroutine != null)
		{
			StopCoroutine(guideHandCoroutine);
			guideHandCoroutine = null;
		}

		if(isGuideHandActive)
		{
			StartCoroutine(StayInGuidedStateBeforeTransition());
		}

		
	}

#endregion
// ----------------------------------------- //	

// ---- Coroutines ---- //
#region Coroutines Declaration
	// the state is managed by the handStateController. But this script InputConnect.Cs receive the player's input.
	// so we can call the IKarmsControls Static function here as we want the function to be called after we stopped
	// to press the left mouse button

	private IEnumerator PreserveMouseDirectionRoutine()
	{	
		isPreservingMouseDirection = true;
		// preservedMouseDirection = Mouse.current.delta.ReadValue();
		// handsStateController.mouseDirection = preservedMouseDirection;

		// yield return new WaitForSeconds(1.5f);
		// isPreservingMouseDirection = false;

		while (handsStateController.currentLeftHandState.stateName == "Is Being Guided")
		{
			Vector2 rawMouseDirection = Mouse.current.delta.ReadValue();
			var (directionName, vector) = GetMouseDirectionAndVector(rawMouseDirection);


			handsStateController.mouseDirection = vector;
			handsStateController.DirectionName = directionName;

			yield return new WaitForSeconds(0.5f);
		}

		isPreservingMouseDirection = false;
	}


	private IEnumerator GuideHandHoldRoutine()
	{
		yield return new WaitForSeconds(0.2f);
		isGuideHandActive = true;
		// mousePosition = Mouse.current.position.ReadValue();
		// Debug.Log($"Mouse Position is {mousePosition}");
		handsStateController.ChangeLeftHandState(new leftHandState_isBeingGuided());
	}

	private IEnumerator StayInGuidedStateBeforeTransition()
	{
		float delay = 1.0f;
		yield return new WaitForSeconds(delay);

		if(isGuideHandActive)
		{
			// mousePosition = Mouse.current.position.ReadValue();
			// Debug.Log($"Mouse Position is {mousePosition}");
			isGuideHandActive = false;
			handsStateController.ChangeLeftHandState(new leftHandState_HasRaisedUp(playerController, playerController.leftArmIKTarget, playerController.leftBendingIKTarget, playerController.leftIKSolverArm, playerController.leftArmIK));
		}
	}

	private IEnumerator WaitForScrollStop()
	{
		yield return new WaitForSeconds(0.2f);
		if(isScrolling)
		{
			isScrolling = false;
			scrollValue = Vector2.zero;
			// Debug.LogWarning("Mouse Scroll Stopped in InputConnect.CS, scrollValue: " + scrollValue);
		}
		stopScrollCoroutine = null;
		handsStateController.StopMouseScroll();
		handsStateController.ResetCanChangeLeftIkTargetOnScroll();
	}

	

#endregion	
// ----------------------------------------- //

// ---- Functions that returns variables ---- //
#region functions that returns variables 

//here we use a tuple to return two values, the direction name and the normalized vector
private (string directionName, Vector2 vector) GetMouseDirectionAndVector (Vector2 mouseDir)
{

	float mouseSensitive = 0.25f;


	if(mouseDir.magnitude < mouseSensitive)
	{
		return ("None", Vector2.zero);
	}

	Vector2 normalizedMouseDir = mouseDir.normalized;

	string directionName = "None";
	Vector2 vector = Vector2.zero;

	//we prioritize the principal axes over the diagonal ones such as North-East, North-West, South-East, South-West
	if (Mathf.Abs(normalizedMouseDir.x) > Mathf.Abs(normalizedMouseDir.y))
	{
		if (normalizedMouseDir.x > 0)
		{
			directionName = "East";
			vector = new Vector2(1, 0);
		}
		else
		{
			directionName = "West";
			vector = new Vector2(-1, 0);
		}
	}
	else
	{
		if (normalizedMouseDir.y > 0)
		{
			directionName = "North";
			vector = new Vector2(0, 1);
		}
		else
		{
			directionName = "South";
			vector = new Vector2(0, -1);
		}
	}

	if(Mathf.Abs(normalizedMouseDir.x - normalizedMouseDir.y) < 0.35f) // tolerance for diagonal directions
	{
		if (normalizedMouseDir.x > 0 && normalizedMouseDir.y >0)
		{
			directionName = "North-East";
			vector = new Vector2(1, 1).normalized;
		}
		else if (normalizedMouseDir.x > 0 && normalizedMouseDir.y < 0)
		{
			directionName = "South-East";
			vector = new Vector2(1, -1).normalized;
		}
		else if(normalizedMouseDir.x < 0 && normalizedMouseDir.y > 0)
		{
			directionName = "North-West";
			vector = new Vector2(-1, 1).normalized;
		}
		else if(normalizedMouseDir.x < 0 && normalizedMouseDir.y < 0)
		{
			directionName = "South-West";
			vector = new Vector2(-1, -1).normalized;
		}
	}

	return (directionName, vector);
	// int closestIndex = 0;
	// float maxDot = -1;

	// for (int i = 0; i < directions.Length; i++)
	// {
	// 	float dot = Vector2.Dot(mouseDir, directions[i]);
	// 	if (dot > maxDot)
	// 	{
	// 		maxDot = dot;
	// 		closestIndex = i;
	// 	}
	// }

	// string directionName = directionNames[closestIndex];
	// Vector2 vector = directions[closestIndex];
	// return (directionName, vector);
}
#endregion
// ----------------------------------------- //
}
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

	private bool isScrolling = false;
	private bool isleftHandGuideActive = false;
	private bool isRightHandGuideActive = false;
	private Coroutine stopScrollCoroutine;
	private Coroutine guideLeftHandCoroutine;
	private Coroutine guideRightHandCoroutine;
	private Vector2 scrollValue;
	private Vector2 mouseDirection;
	private Vector2 preservedMouseDirection;
	private bool isPreservingMouseDirection = false;

	private float ScrollSensitivity = 10f;

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

		if(handsStateController.currentRightHandState.stateName =="Is Being Guided" && !isPreservingMouseDirection)
		{
	
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
				leftHandStateRishingUP = new LeftHandState_IsRisingUp(playerController, playerController.leftArmIKTarget, playerController.leftArmIK, playerController.leftIKSolverArm, initialLeftIkWeight, initialLeftIkRotationWeight, playerController.leftArmIK);
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
		
		playerController.leftBendingIKTarget.transform.position = playerController.leftArmIK.solver.hand.transform.position;
		playerController.rightBendingIKTarget.transform.position = playerController.rightArmIK.solver.hand.transform.position;
		scrollValue = context.ReadValue<Vector2>();

		//Debug.Log($"isLeftArmActive: {handsStateController.isLeftArmActive}, isRightArmActive: {handsStateController.isRightArmActive}");

		if (scrollValue.y != 0)
		{
			if (handsStateController.isLeftArmActive)
			{
				playerController.leftBendingIKTarget.SetActive(true);
				isScrolling = true;
				handsStateController.changeLeftHandIkTargetOnScroll();	

				

				manageScrollInput();		

			}
			if (handsStateController.isRightArmActive)
			{
				playerController.rightBendingIKTarget.SetActive(true);
				isScrolling = true;
				handsStateController.changeRightHandIkTargetOnScroll();

				manageScrollInput();
			}
			
		}
		else
		{
			stopScrollCoroutine ??= StartCoroutine(WaitForScrollStop());
			
		}
	}

	private void manageScrollInput()
	{
			if(stopScrollCoroutine != null)
				{
					StopCoroutine(stopScrollCoroutine);
					stopScrollCoroutine = null;
				}

				if(scrollValue.y < 0)
				{
					handsStateController.OnMouseScrollDetected(ScrollSensitivity);
				}
				else if(scrollValue.y > 0)
				{
					handsStateController.OnMouseScrollDetected(-ScrollSensitivity);
				}
	}
	
	private void OnMouseScrollCanceled(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Canceled)
		{
			stopScrollCoroutine ??= StartCoroutine(WaitForScrollStop());
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
    // Règle C : Si les deux mains sont dans l'état "Has Raised Up", elles peuvent entrer simultanément dans l'état "Is Being Guided"
    if (handsStateController.currentLeftHandState.stateName == "Has Raised Up" &&
        handsStateController.currentRightHandState.stateName == "Has Raised Up")
    {
        guideLeftHandCoroutine ??= StartCoroutine(GuideLeftHandHoldRoutine());
        guideRightHandCoroutine ??= StartCoroutine(GuideRightHandHoldRoutine());
        return; 
    }

    // Règle A : Si l'état de la main droite n'est pas "Is Being Guided", alors la main gauche peut entrer dans cet état
    if (handsStateController.currentRightHandState.stateName != "Is Being Guided" &&
	 handsStateController.currentLeftHandState.stateName == "Has Raised Up" || 
	 handsStateController.currentRightHandState.stateName == "Do Nothing")
    {
        guideLeftHandCoroutine ??= StartCoroutine(GuideLeftHandHoldRoutine());
    }

    // Règle B : Si l'état de la main gauche n'est pas "Is Being Guided", alors la main droite peut entrer dans cet état
    if (handsStateController.currentLeftHandState.stateName != "Is Being Guided" &&
	handsStateController.currentRightHandState.stateName == "Has Raised Up" ||
	handsStateController.currentLeftHandState.stateName == "Do Nothing")
    {
        guideRightHandCoroutine ??= StartCoroutine(GuideRightHandHoldRoutine());
    }
}




	private void OnGuideHandStop(InputAction.CallbackContext context)
	{

		if(guideLeftHandCoroutine != null)
		{
			StopCoroutine(guideLeftHandCoroutine);
			guideLeftHandCoroutine = null;
		}
		if(guideRightHandCoroutine != null)
		{
			StopCoroutine(guideRightHandCoroutine);
			guideRightHandCoroutine = null;
		}

		if(isleftHandGuideActive)
		{
			StartCoroutine(LeftHandStayInGuidedStateBeforeTransition());
		}
		if(isRightHandGuideActive)
		{
			StartCoroutine(RightHandStayInGuidedStateBeforeTransition());
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

		while (handsStateController.currentLeftHandState.stateName == "Is Being Guided")
		{
			Vector2 rawMouseDirection = Mouse.current.delta.ReadValue();

			if (rawMouseDirection.magnitude < 0.01f)
			{
				handsStateController.mouseDirection = Vector2.zero;
				handsStateController.DirectionName = "None";
			}
			else
			{
			
			var (directionName, vector) = GetMouseDirectionAndVector(rawMouseDirection);

			handsStateController.mouseDirection = vector;
			handsStateController.DirectionName = directionName;
			
			}

			yield return new WaitForSeconds(0.5f);
		}
		while (handsStateController.currentRightHandState.stateName == "Is Being Guided")
		{
			Vector2 rawMouseDirection = Mouse.current.delta.ReadValue();

			if (rawMouseDirection.magnitude < 0.01f)
			{
				handsStateController.mouseDirection = Vector2.zero;
				handsStateController.DirectionName = "None";
			}
			else
			{
			
			var (directionName, vector) = GetMouseDirectionAndVector(rawMouseDirection);

			handsStateController.mouseDirection = vector;
			handsStateController.DirectionName = directionName;
			
			}

			yield return new WaitForSeconds(0.5f);
		}

		isPreservingMouseDirection = false;
	}


	private IEnumerator GuideLeftHandHoldRoutine()
	{
	
		yield return new WaitForSeconds(0.1f);
		isleftHandGuideActive = true;
		handsStateController.ChangeLeftHandState(new leftHandState_isBeingGuided());

	}

	private IEnumerator GuideRightHandHoldRoutine()
	{
		yield return new WaitForSeconds(0.1f); // Attente avant de démarrer
		isRightHandGuideActive = true;
		handsStateController.ChangeRightHandState(new RightHandState_isBeingGuided()); // État spécifique pour la main droite
	}


	private IEnumerator LeftHandStayInGuidedStateBeforeTransition()
	{
		float delay = 1.0f;
		yield return new WaitForSeconds(delay);

		if(isleftHandGuideActive)
		{
			isleftHandGuideActive = false;
			handsStateController.ChangeLeftHandState(new leftHandState_HasRaisedUp(playerController, playerController.leftArmIKTarget, 
			playerController.leftBendingIKTarget, playerController.leftIKSolverArm, playerController.leftArmIK));
		}	
	}

	private IEnumerator RightHandStayInGuidedStateBeforeTransition()
	{
		float delay = 1.0f; // Temps avant de quitter l'état de guidage
		yield return new WaitForSeconds(delay);

		if (isRightHandGuideActive)
		{
			isRightHandGuideActive = false;
			handsStateController.ChangeRightHandState(new RightHandState_HasRaisedUp(playerController,playerController.rightArmIKTarget,
			playerController.rightBendingIKTarget, playerController.rightIKSolverArm, playerController.rightArmIK));
		}
	}

	private IEnumerator WaitForScrollStop()
	{
		yield return new WaitForSeconds(0.2f);
		if(isScrolling)
		{
			isScrolling = false;
			scrollValue = Vector2.zero;
		}
		stopScrollCoroutine = null;
		handsStateController.StopMouseScroll();
		handsStateController.ResetCanChangeLeftIkTargetOnScroll();
		handsStateController.ResetCanChangeRightIkTargetOnScroll();
	}

	

#endregion	
// ----------------------------------------- //

// ---- Functions that returns variables ---- //
#region functions that returns variables 

//here we use a tuple to return two values, the direction name and the normalized vector
private (string directionName, Vector2 vector) GetMouseDirectionAndVector (Vector2 mouseDir)
{

	float mouseSensitive = 1f;
	float sensitivityMultiplier = 20f;

	if(mouseDir.magnitude < mouseSensitive)
	{
		return ("None", Vector2.zero);
	}

	Vector2 adjustedMouseDir = mouseDir * sensitivityMultiplier;
	Vector2 normalizedMouseDir = adjustedMouseDir.normalized;
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
}
#endregion
// ----------------------------------------- //
}
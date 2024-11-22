using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputConnect : MonoBehaviour
{

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

	private void UpdateMovement()
	{
		Vector2 moveDirection = new Vector2(rightLeft, forwardBackward);
		playerSetDirection.SetMoveDirection(moveDirection);
		playerSetDirection.SetForwardDirection(new Vector2(0f, forwardBackward));
		playerSetDirection.SetRightDirection(new Vector2(rightLeft, 0f));
		playerSetDirection.UpdateCurrentMovingDirection(new Vector2(0f, forwardBackward), new Vector2(rightLeft, 0f));   
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

	private void OnPlayerJump(InputAction.CallbackContext context)
	{
		Debug.LogWarning("Jump Input called in InputConnect.CS ");
		Debug.Log("Jump Input called in InputConnect.CS ");
		playerController.haspressedJump = true;
		playerController.canJump = true;				
	}

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

#region Holding Mouse ToGuide Hand Logic 

	private void OnGuideHandStart(InputAction.CallbackContext context)
	{

		if(guideHandCoroutine == null)
		{
			guideHandCoroutine = StartCoroutine(GuideHandHoldRoutine());
		}

	}

	private void OnGuideHandStop(InputAction.CallbackContext context)
	{
		if (guideHandCoroutine != null)
		{
			StopCoroutine(guideHandCoroutine);
			guideHandCoroutine = null;
		}
		if (isGuideHandActive)
		{
			isGuideHandActive = false;
			Debug.Log("GuideHand Action Completed");
		}
	}

	private IEnumerator GuideHandHoldRoutine()
	{
		yield return new WaitForSeconds(0.2f);
		isGuideHandActive = true;
		Debug.Log("Guide Hand Active");
	}

	
#endregion
}
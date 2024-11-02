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
				leftHandStateRishingUP = new LeftHandState_IsRisingUp(playerController, playerController.leftArmIKTarget, playerController.leftArmIK, playerController.leftIKSolverArm, initialLeftIkWeight, initialLeftIkRotationWeight);
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
				rightHandStateRishingUP = new RightHandState_IsRisingUp(playerController, playerController.rightArmIKTarget, playerController.rightArmIK, playerController.rightIKSolverArm, initialRightIkWeight, initialRightIkRotationWeight);
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
			handsStateController.RevertLeftHandState(new LeftHandState_DoNothing(playerController, playerController.leftArmIKTarget, playerController.leftArmIK));
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

	private void OnMouseScroll(InputAction.CallbackContext context)
	{
		Vector2 scrollValue = context.ReadValue<Vector2>();

		if (scrollValue.y > 0)
		{
			Debug.Log("la molette de la souris est tournée vers le haut");
		}
		else if (scrollValue.y < 0)
		{
			Debug.Log("la molette de la souris est tournée vers le bas");
		}

	}
 
  
}
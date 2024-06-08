using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputConnect : MonoBehaviour
{

	private PlayerControls controls;
	private HandsStateController handsStateController;
	private PlayerController playerController;
	private PlayerSetDirection playerSetDirection;
	private LeftHandState leftHandStateRishingUP;
	public float forwardBackward;
	public float rightLeft;

	private float leftHand;

	private float rightHand;   
	private bool combinedInput;

	private void Awake()
	{
		playerController = GetComponent<PlayerController>();  
		playerSetDirection = GetComponent<PlayerSetDirection>();
		handsStateController = GetComponent<HandsStateController>();

		leftHandStateRishingUP = new LeftHandState_IsRisingUp();

	
		controls = new PlayerControls();

		controls.PlayerInputMap.ForwardBackward.performed += ctx => OnForwardMovement(ctx);
		controls.PlayerInputMap.ForwardBackward.canceled += ctx => OnForwardMovement(ctx);

		controls.PlayerInputMap.RightLeft.performed += ctx => OnSideMovement(ctx);
		controls.PlayerInputMap.RightLeft.canceled += ctx => OnSideMovement(ctx);

		controls.PlayerInputMap.LeftHand.performed += ctx => OnLeftHandAction(ctx);
		controls.PlayerInputMap.LeftHand.canceled += ctx => OnLeftHandAction(ctx);

		controls.PlayerInputMap.RightHand.performed += ctx => OnRightHandAction(ctx);
		controls.PlayerInputMap.RightHand.canceled += ctx => OnRightHandAction(ctx);     
		
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

	private void OnLeftHandAction(InputAction.CallbackContext context)
	{
		leftHand = context.phase == InputActionPhase.Performed ? context.ReadValue<float>() : 0f;

		if (context.phase == InputActionPhase.Performed)
		{
	   
			handsStateController.ChangeLeftHandState(new LeftHandState_IsRisingUp());
		}
		 

		if (context.phase == InputActionPhase.Canceled)
		{
			OnLeftHandActionCompleted();
		   
		}    

	}
	
	private void OnRightHandAction(InputAction.CallbackContext context)
	{
		rightHand = context.phase == InputActionPhase.Performed ? context.ReadValue<float>() : 0f;      

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
		Debug.Log("Left Hand action completed ");
		handsStateController.RevertLeftHandState();
	} 
 
  
}

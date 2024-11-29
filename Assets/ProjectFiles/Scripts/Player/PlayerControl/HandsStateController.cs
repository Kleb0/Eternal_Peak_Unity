using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class HandsStateController : MonoBehaviour
{
	UIDEBUG uiDebug;

	public LeftHandState currentLeftHandState;
	public RightHandState currentRightHandState;
	private LeftHandState previousLeftHandState;
	private RightHandState previousRightHandState;
	private LeftHandState_DoNothing leftHandStateDoNothing;
	private LeftHandState_ComingBack leftHandStateComingBack;    
	private LeftHandState_IsRisingUp leftHandStateIsRisingUp;
	private GameObject leftPalm;
	private GameObject leftHandRayCaster;	
	private GameObject rightPalm;
	private GameObject rightHandRayCaster;
	
	private PlayerController playerController;
	private float currentLeftIkWeight;
	private float currentLeftIkRotationWeight;

	private float currentRightIkWeight;
	private float currentRightIkRotationWeight;
	private bool isLeftHandLoopEnded = false;
	private bool isRightHandLoopEnded = false;

	float distanceBetweenLeftHandAndLeftShoulder; 
	float distanceBetweenRightHandAndRightShoulder;   
	private float scrollAdjustmentValue = 0f; 
	private bool isScrolling = false;
	private bool canChangeLeftIkTargetOnScroll = false;
	private bool isLeftHandAligned = false;

	private bool isBeingGuide = false;


	// We build a loop for the hand state management :
	// in the player controller script, the hand state is defined on do nothing at start
	// in the input connect script, when the player press the left mouse button, the hand state is set to is rising up
	// then, when the action is cancelled, the hand state is set to coming back
	// and when the hand state is coming back, the hand state is set to do nothing
	// so we have a loop : do nothing -> is rising up -> coming back -> do nothing
	// these states, will activate and deactivate the arm Ik weight, the arm Ik rotation weight, and the arm Ik target


	public void Start()
	{
		playerController = GetComponent<PlayerController>();
		uiDebug = GetComponent<UIDEBUG>();
		if (uiDebug == null)
		{
			Debug.LogError("UI Debug is null");
		}
		else
		{
			Debug.Log("UI Debug is not null");
		}

		currentLeftIkWeight = 0f;
		currentLeftIkRotationWeight = 0f;
		currentRightIkWeight = 0f;
		currentRightIkRotationWeight = 0f;
		leftPalm = playerController.leftPalm;
		if(leftPalm == null)
		{
			leftPalm = GameObject.Find("LeftPalm");
		}

		leftHandRayCaster = GameObject.Find("Left_HandRay_Caster");
		rightPalm = GameObject.Find("RightPalm");
		rightHandRayCaster = rightPalm.transform.GetChild(0).gameObject;
		leftPalm.SetActive(false);
		rightPalm.SetActive(false);
	}

	public void Update()
	{
		// here we execute the action of the current active state
		if (currentLeftHandState != null)
		{
			currentLeftHandState.ExecuteState();
			currentRightHandState.ExecuteState();
		}
		
		PlayLeftHandLoop();
		PlayRightHandLoop();
	} 

#region  Player Left Hand Loop
	public void PlayLeftHandLoop()
	{
		if (currentLeftHandState is LeftHandState_ComingBack)
		{
			
			bool completed = IKArmsControl.DecrementLeftIkWeight(playerController, playerController.leftArmIK, ref currentLeftIkWeight, ref currentLeftIkRotationWeight, 2f);
	

			if (completed && !isLeftHandLoopEnded)
			{
				EndLeftHandLoop();
				isLeftHandLoopEnded = false;
				isLeftHandAligned = false;

			}
		}

		else if (currentLeftHandState is LeftHandState_IsRisingUp) 
		{
		
			bool completed = IKArmsControl.IncrementLeftIkWeight(playerController, playerController.leftArmIK, ref currentLeftIkWeight, ref currentLeftIkRotationWeight, 2f);

			if (completed)
			{
				SetLeftHandState(new leftHandState_HasRaisedUp(playerController, playerController.leftArmIKTarget, 
				playerController.leftBendingIKTarget, playerController.leftIKSolverArm, playerController.leftArmIK));
			}
		}
		if(currentLeftHandState.stateName == "Has Raised Up")
		{
			distanceBetweenLeftHandAndLeftShoulder = IKArmsControl.CalcDistBetweenLeftHandAndLeftShoulder(playerController.leftArmIK);
			uiDebug.UpdateLeftArmBendingValue(distanceBetweenLeftHandAndLeftShoulder);
			playerController.leftArmBendingValue = distanceBetweenLeftHandAndLeftShoulder;

		}

		if(currentLeftHandState.stateName =="Is Being Guided")
		{
			IKArmsControl.GuideleftHandByMouse(playerController, playerController.leftArmIK);
		}
		

		if(currentLeftHandState.stateName == "Is Holding A Grip" )
		{
			distanceBetweenLeftHandAndLeftShoulder = IKArmsControl.CalcDistBetweenLeftHandAndLeftShoulder(playerController.leftArmIK);
			uiDebug.UpdateLeftArmBendingValue(distanceBetweenLeftHandAndLeftShoulder);
			playerController.leftArmBendingValue = distanceBetweenLeftHandAndLeftShoulder;
		}


		// else
		// {
		// 	distanceBetweenLeftHandAndLeftShoulder = 0f;
		// 	uiDebug.UpdateLeftArmBendingValue(distanceBetweenLeftHandAndLeftShoulder);
		// 	playerController.leftArmBendingValue = distanceBetweenLeftHandAndLeftShoulder;
		// }
	}
#endregion	   

// ------- Left Hand State Management ------- //
#region LeftHandStateManagement

	public void SetLeftHandState(LeftHandState newLeftHandState)
	{
		if (currentLeftHandState != null)
		{
			currentLeftHandState.ExitState(); 
		}

		currentLeftHandState = newLeftHandState;
		currentLeftHandState.EnterState();

		if (uiDebug != null)
		{
			uiDebug.UpdateLeftHandStateUI(currentLeftHandState.stateName);
		}   
	}
	public void ChangeLeftHandStateToHoldingGrip()
	{

		Debug.Log("Changing Left Hand State to Holding Grip");
		ChangeLeftHandState(new LeftHandState_IsHoldingAGrip());
		// IKArmsControl.ChangeIKarmTarget(playerController.leftArmIK, playerController.leftHandHoldingGrip, playerController.leftArmIKTarget);	
	
	}

	public void ChangeLeftHandState(LeftHandState newLeftHandState)
	{
		previousLeftHandState = currentLeftHandState;
		SetLeftHandState(newLeftHandState);
		isLeftHandLoopEnded = false;        
	}

	public void RevertLeftHandState(LeftHandState currentLeftHandState)
	{   
		IKArmsControl.ResetFPSTarget(playerController.leftArmIK, playerController.leftArmIKTarget);        
		ChangeLeftHandState(new LeftHandState_ComingBack());
		playerController.leftHandHoldingAGrip = false; 
		isLeftHandLoopEnded = false;
		leftPalm.SetActive(false);       

	}

	public void EndLeftHandLoop()
	{
		ChangeLeftHandState	(new LeftHandState_DoNothing(playerController, playerController.leftArmIKTarget, playerController.leftBendingIKTarget, playerController.leftArmIK));
		isLeftHandLoopEnded = false;

	}

	public LeftHandState GetCurrentLeftHandState()
	{
		return currentLeftHandState;
	}


#endregion
// -------------------------------------------- //

// ------- Right Hand State Management ------- //
#region  Player Right Hand Loop
	public void PlayRightHandLoop()
	{
		if (currentRightHandState is RightHandState_ComingBack)
		{
			bool completed = IKArmsControl.DecrementRightIkWeight(playerController, playerController.rightArmIK, ref currentRightIkWeight, ref currentRightIkRotationWeight, 2f);
			uiDebug.UpdateRightArmBendingValue(currentRightIkWeight);
			playerController.rightArmBendingValue = currentLeftIkWeight;

			if (completed && !isRightHandLoopEnded)
			{
				EndRightHandLoop();
				isRightHandLoopEnded = true;
			}
		}

		else if (currentRightHandState is RightHandState_IsRisingUp)
		{
			rightPalm.SetActive(true);
			bool completed = IKArmsControl.IncrementRightIkWeight(playerController, playerController.rightArmIK, ref currentRightIkWeight, ref currentRightIkRotationWeight, 2f);
			uiDebug.UpdateRightArmBendingValue(currentRightIkWeight);
			playerController.rightArmBendingValue = currentLeftIkWeight;
			IKArmsControl.CorrectIkHandRaycastDirection(playerController.rightArmIK, rightPalm);
		}
		if(currentRightHandState.stateName == "Is Holding A Grip" )
		{
			distanceBetweenRightHandAndRightShoulder = IKArmsControl.CalcDistBetweenRighttHandAndRightShoulder(playerController.rightArmIK);
			uiDebug.UpdateRightArmBendingValue(distanceBetweenRightHandAndRightShoulder);
			playerController.rightArmBendingValue = distanceBetweenRightHandAndRightShoulder;
		
		}
		// else
		// {
		// 	distanceBetweenRightHandAndRightShoulder = 0f;
		// 	uiDebug.UpdateRightArmBendingValue(distanceBetweenRightHandAndRightShoulder);
		// 	playerController.rightArmBendingValue = distanceBetweenRightHandAndRightShoulder;

		// }
	}
#endregion
// ------------------------------------------ //

// -------------------------------------------- //
#region RightHandStateManagement

	public void SetRightHandState(RightHandState newRightHandState)
	{
		if (currentRightHandState != null)
		{
			currentRightHandState.ExitState();
		}
		currentRightHandState = newRightHandState;
		currentRightHandState.EnterState();
		
		if (uiDebug != null)
		{
			uiDebug.UpdateRightHandStateUI(currentRightHandState.stateName);
		}
	}

	public void ChangeRightHandStateToHoldingGrip()
	{
		ChangeRightHandState(new RightHandState_IsHoldingAGrip());
		IKArmsControl.ChangeIKarmTarget(playerController.rightArmIK, playerController.rightHandHoldingGrip, playerController.rightArmIKTarget);
	}
	public RightHandState GetCurrentRightHandState()
	{
		return currentRightHandState;
	}

	public void ChangeRightHandState(RightHandState newRightHandState)
	{
		previousRightHandState = currentRightHandState;
		SetRightHandState(newRightHandState);
		isRightHandLoopEnded = false;       
	}

	public void RevertRightHandState(RightHandState currentRightHandState)
	{
		IKArmsControl.ResetFPSTarget(playerController.rightArmIK, playerController.rightArmIKTarget);
		SetRightHandState(new RightHandState_ComingBack());
		playerController.rightHandHoldingAGrip = false;
		isRightHandLoopEnded = false;
		rightPalm.SetActive(false);
	}

	public void EndRightHandLoop()
	{
		SetRightHandState(new RightHandState_DoNothing(playerController, playerController.rightArmIKTarget, playerController.rightArmIK));
		isRightHandLoopEnded = true;
	}

	// The principle is merely simple : We have a state machine for the hands of the player, 
	//and we can change the state of the hands of the player by calling the SetLeftHandState and SetRightHandState methods.$
	//Then we can get the current state of the hands of the player by calling the GetCurrentLeftHandState and GetCurrentRightHandState methods.

	#endregion
// -------------------------------------------- //

// ------- Scroll Management ------- //
#region ScrollManagement
	public void OnMouseScrollDetected(float adjustmentValue)
	{
		
		if (currentLeftHandState is leftHandState_HasRaisedUp)
		{
			isScrolling = true;
			IKArmsControl.ControlLeftArmBendingOnMouseScroll(playerController.leftArmIK, adjustmentValue, playerController.leftBendingIKTarget);
			
		}

		// IKArmsControl.ControlLeftArmBendingOnMouseScroll(playerController.leftArmIK, ref currentLeftIkWeight, ref currentLeftIkRotationWeight, scrollAdjustmentValue);	
		
	}

	public void StopMouseScroll()
	{
		if (currentLeftHandState is leftHandState_HasRaisedUp)
		{
			isScrolling = false;
			// Debug.LogWarning("Mouse Scroll Stopped");
		}
	}

	#endregion
// -------------------------------------------- //

// ---------- Ik Target Management (to be refactored later) ---------- //
#region IkTargetManagement

	public void changeLeftHandIkTargetOnScroll()
	{
		if(currentLeftHandState is leftHandState_HasRaisedUp && canChangeLeftIkTargetOnScroll)
		{
			// Debug.LogWarning("Changing Left Hand IK Target on Scroll");
			IKArmsControl.ChangeIkArmTargetToBendingTarget(playerController.leftArmIK, playerController.leftBendingIKTarget);			
			canChangeLeftIkTargetOnScroll = false;
		}

	}
	public void ResetCanChangeLeftIkTargetOnScroll()
	{
		// Debug.LogWarning("Resetting Can Change Left IK Target on Scroll");
		canChangeLeftIkTargetOnScroll = true;
		
	}


	#endregion
// -------------------------------------------- //
}

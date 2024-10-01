using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_AgainstWall : PlayerState_Moving
{
	protected CharacterController Controller;
	protected new Vector2 moveDirection;
	protected new Vector2 forwardBackward;
	protected new Vector2 rightLeft;
	protected new Vector2 combinedMovement;
	protected float rightarmsBendingValue;
	protected float leftarmsBendingValue;
	protected PlayerController playerController;

	protected GameObject leftIkTarget;

	protected float lateralLimit = 0.25f;

	protected bool isLeftHandHoldingGrip;
	protected bool isRightHandHoldingGrip;
	protected bool areBothHandsHoldingGrip;

	protected bool canMoveLeft;
	protected bool canMoveRight;
	
	public PlayerState_AgainstWall(PlayerController playerController, CharacterController characterController, Vector2 moveDirection, float speed, Vector2 combinedMovement, 
	Vector2 fowardBackward, Vector2 rightLeft, float rightarmsBendingValue, float leftarmsBendingValue, bool isLeftHandHoldingGrip, bool isRightHandHoldingGrip, bool areBothHandsHoldingGrip)

	: base(characterController, moveDirection, speed , combinedMovement, fowardBackward, rightLeft)
	{
		this.rightarmsBendingValue = rightarmsBendingValue;
		this.leftarmsBendingValue = leftarmsBendingValue;
		this.playerController = playerController;
		this.isLeftHandHoldingGrip = isLeftHandHoldingGrip;
		this.isRightHandHoldingGrip = isRightHandHoldingGrip;
		this.areBothHandsHoldingGrip = areBothHandsHoldingGrip;
		stateName = "Against Wall";

	}

	public override void ExecuteState()
	{
		isLeftHandHoldingGrip = playerController.leftHandHoldingAGrip;
		isRightHandHoldingGrip = playerController.rightHandHoldingAGrip;
		areBothHandsHoldingGrip = isLeftHandHoldingGrip && isRightHandHoldingGrip;
		
		//Debug.Log("Executing Against Wall State | " + " Current is Left Hand Holding A Grip : "+ isLeftHandHoldingGrip + " ! " + " Current is Right Hand Hold A Grip : " + isRightHandHoldingGrip + " ! " + " Are Both Hands Holding Grip : " + areBothHandsHoldingGrip + " ! ");
		rightarmsBendingValue = playerController.rightArmBendingValue;
		leftarmsBendingValue = playerController.leftArmBendingValue;
		forwardBackward = playerController.playerSetDirection.GetForwardDirection();
		
		rightLeft = playerController.playerSetDirection.GetRightDirection();

		if(isLeftHandHoldingGrip)
		{
			leftIkTarget = playerController.leftHandHoldingGrip;
		}
		if(isRightHandHoldingGrip)
		{
			leftIkTarget = playerController.rightHandHoldingGrip;
		}
		if(isLeftHandHoldingGrip && isRightHandHoldingGrip)
		{
			areBothHandsHoldingGrip = true;
		}		
		
		//calc lateral limits based on the position of the hand holding the grip
		Vector3 leftLimit = leftIkTarget.transform.position + leftIkTarget.transform.right * lateralLimit;
		Vector3 rightLimit = leftIkTarget.transform.position - leftIkTarget.transform.right * lateralLimit;
		Vector3 playerPosition = controller.transform.position;

		//determine lateral limits the plan can move in when both hands are holding a 
		if(areBothHandsHoldingGrip)
		{
			canMoveLeft = playerPosition.x  > leftLimit.x - 0.5f;
			canMoveRight = playerPosition.x < rightLimit.x - 0.5f;
			// Debug.Log("Both Hands Holding Grip");
		}
		else
		{
			canMoveLeft = playerPosition.x  > leftLimit.x;
			canMoveRight = playerPosition.x < rightLimit.x;
		}

		bool canMoveBackward = forwardBackward.y < 0f && leftarmsBendingValue <1f && rightarmsBendingValue <1f;
		bool canMoveForward = forwardBackward.y > 0f && (leftarmsBendingValue > 0.15f || rightarmsBendingValue > 0.15f);
		bool canMoveLaterally = (rightLeft.x < 0f && canMoveLeft) || (rightLeft.x > 0f && canMoveRight);

		//Debug.Log(" Forward Backward value : " + forwardBackward.y + "Left Arm Bending value : " + leftarmsBendingValue + " Right Arm Bending Value : " + rightarmsBendingValue);

		if(canMoveBackward || canMoveForward || canMoveLaterally)
		{
			base.ExecuteState();
		}
	}
}

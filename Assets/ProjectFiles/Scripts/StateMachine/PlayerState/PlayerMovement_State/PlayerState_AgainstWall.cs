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
	protected float speed;
	protected float rightarmsBendingValue;
	protected float leftarmsBendingValue;
	protected PlayerController playerController;

	protected GameObject leftIkTarget;

	protected float lateralLimit = 0.5f;
	
	public PlayerState_AgainstWall(PlayerController playerController, CharacterController characterController, Vector2 moveDirection, float speed, Vector2 combinedMovement, 
	Vector2 fowardBackward, Vector2 rightLeft, float rightarmsBendingValue, float leftarmsBendingValue)

	: base(characterController, moveDirection, speed , combinedMovement, fowardBackward, rightLeft)
	{
		this.rightarmsBendingValue = rightarmsBendingValue;
		this.leftarmsBendingValue = leftarmsBendingValue;
		this.playerController = playerController;
		stateName = "Against Wall";

	}

	public override void ExecuteState()
	{
		rightarmsBendingValue = playerController.rightArmBendingValue;
		leftarmsBendingValue = playerController.leftArmBendingValue;
		forwardBackward = playerController.playerSetDirection.GetForwardDirection();
		rightLeft = playerController.playerSetDirection.GetRightDirection();
		leftIkTarget = playerController.leftHandHoldingGrip;

		//Debug.Log("Executing Against Wall State : Right Arm Bending Value : " + rightarmsBendingValue + " Left Arm Bending Value : " + leftarmsBendingValue);
		//Debug.Log("Executing Against Wall State : Forward Backward : " + forwardBackward + " left arms bending value : " + leftarmsBendingValue );
		

		Vector3 leftLimit = leftIkTarget.transform.position + leftIkTarget.transform.right * lateralLimit;
		Vector3 rightLimit = leftIkTarget.transform.position - leftIkTarget.transform.right * lateralLimit;
		Vector3 playerPosition = controller.transform.position;

		bool canMoveLeft = playerPosition.x  > leftLimit.x;
		bool canMoveRight = playerPosition.x < rightLimit.x;



		if ((forwardBackward.y < 0f && leftarmsBendingValue < 1f) ||
		 (rightLeft.x < 0f && canMoveLeft) ||
		  (rightLeft.x > 0f && canMoveRight))
		{

			base.ExecuteState();
		}
		else if((forwardBackward.y > 0f && leftarmsBendingValue > 0.15f) || 
		(rightLeft.x < 0f && canMoveLeft) || 
		(rightLeft.x > 0f  &&  canMoveRight))
		{

			base.ExecuteState();
		}


	
	}
}

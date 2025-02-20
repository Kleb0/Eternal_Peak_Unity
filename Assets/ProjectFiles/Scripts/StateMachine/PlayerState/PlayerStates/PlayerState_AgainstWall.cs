using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_AgainstWall : PlayerState_Moving
{
	protected PlayerController playerController;
	protected CharacterController Controller;
	protected new Vector2 moveDirection;
	protected new Vector2 forwardBackward;
	protected new Vector2 rightLeft;
	protected new Vector2 combinedMovement;
	protected float rightarmsBendingValue;
	protected float leftarmsBendingValue;


	protected GameObject leftIkTarget;
	protected GameObject rightIkTarget;

	protected float rangeLimit;

	protected bool isLeftHandHoldingGrip;
	protected bool isRightHandHoldingGrip;
	protected bool areBothHandsHoldingGrip;

	protected bool canMoveLeft;
	protected bool canMoveRight;
	
	public PlayerState_AgainstWall(PlayerController playerController, 
	CharacterController characterController, 
	float rangeLimit,
	Vector2 moveDirection, 
	float speed, 
	Vector2 combinedMovement, 
	Vector2 fowardBackward, 
	Vector2 rightLeft, 
	float rightarmsBendingValue, 
	float leftarmsBendingValue, 
	bool isLeftHandHoldingGrip, 
	bool isRightHandHoldingGrip, 
	bool areBothHandsHoldingGrip,
	GameObject leftIkTarget,
	GameObject rightIkTarget)
	: base(characterController, moveDirection, speed, combinedMovement, fowardBackward, rightLeft)
	{
		this.playerController = playerController;
		this.Controller = characterController; 
		this.rangeLimit = rangeLimit;
		this.rightarmsBendingValue = rightarmsBendingValue;
		this.leftarmsBendingValue = leftarmsBendingValue;
		this.playerController = playerController;
		this.isLeftHandHoldingGrip = isLeftHandHoldingGrip;
		this.isRightHandHoldingGrip = isRightHandHoldingGrip;
		this.areBothHandsHoldingGrip = areBothHandsHoldingGrip;
		this.moveDirection = moveDirection;
		this.combinedMovement = combinedMovement;
		this.forwardBackward = fowardBackward;
		this.rightLeft = rightLeft;
		this.speed = speed;
		this.leftIkTarget = playerController.leftHandHoldingGrip;
		this.rightIkTarget = playerController.rightHandHoldingGrip;
		stateName = "Against Wall";
	}

	public override void EnterState()
	{
		base.EnterState();
		// Debug.Log("Entering Against Wall");
		// Debug.Log($"Left Ik Target: {leftIkTarget}");
		// playerController.isInAir = true;
	}

	public override void ExecuteState()
	{
		Vector3 leftIkTargetPosition; Vector3 rightIkTargetPosition;
		bool canMoveBackward, canMoveForward, canMoveLaterally;


		PlayerProcessMoveAgainstWall.MoveAgainstWall(
			this.playerController,
			this.Controller,
			rangeLimit,
			out canMoveLeft,
			out canMoveRight,
			out areBothHandsHoldingGrip,
			out leftIkTargetPosition,
			out rightIkTargetPosition,
			out canMoveBackward,
			out canMoveForward,
			out canMoveLaterally,
			leftIkTarget,
			rightIkTarget
	
		);

		if(canMoveBackward || canMoveForward || canMoveLaterally)
		{
			base.ExecuteState();
		}

	}
	public override void ExitState()
	{
		// Debug.Log("Exiting Against Wall");
		// playerController.isInAir = false;
		base.ExitState();
	}
}

using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class LeftHandState_IsRisingUp : LeftHandState
{
	public override string stateName { get; protected set; } = "Is rising up";

	// gets this state a parameter with the class ArmIK
	protected PlayerController playerController;
	protected ArmIK LeftArmIK;    // solver arm
	protected IKSolverArm LeftIKSolverArm;
	protected float reachingSpeed;
	protected GameObject leftArmIkTarget;
	protected GameObject leftBendingTarget;

	protected float IKWeight = 0f;
	protected float IKRotationWeight;
	private float rate = 1f;
	protected float target = 1f;

	// the constructor will take the playerController, the leftArmIkTarget, the leftArmIK, the leftIKSolverArm, the IKWeight and the IKRotationWeight
	// from the playerController
	// and initialize them in the state
	 public LeftHandState_IsRisingUp(PlayerController playerController, GameObject LeftArmIkTarget, GameObject leftBendingTarget, ArmIK leftArmIk, IKSolverArm leftIKSolverArm, float IKWeight, float IKRotationWeight, ArmIK armIK)
	{
		this.playerController = playerController;
		this.leftArmIkTarget = playerController.leftArmIKTarget;
		this.LeftArmIK = playerController.leftArmIK;
		this.LeftIKSolverArm = playerController.leftIKSolverArm;
		this.IKWeight = playerController.leftIKSolverArm.IKPositionWeight;
		this.IKRotationWeight = playerController.leftIKSolverArm.IKRotationWeight;
		this.leftBendingTarget = playerController.leftBendingIKTarget;
		
	  
		// As we are using here a special constructor, we just set the stateName to "Moving" here without the need to override it in the derived classes.
	}

	// when we enter in the state, we enable the target of the left arm, and the left arm itself

	public override void EnterState()
	{
		Debug.Log("Entering LeftHandState_IsRisingUp");
		IKArmsControl.EnableIKTarget(playerController, leftArmIkTarget, leftBendingTarget, LeftArmIK);
		IKArmsControl.EnableIkArm(LeftArmIK);

	}

	// to enable the state, will pass the playerController, the left Arm itself, the left arm solver, the reaching speed 
	// use a function to be able to change the target of the left arm

	public override void ExecuteState()
	{
		IKArmsControl.IncrementLeftIkWeight(playerController, LeftArmIK, ref IKWeight, ref IKRotationWeight, rate);

	}
}
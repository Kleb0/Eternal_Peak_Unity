using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class RightHandState_DoNothing : RightHandState
{

	PlayerController playerController;
	GameObject rightArmIKTarget;
	ArmIK rightArmIK;
	
	public override string stateName { get; protected set; } = "Do Nothing";

	public override void EnterState()
	{
		Debug.Log("TEST Enter RightHandState with name " + stateName);
		IKArmsControl.DisableIKTarget(playerController.rightArmIKTarget);
		IKArmsControl.DisableIkArm(playerController.rightArmIK);
	}

	public RightHandState_DoNothing(PlayerController playerController, GameObject rightArmIKTarget, ArmIK rightArmIK)
	{
		this.playerController = playerController;
		this.rightArmIKTarget = playerController.rightArmIKTarget;
		this.rightArmIK = playerController.rightArmIK;
	}
}

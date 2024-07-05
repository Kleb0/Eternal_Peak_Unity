using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class LeftHandState_DoNothing : LeftHandState
{
	// Start is called before the first frame update

	PlayerController playerController;
	GameObject leftArmIKTarget;
	ArmIK leftArmIK;
  
	public override string stateName { get; protected set; } = "Do Nothing";   

	public LeftHandState_DoNothing(PlayerController playerController, GameObject leftArmIKTarget, ArmIK leftArmIK)
	{
		this.playerController = playerController;
		this.leftArmIKTarget = playerController.leftArmIKTarget;
		this.leftArmIK = playerController.leftArmIK;
	}

	public override void EnterState()
	{
		IKArmsControl.DisableIKTarget(playerController.leftArmIKTarget);
		IKArmsControl.DisableIkArm(playerController.leftArmIK);
	}

	public override void ExecuteState()
	{

	}
}

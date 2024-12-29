using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class RightHandState_HasRaisedUp : RightHandState
{

    public override string stateName { get; protected set; } = "Has Raised Up";

    protected PlayerController playerController;
    protected ArmIK RightArmIK;
    protected IKSolverArm RightIKSolverArm;
    protected GameObject rightArmIkTarget;
    protected GameObject rightBendingTarget;

    public RightHandState_HasRaisedUp(PlayerController playerController, GameObject rightArmIktarget, GameObject rightBendingTarget, IKSolverArm rightIkSolverArm, ArmIK rightArmIk)
    {
        this.playerController = playerController;
        this.rightArmIkTarget = playerController.rightArmIKTarget;
        this.RightArmIK = playerController.rightArmIK;
        this.RightIKSolverArm = playerController.rightIKSolverArm;
        this.rightBendingTarget = playerController.rightBendingIKTarget;

    }

}

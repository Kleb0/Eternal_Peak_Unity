using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class leftHandState_HasRaisedUp : LeftHandState
{
    public override string stateName { get; protected set; } = "Has Raised Up";

    protected PlayerController playerController;
    protected ArmIK LeftArmIK; 
    protected IKSolverArm LeftIKSolverArm;
    protected GameObject leftArmIkTarget;
    protected GameObject leftBendingTarget;

    public leftHandState_HasRaisedUp(PlayerController playerController, GameObject leftArmIkTarget, GameObject leftBendingTarget, IKSolverArm leftIKSolverArm, ArmIK leftArmIk)
    {
        this.playerController = playerController;
        this.leftArmIkTarget = playerController.leftArmIKTarget;
        this.LeftArmIK = playerController.leftArmIK;
        this.LeftIKSolverArm = playerController.leftIKSolverArm;
        this.leftBendingTarget = playerController.leftBendingIKTarget;

    }

    public override void EnterState()
    {
        // Debug.Log("Entering LeftHandState_HasRaisedUp");
    }

    public override void ExecuteState()
    {
    
    }

    public override void ExitState()
    {
    
    }



}

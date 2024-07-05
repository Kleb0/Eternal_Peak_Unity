using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using Unity.Burst.Intrinsics;

public class RightHandState_IsRisingUp : RightHandState
{
    public override string stateName { get; protected set; } = "Is rising up";  

    protected PlayerController playerController;

    protected ArmIK rightArmIK;
    protected IKSolverArm rightArmSolver;
    protected float reachingSpeed;
    protected GameObject rightHandIKTarget;

    
    protected float IKWeight = 0f;
    protected float IKRotationWeight;


    private float rate = 1f;
    protected float target = 1f;

    public RightHandState_IsRisingUp(PlayerController playerController, GameObject rightHandIkTarget, ArmIK rightArmIK, IKSolverArm rightArmSolver, float IKWeight, float IKRotationWeight)
    {
        this.playerController = playerController;
        this.rightArmIK = playerController.rightArmIK;
        this.rightArmSolver = playerController.rightIKSolverArm;
        this.IKWeight = playerController.rightIKSolverArm.IKPositionWeight;
        this.IKRotationWeight = playerController.rightIKSolverArm.IKRotationWeight;
        this.rightHandIKTarget = playerController.rightArmIKTarget;
    }

    public override void EnterState()
    {
        IKArmsControl.EnableIKTarget(rightHandIKTarget);
        IKArmsControl.EnableIkArm(rightArmIK);
    }

    public override void ExecuteState()
    {
        IKArmsControl.IncrementRightIkWeight(rightArmIK, ref IKWeight, ref IKRotationWeight, rate);
    }

    

   
}

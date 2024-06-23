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
    protected PlayerAnimation playerAnimation;

    protected float IKWeight = 0f;
    protected float IKRotationWeight;

    private float rate = 1f;
    protected float target = 1f;


    // to enable the state, will pass the playerController, the left Arm itself, the left arm solver, the reaching speed 
    // use a function to be able to change the target of the left arm
    public LeftHandState_IsRisingUp() 
    {
       

        //here we enable the Left Arm IK at the creation of the state
    }

    public override void ExecuteState()
    {
        Debug.Log("Left hand state Rising Up is executed");
        
        
        // IKWeight = RiseHand.IncrementIKWeight(IKWeight, rate);
        // Debug.Log("IKWeight value: " + IKWeight);
    }

    public override void ExitState()
    {
      
        // RiseHand.DisableTarget(leftArmIkTarget);
        IKWeight = 0f;
    }

    public float incrementationThroughTime( float incrementedweight)
    {
        incrementedweight += 0.1f * Time.deltaTime;
        return incrementedweight;

    }


}


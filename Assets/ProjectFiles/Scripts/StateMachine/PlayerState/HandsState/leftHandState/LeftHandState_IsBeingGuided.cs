using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class leftHandState_isBeingGuided : LeftHandState
{
    public override string stateName { get; protected set; } = "Is Being Guided";

    //here at start we should call IKArmsControl.GuideHandByMouse(playerController, LeftArmIK);

}
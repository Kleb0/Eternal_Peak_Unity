using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandState : State
{
    public override string stateName { get; protected set; } = "LeftHandState";
    
    public override void EnterState()
    {
        Debug.Log("Enter LeftHandState with name " + stateName);
        
    }

    public override void ExitState()
    {
        Debug.Log("Exit LeftHandState with name " + stateName);
    }

}

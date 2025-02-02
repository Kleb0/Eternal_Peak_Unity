using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Idle : PlayerState
{ 
    public override string stateName { get; protected set; } = "Idle";

    public override void EnterState()
    {
        Debug.Log("Start Idle");
    }
}

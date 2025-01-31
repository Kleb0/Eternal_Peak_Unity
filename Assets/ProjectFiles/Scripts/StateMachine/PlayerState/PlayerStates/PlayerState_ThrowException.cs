using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_ThrowException : PlayerState
{
    public override string stateName { get; protected set; } = "Throw Exception";
    
    public override void EnterState()
    {
        throw new System.NotImplementedException();
    }
}

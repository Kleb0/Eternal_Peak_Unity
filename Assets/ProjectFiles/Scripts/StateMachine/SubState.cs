using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubState : State
{
    // we will use this class to create substates later

    public string parentStateName { get; protected set; } = "ParentState";
    public string subStateName { get; protected set; } = "SubState";
    public override string stateName { get; protected set; }

    public SubState(string parentStateName, string subStateName)
    {
        this.parentStateName = parentStateName;
        this.subStateName = subStateName;
        this.stateName = parentStateName + "/" + subStateName;
    }

    
    
}

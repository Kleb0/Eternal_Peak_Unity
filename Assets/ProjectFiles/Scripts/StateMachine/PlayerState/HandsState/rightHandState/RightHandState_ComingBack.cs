using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandState_ComingBack : RightHandState
{
    // Start is called before the first frame update
   public override string stateName { get; protected set; } = "Coming back";

    public GameObject rightHandIKTarget;
}

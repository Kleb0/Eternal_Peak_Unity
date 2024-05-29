using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Idle : PlayerState
{ 

    public PlayerState_Idle()
    {
        stateName = "Idle";
    }

    public override void ExecuteState()
    {
        // here we implement the logic for the walking state, as PlayerState_Walking is a subclass of PlayerState
        // we don't need to implement the EnterState and ExitState methods as they already have been implemented in the PlayerState class

    }
  
   
}

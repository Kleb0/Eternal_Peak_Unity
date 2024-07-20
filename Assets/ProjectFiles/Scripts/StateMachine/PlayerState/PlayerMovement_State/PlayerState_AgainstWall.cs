using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_AgainstWall : PlayerState
{
    
    public PlayerState_AgainstWall()
    {
        stateName = "Against Wall";
        // As we are using here a special constructor, we just set the stateName to "AgainstWall" here without the need to override it in the derived classes.
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_WalkingAgainstWall : PlayerState_Moving
{
    // Start is called before the first frame update

    float armsBendingValue = 0f;
    public PlayerState_WalkingAgainstWall(CharacterController controller, Vector2 moveDirection, float speed, Vector2 combinedMovement, Vector2 fowardBackward, Vector2 rightLeft, float armsBendingValue) 
        : base(controller, moveDirection, speed, combinedMovement, fowardBackward, rightLeft)
    {
        this.armsBendingValue = armsBendingValue;
        stateName = "Walking against a wall";
        // As we are using here a special constructor, we just set the stateName to "Walking Against Wall" here without the need to override it in the derived classes.
    }

    public override void ExecuteState()
    {
        if (armsBendingValue > 0f && armsBendingValue < 1f)
        {
            PlayerMove.Move(controller, moveDirection, speed, forwardBackward, rightLeft);
        }
     
      
       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Running : PlayerState_Moving
{
    public PlayerState_Running(CharacterController controller, Vector2 moveDirection, float speed, 
        Vector2 combinedMovement, Vector2 fowardBackward, Vector2 rightLeft) 
        : base(controller, moveDirection, speed, combinedMovement, fowardBackward, rightLeft)
    {
        stateName = "Running";
    }
    public override void ExecuteState()
    {
        PlayerRunning.Move(controller, moveDirection, speed, forwardBackward, rightLeft);
    }
     
}
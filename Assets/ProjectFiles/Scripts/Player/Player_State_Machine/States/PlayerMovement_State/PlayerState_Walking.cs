using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerState_Walking : PlayerState_Moving
{
    public PlayerState_Walking(CharacterController controller, Vector2 moveDirection, float speed, Vector2 combinedMovement, Vector2 fowardBackward, Vector2 rightLeft) 
    : base(controller, moveDirection, speed , combinedMovement, fowardBackward, rightLeft)
    {
        stateName = "Walking";
    }
   
}

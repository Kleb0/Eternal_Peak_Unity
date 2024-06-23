using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerState_Moving : PlayerState
{

    
    protected CharacterController controller;
    protected Vector2 moveDirection;
    protected float speed;

    protected Vector2 combinedMovement;
    protected Vector2 forwardBackward;
    protected Vector2 rightLeft;

    public PlayerState_Moving(CharacterController controller, Vector2 moveDirection, float speed, Vector2 combinedMovement, Vector2 fowardBackward, Vector2 rightLeft)
    {
        this.controller = controller;
        this.moveDirection = moveDirection;
        this.speed = speed;
        this.combinedMovement = combinedMovement;
        this.forwardBackward = fowardBackward;
        this.rightLeft = rightLeft;
        stateName = "Moving";
        // As we are using here a special constructor, we just set the stateName to "Moving" here without the need to override it in the derived classes.
    }
    public override void ExecuteState()
    {
        
        PlayerMove.Move(controller, moveDirection, speed, forwardBackward, rightLeft);
    }

    public void UpdateDirections(Vector2 forwardBackward, Vector2 rightLeft)
    {
        this.forwardBackward = forwardBackward;
        this.rightLeft = rightLeft;
        this.moveDirection = new Vector2(rightLeft.x, forwardBackward.y);
        
    }

}

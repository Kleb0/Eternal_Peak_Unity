using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Running : PlayerState
{
    
    private CharacterController controller;
    private Vector2 moveDirection;
    private float speed;

    public PlayerState_Running(CharacterController controller,  Vector2 moveDirection, float speed)
    {
        this.controller = controller;
        this.moveDirection = moveDirection;
        this.speed = speed;
        stateName = "Running";
    }
    
    public override void ExecuteState()
    {
        PlayerRunning.Move(controller, moveDirection, speed);
    }
     
}

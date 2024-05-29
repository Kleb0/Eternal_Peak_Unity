using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Running : PlayerState
{
    
    private CharacterController controller;
    private float speed;

    public PlayerState_Running(CharacterController controller, float speed)
    {
        this.controller = controller;
        this.speed = speed;
        stateName = "Running";
    }
    
    public override void ExecuteState()
    {
        PlayerRunning.Move(controller, speed);
    }
     
}

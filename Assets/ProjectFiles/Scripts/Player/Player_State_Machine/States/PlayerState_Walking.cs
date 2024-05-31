using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerState_Walking : PlayerState
{
    private CharacterController controller;
    private Vector2 moveDirection;
    private float speed;
    public PlayerState_Walking(CharacterController controller, Vector2 moveDirection, float speed)
    {
        this.controller = controller;
        this.moveDirection = moveDirection;
        this.speed = speed;
        stateName = "Walking";
    }
    public override void ExecuteState()
    {
        // here we implement the logic for the walking state, as PlayerState_Walking is a subclass of PlayerState
        // we don't need to implement the EnterState and ExitState methods as they already have been implemented in the PlayerState class 
        PlayerMove.Move(controller, moveDirection, speed);   
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_isJumping : PlayerState
{
        private PlayerController playerController;
        private CharacterController controller;
        private HandsStateController handsStateController;
        private Vector2 inputDirection;
        private float jumpSpeed;
        private float currentSpeed;


    public PlayerState_isJumping(PlayerController playerController, CharacterController controller, HandsStateController handsStateController, Vector2 inputDirection, float jumpSpeed, float currentSpeed)
    {
        this.playerController = playerController;
        this.controller = controller;
        this.handsStateController = handsStateController;
        this.inputDirection = inputDirection;
        this.jumpSpeed = jumpSpeed;
        this.currentSpeed = currentSpeed;

        stateName = "is Jumping";
    }

    public override void EnterState()
    {
        Debug.Log("Start Jumping");
    }

    public override void ExecuteState()
    {
        PlayerProcessJumping.ProcessJumping(playerController, controller, handsStateController, inputDirection, jumpSpeed, currentSpeed);
        
       
    }

    public override void ExitState()
    {
        Debug.Log("End Jumping");
    }


}

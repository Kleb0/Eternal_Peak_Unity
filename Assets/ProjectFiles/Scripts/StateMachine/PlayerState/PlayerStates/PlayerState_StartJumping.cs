using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_StartJumping : PlayerState_Moving
{
    protected PlayerController playerController;
    protected CharacterController Controller;
    protected HandsStateController handsStateController;
    protected new Vector2 moveDirection;
    protected new Vector2 forwardBackward;
    protected new Vector2 rightLeft;
    protected new Vector2 combinedMovement;

    protected float jumpHeight;
    protected float jumpSpeed;

    protected float currentSpeed;
    protected float verticalVelocity;

    protected bool isLeftHandHoldingGrip;
    protected bool isRightHandHoldingGrip;
    protected bool areBothHandsHoldingGrip;

    public PlayerState_StartJumping(
        PlayerController playerController,
        CharacterController characterController,
        HandsStateController handsStateController,
        Vector2 moveDirection,
        float jumpSpeed,
        float currentSpeed,
        float jumpHeight,
        Vector2 forwardBackward,
        Vector2 rightLeft
    )
        : base(characterController, moveDirection, jumpSpeed, Vector2.zero, forwardBackward, rightLeft)
    {
        this.playerController = playerController;
        Controller = characterController;
        this.handsStateController = handsStateController;
        this.jumpHeight = jumpHeight;
        this.jumpSpeed = jumpSpeed;
        this.currentSpeed = currentSpeed;
        this.moveDirection = moveDirection;
        this.forwardBackward = forwardBackward;
        this.rightLeft = rightLeft;

        stateName = "Start Jumping";
    }

    public override void ExecuteState()
    {
        PlayerStartJump.StartJump(playerController, Controller, handsStateController, new Vector2(0, jumpHeight), 
        moveDirection, jumpSpeed , currentSpeed);
    }


}

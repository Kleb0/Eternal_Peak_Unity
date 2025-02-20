using UnityEngine;

using System.Collections;
using System.Collections.Generic;


public class PlayerState_StartFalling : PlayerState_Moving
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

    public PlayerState_StartFalling(
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

        stateName = "Start Falling";
    }

    public override void EnterState()
    {
        Debug.Log("Entering Start Falling State");

    }


}

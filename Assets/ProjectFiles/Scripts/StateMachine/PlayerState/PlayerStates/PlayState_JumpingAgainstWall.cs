using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_JumpingAgainstWall : PlayerState_Moving
{
    protected PlayerController playerController;
    protected CharacterController Controller;
    protected HandsStateController handStateController;
    protected new Vector2 moveDirection;
    protected new Vector2 forwardBackward;
    protected new Vector2 rightLeft;
    protected new Vector2 combinedMovement;
    protected float rightArmsBendingValue;
    protected float leftArmsBendingValue;

    protected bool isLeftHandHoldingGrip;
    protected bool isRightHandHoldingGrip;
    protected bool areBothHandsHoldingGrip;

    protected float jumpHeight;
    protected float jumpSpeed;

    public PlayerState_JumpingAgainstWall(
        PlayerController playerController,
        CharacterController characterController,
        HandsStateController handsStateController,
        Vector2 moveDirection,
        float jumpSpeed,
        float jumpHeight,
        Vector2 forwardBackward,
        Vector2 rightLeft
    )
        : base(characterController, moveDirection, jumpSpeed, Vector2.zero, forwardBackward, rightLeft)
    {
        this.playerController = playerController;
        this.Controller = characterController;
        this.handStateController = handsStateController;
        this.jumpHeight = jumpHeight;
        this.jumpSpeed = jumpSpeed;
        this.moveDirection = moveDirection;
        this.forwardBackward = forwardBackward;
        this.rightLeft = rightLeft;


        // this.isLeftHandHoldingGrip = false;
        // this.isRightHandHoldingGrip = false;
        // this.areBothHandsHoldingGrip = false;

        stateName = "Jumping Against Wall";
    }

    public override void ExecuteState()
    {
        // Utilise la logique du saut contre le mur
        playerController.currentPlayerState = this;
        // PlayerJump.Jump(playerController, Controller, this.handStateController, new Vector2(0, jumpHeight), moveDirection, jumpSpeed);
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Enter in state Jumping Against Wall");
    }
}

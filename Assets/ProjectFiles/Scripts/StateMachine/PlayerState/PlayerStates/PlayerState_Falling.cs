using UnityEngine;

public class PlayerState_Falling : PlayerState_Moving
{
    private PlayerController playerController;
    private HandsStateController handsStateController;
    private Vector2 inputDirection;
    private float fallSpeed;
    private float currentSpeed;

    public PlayerState_Falling(PlayerController playerController, CharacterController controller, HandsStateController handsStateController, Vector2 inputDirection, float fallSpeed, float currentSpeed)
        : base(controller, inputDirection, fallSpeed, Vector2.zero, Vector2.zero, Vector2.zero)
    {
        this.playerController = playerController;
        this.controller = controller;
        this.handsStateController = handsStateController;
        this.inputDirection = inputDirection;
        this.fallSpeed = fallSpeed;
        this.currentSpeed = currentSpeed;

        stateName = "Falling";
    }
}
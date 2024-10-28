using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class PlayerState_Jumping : PlayerState_Moving
{
	private float jumpHeight; 
	private float jumpSpeed;

	private PlayerController playerController;

	public PlayerState_Jumping(PlayerController playerController, CharacterController controller, Vector2 moveDirection, float jumpSpeed, float jumpHeight, Vector2 forwardBackward, Vector2 rightLeft)
	: base(controller, moveDirection, jumpSpeed, Vector2.zero, forwardBackward, rightLeft)
	{
		this.jumpHeight = jumpHeight;
		this.jumpSpeed = jumpSpeed;
		this.playerController = playerController;
		stateName = "Jumping";

	}

    public override void ExecuteState()
    {
	

        PlayerJump.Jump(playerController, controller, new Vector2(0, jumpHeight), moveDirection, jumpSpeed);
    }

    public override void EnterState()
    {
        base.EnterState();
		playerController = controller.GetComponent<PlayerController>();
    }

	public override void ExitState()
	{
		base.ExitState();
	}

}

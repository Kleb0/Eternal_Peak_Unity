using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : PlayerMove
{

	private static float gravity = -9.81f;

	public static void Jump(PlayerController playerController, CharacterController controller, Vector2 jumpHeight, Vector2 inputDirection, float jumpSpeed)
	{

		if (controller == null)
		{
			Debug.LogError("Controller is null");
			return;
		}

		if (controller.isGrounded && playerController.canJump && playerController.haspressedJump)
		{
			playerController.verticalVelocity = Mathf.Sqrt(jumpHeight.y * -2f * gravity);
			playerController.isInAir = true;
			playerController.canJump = false;
			// Debug.Log("Jumping start vertical velocity is : " + playerController.verticalVelocity);
		}

		if (playerController.isInAir)
		{
			playerController.verticalVelocity += gravity * Time.deltaTime;


			Vector3 jumpMove = playerController.transform.forward * inputDirection.y + playerController.transform.right * inputDirection.x;
			jumpMove.y = playerController.verticalVelocity;
			// Vector3 jumpMove = new(jumpDirection.x, playerController.verticalVelocity, jumpDirection.y);
			controller.Move(jumpSpeed * Time.deltaTime * jumpMove);

			if (controller.isGrounded)
			{
				playerController.isInAir = false;
				playerController.canJump = true;
				playerController.haspressedJump = false;
				// Debug.Log("Jumping end");
			}

		}
	 }
}

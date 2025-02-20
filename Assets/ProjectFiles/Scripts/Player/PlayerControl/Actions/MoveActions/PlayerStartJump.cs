using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStartJump : PlayerMove
{

	private static float gravity = -9.81f;
	public static void StartJump(
		PlayerController playerController, 
		CharacterController controller, 
		HandsStateController handsStateController, 
		Vector2 jumpHeight, 
		Vector2 inputDirection, 
		float jumpSpeed, 
		float currentSpeed)
	{
	
		// Début du saut
		if (playerController.isGrounded)
		{
			playerController.verticalVelocity = Mathf.Max(Mathf.Sqrt(jumpHeight.y * -2f * gravity), 0.1f);	
			playerController.isInAir = true;	
		}		

	 }
}

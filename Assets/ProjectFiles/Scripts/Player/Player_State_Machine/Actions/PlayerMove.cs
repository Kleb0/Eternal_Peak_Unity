using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove
{
	public static void Move(CharacterController controller, Vector2 moveDirection, float speed, Vector2 forwardBackWard, Vector2 rightLeft)
	{				

	
		Vector3 move = Vector3.zero;

		
		// Vector3 move;

		// chec=k if one vector is active


		if (forwardBackWard != Vector2.zero)
		{
			move += controller.transform.forward * forwardBackWard.y;				
			
		}

		if (rightLeft != Vector2.zero)
		{
			move += controller.transform.right * rightLeft.x;
		}

		if (forwardBackWard != Vector2.zero && rightLeft != Vector2.zero)
		{
			// combine the two vectors for diagonal movement
			Vector2 combinedMovement = forwardBackWard + rightLeft;
			move += controller.transform.right * combinedMovement.x + controller.transform.forward * combinedMovement.y;
		}

		if (move != Vector3.zero)
		{
			move = move.normalized;
		}

		controller.Move(move * speed * Time.deltaTime);

	
	
			
	}
}

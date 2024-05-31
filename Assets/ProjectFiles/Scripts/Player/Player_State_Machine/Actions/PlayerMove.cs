using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove
{
	public static void Move(CharacterController controller, Vector2 moveDirection, float speed)
	{
		
		
		Vector3 move = controller.transform.right * moveDirection.x + controller.transform.forward * moveDirection.y;
		controller.Move(move * speed * Time.deltaTime);
		
	}
}

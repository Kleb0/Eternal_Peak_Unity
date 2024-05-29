using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove
{
	public static void Move(CharacterController controller, float speed)
	{
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		Vector3 move = controller.transform.right * x + controller.transform.forward * z;

		controller.Move(move * speed * Time.deltaTime);
		
	}
}

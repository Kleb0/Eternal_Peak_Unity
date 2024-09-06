using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingAgainstWall : PlayerMove
{
	// Start is called before the first frame update
	public static new void Move (CharacterController controller, Vector2 moveDirection, float Runspeed, Vector2 forwardBackward, Vector2 rightLeft)
	{
		PlayerMove.Move(controller, moveDirection, Runspeed, forwardBackward, rightLeft);
	} 
}

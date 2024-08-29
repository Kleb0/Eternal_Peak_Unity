using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_AgainstWall : PlayerState
{
	protected CharacterController Controller;
	protected Vector2 moveDirection;
	protected Vector2 forwardBackward;
	protected Vector2 rightLeft;

	protected Vector2 combinedMovement;
	protected float speed;
	

	
	public PlayerState_AgainstWall(CharacterController Controller, Vector2 moveDirection,  float speed,  Vector2 combinedMovement, Vector2 forwardBackward, Vector2 rightLeft)
	{
		this.Controller = Controller;
		this.moveDirection = moveDirection;
		this.speed = speed;
		this.combinedMovement = combinedMovement;
		this.forwardBackward = forwardBackward;
		this.rightLeft = rightLeft;
		stateName = "Against Wall";
		// As we are using here a special constructor, we just set the stateName to "AgainstWall" 
		//here without the need to override it in the derived classes.
	}

    public override void EnterState()
    {
		Debug.Log("PlayerState_AgainstWall -> EnterState");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetDirection : MonoBehaviour
{
    private Vector2 moveDirection;
    private Vector2 forwardDirection;
    private Vector2 rightDirection;

    public PlayerController playerController;

    public void SetMoveDirection(Vector2 direction)
    {

        moveDirection = direction;
    }

    public void SetForwardDirection(Vector2 fDirection)
    {
      //  Debug.Log("Forward Direction: " + fDirection);
        forwardDirection = fDirection;
    }
    public void SetRightDirection(Vector2 rDirection)
    {
     //   Debug.Log("Right Direction: " + rDirection);
        rightDirection = rDirection;
    }

    public void UpdateCurrentMovingDirection(Vector2 forward, Vector2 right)
    {
      //  Debug.Log("Update Direction");
        if (playerController != null && playerController.currentPlayerState is PlayerState_Moving movingState)
        {
            movingState.UpdateDirections(forward, right);
        }

    }

    public Vector2 GetMoveDirection()
    {
        
        return moveDirection;
    }
    public Vector2 GetForwardDirection()
    {
        
        return forwardDirection;
    }
    public Vector2 GetRightDirection()
    {
        
        return rightDirection;
    }


}

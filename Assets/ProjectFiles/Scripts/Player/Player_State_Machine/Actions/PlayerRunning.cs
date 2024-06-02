using UnityEngine;

public class PlayerRunning : PlayerMove
{
    public static new void Move (CharacterController controller, Vector2 moveDirection, float Runspeed, Vector2 forwardBackward, Vector2 rightLeft)
    {
        PlayerMove.Move(controller, moveDirection, Runspeed, forwardBackward, rightLeft);
    } 
}
using UnityEngine;

public class PlayerRunning : PlayerMove
{
    public static new void Move (CharacterController controller, Vector2 moveDirection, float Runspeed)
    {
        PlayerMove.Move(controller, moveDirection, Runspeed);
    } 
}
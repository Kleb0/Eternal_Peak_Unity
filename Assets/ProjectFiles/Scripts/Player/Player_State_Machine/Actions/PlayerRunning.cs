using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerRunning : PlayerMove
{

    public static new void Move (CharacterController controller, float Runspeed)
    {
        PlayerMove.Move(controller, Runspeed);
    }
 
}

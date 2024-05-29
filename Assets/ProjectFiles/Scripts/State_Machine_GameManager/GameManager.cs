using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   private Game_StateMachine stateMachine;

   void Start()
   {
         stateMachine = new Game_StateMachine();
         stateMachine.ChangeState(new RunningState());
   }

    void Update()
    {
            stateMachine.Update();
    }
}

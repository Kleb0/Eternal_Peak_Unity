using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_StateMachine 
{
   private IGameState currentState;

   public void ChangeState(IGameState newState)
   {
        if (currentState!= null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();

   }

   public void Update()
   {
         if (currentState != null)
         {
              currentState.Execute();
         }
   }
}

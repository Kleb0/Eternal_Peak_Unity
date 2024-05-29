using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : IGameState
{
    public void Enter(){Debug.Log("Enter Running State");}
    public void Execute(){ /* Management of Running Interactions here */}
    public void Exit(){Debug.Log("Exit Running State");}
  
}

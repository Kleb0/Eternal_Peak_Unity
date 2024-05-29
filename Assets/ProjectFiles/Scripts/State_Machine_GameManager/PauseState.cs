using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : IGameState
{
    public void Enter(){Debug.Log("Enter Pause State");}
    public void Execute(){ /* Management of Pause Interactions here */}
    public void Exit(){Debug.Log("Exit Pause State");}
    
}

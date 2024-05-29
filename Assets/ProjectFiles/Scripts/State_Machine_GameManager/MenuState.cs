using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuState : IGameState
{
    public void Enter(){Debug.Log("Enter Menu State");}
    public void Execute(){ /* Management of Menu Interactions here */}
    public void Exit(){Debug.Log("Exit Menu State");}
    
}

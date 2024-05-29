using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    string stateName { get; }
    void EnterState();
    void ExecuteState();
    void ExitState();
 
}

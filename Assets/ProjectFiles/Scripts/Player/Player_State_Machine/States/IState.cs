using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    string stateName { get; }
    void EnterState();
    void ExecuteState();
    void ExitState();
 
}

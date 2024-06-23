using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class is the base class for all the states of the player, it implements the IState interface

public class State : IState
{

    // as this class implements the IState interface, it must implement the following properties and methods,
    // so the derived classes can override them or use them as they are without the need to specify their logic
    // so that the derived classes can have their own logic for themselves without the need to implement the logic of a State

    public virtual string stateName { get; protected set; } = "STATE_OBJECT";
    public virtual void EnterState() {  /* Debug.Log("Enter " + stateName + " State"); */ }
    public virtual void ExecuteState() {/* State Logic here */}
    public virtual void ExitState() { /* Debug.Log("Exit " + stateName + " State"); */ }
}

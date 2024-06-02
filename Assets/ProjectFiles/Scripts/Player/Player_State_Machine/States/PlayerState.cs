using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class is the base class for all the states of the player

public class PlayerState : IPlayerState
{
    public string stateName { get; set; } = "PlayerState";

    public virtual void EnterState() {  /* Debug.Log("Enter " + stateName + " State"); */ }
    public virtual void ExecuteState() {/* State Logic here */}
    public virtual void ExitState() { /* Debug.Log("Exit " + stateName + " State"); */ }
}

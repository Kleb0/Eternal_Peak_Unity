using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public interface IState
{

    // we use the get set to make sure that the properties are overriden in the derived classes
    // and to encapsulate the behaviour of the properties

    // virtual keyword is used to allow the derived classes to override the properties
    public virtual string stateName 
    {
        get { return stateName;}
        set { stateName = value; }
    }

    // the following methods are the methods, representing the logic of a state. We need to Enter the state, Execute the state and Exit the state
    // They will be used in state manager scripts to change the state of an object that have states such as the player, the enemy, the weapon, etc.

    void EnterState();
    void ExecuteState();
    void ExitState();
 

}

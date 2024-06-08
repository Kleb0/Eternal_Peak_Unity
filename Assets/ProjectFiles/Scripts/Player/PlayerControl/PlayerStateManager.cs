using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStateManager : MonoBehaviour
{
    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        SetState(new PlayerState_Idle());
        
    }

    // Update is called once per frame
    void Update()
    {
        ExecuteState();        
    }


    public void SetState(State newState)
    {
        if(currentState != null)
        {
            currentState.ExitState();
        }
        currentState = newState;     
        currentState.EnterState();
    }

    public void ExecuteState()
    {
        if(currentState != null)
        {
            currentState.ExecuteState();
        }
    }
}

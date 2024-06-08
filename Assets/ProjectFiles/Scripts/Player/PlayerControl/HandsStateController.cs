using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsStateController : MonoBehaviour
{
    UIDEBUG uiDebug;
    private LeftHandState currentLeftHandState;
    private RightHandState currentRightHandState;
    private LeftHandState previousLeftHandState;
    public void Start()
    {
        uiDebug = GetComponent<UIDEBUG>();
        if (uiDebug == null)
        {
            Debug.LogError("UI Debug is null");
        }
        else
        {
            Debug.Log("UI Debug is not null");
        }
    }
    // Start is called before the first frame update
    public void SetLeftHandState(LeftHandState newLeftHandState)
    {

        if (currentLeftHandState != null)
        {
            currentLeftHandState.ExitState();
        }
        currentLeftHandState = newLeftHandState;
        currentLeftHandState.EnterState();     
        Debug.Log(" Current Left Hand State TEST: " + currentLeftHandState.stateName);
        if (uiDebug != null)
        {
            uiDebug.UpdateLeftHandStateUI(currentLeftHandState.stateName);
        }
        
    }

    public void ChangeLeftHandState(LeftHandState newLeftHandState)
    {
        previousLeftHandState = currentLeftHandState;
        SetLeftHandState(newLeftHandState);
        Debug.Log("Left Hand State: " + newLeftHandState.stateName);
    }

    public void RevertLeftHandState()
    {
        SetLeftHandState(previousLeftHandState);
    }

    public LeftHandState GetCurrentLeftHandState()
    {
        return currentLeftHandState;
    }

    // ------- Right Hand State Management ------- //


    public void SetRightHandState(RightHandState newRightHandState)
    {
        if (currentRightHandState != null)
        {
            currentRightHandState.ExitState();
        }
        currentRightHandState = newRightHandState;
        currentRightHandState.EnterState();
        Debug.Log(" Current Right Hand State TEST: " + currentRightHandState.stateName);
        if (uiDebug != null)
        {
            uiDebug.UpdateRightHandStateUI(currentRightHandState.stateName);
        }
    }

    public RightHandState GetCurrentRightHandState()
    {
        return currentRightHandState;
    }

    // The principle is merely simple : We have a state machine for the hands of the player, 
    //and we can change the state of the hands of the player by calling the SetLeftHandState and SetRightHandState methods.$
    //Then we can get the current state of the hands of the player by calling the GetCurrentLeftHandState and GetCurrentRightHandState methods.
}

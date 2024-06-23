using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsStateController : MonoBehaviour
{
    UIDEBUG uiDebug;
    private LeftHandState currentLeftHandState;
    private RightHandState currentRightHandState;
    private LeftHandState previousLeftHandState;
    private RightHandState previousRightHandState;
    private LeftHandState_DoNothing leftHandStateDoNothing;
    private LeftHandState_ComingBack leftHandStateComingBack;    
    private LeftHandState_IsRisingUp leftHandStateIsRisingUp;    

    // We build a loop for the hand state management :
    // in the player controller script, the hand state is defined on do nothing at start
    // in the input connect script, when the player press the left mouse button, the hand state is set to is rising up
    // then, when the action is cancelled, the hand state is set to coming back
    // and when the hand state is coming back, the hand state is set to do nothing
    // so we have a loop : do nothing -> is rising up -> coming back -> do nothing
    // these states, will activate and deactivate the arm Ik weight, the arm Ik rotation weight, and the arm Ik target


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

    public void Update()
    {

        // here we execute the action of the current active state
        if (currentLeftHandState != null)
        {
            currentLeftHandState.ExecuteState();
        }
    }    
    public void SetLeftHandState(LeftHandState newLeftHandState)
    {
        
        
        if (currentLeftHandState != null)
        {
            currentLeftHandState.ExitState(); 
        }

        currentLeftHandState = newLeftHandState;
        currentLeftHandState.EnterState();

        if (uiDebug != null)
        {
            uiDebug.UpdateLeftHandStateUI(currentLeftHandState.stateName);
        }   
    }

    public void ChangeLeftHandState(LeftHandState newLeftHandState)
    {
        previousLeftHandState = currentLeftHandState;
        SetLeftHandState(newLeftHandState);        
    }

    public void RevertLeftHandState(LeftHandState currentLeftHandState)
    {           
            ChangeLeftHandState(new LeftHandState_ComingBack());
            StartCoroutine(WaitAndChangeLeftHandState(currentLeftHandState, 2f));  

    }

    public void EndLeftHandLoop()
    {
        ChangeLeftHandState(new LeftHandState_DoNothing());
    }


    public LeftHandState GetCurrentLeftHandState()
    {
        return currentLeftHandState;
    }
    public IEnumerator WaitAndChangeLeftHandState(LeftHandState newLeftHandState, float time)
    {
        yield return new WaitForSeconds(time);

        // when the time is elapsed, we debug log a message   

        EndLeftHandLoop();
       
        // SetLeftHandState(newLeftHandState);
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
        
        if (uiDebug != null)
        {
            uiDebug.UpdateRightHandStateUI(currentRightHandState.stateName);
        }
    }

    public RightHandState GetCurrentRightHandState()
    {
        return currentRightHandState;
    }

    public void ChangeRightHandState(RightHandState newRightHandState)
    {
        previousRightHandState = currentRightHandState;
        SetRightHandState(newRightHandState);       
    }

    public void RevertRightHandState()
    {
        SetRightHandState(previousRightHandState);
    }

    // The principle is merely simple : We have a state machine for the hands of the player, 
    //and we can change the state of the hands of the player by calling the SetLeftHandState and SetRightHandState methods.$
    //Then we can get the current state of the hands of the player by calling the GetCurrentLeftHandState and GetCurrentRightHandState methods.

    // coroutine 

}

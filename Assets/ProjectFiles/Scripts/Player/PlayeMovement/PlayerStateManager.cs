using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStateManager : MonoBehaviour
{
    [Header("Player State DEBUG UI ")] 
    public TextMeshProUGUI currentStateText;
    public string currentStateName;

    private PlayerState currentState;

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


    public void SetState(PlayerState newState)
    {
        if(currentState != null)
        {
            currentState.ExitState();
        }
        currentState = newState;
        currentStateText.text = currentState.stateName;
        currentStateName = currentState.stateName;
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

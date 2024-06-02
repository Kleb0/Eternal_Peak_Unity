using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputConnect : MonoBehaviour
{

    private PlayerControls controls;
    private Firstpersonmovement firstPersonMovement;
    private float forwardBackward;
    private float rightLeft;   
    private bool combinedInput;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.PlayerInputMap.ForwardBackward.performed += ctx => OnForwardMovement(ctx);
        controls.PlayerInputMap.ForwardBackward.canceled += ctx => OnForwardMovement(ctx);

        controls.PlayerInputMap.RightLeft.performed += ctx => OnSideMovement(ctx);
        controls.PlayerInputMap.RightLeft.canceled += ctx => OnSideMovement(ctx);       

        firstPersonMovement = GetComponent<Firstpersonmovement>();    
        
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void OnForwardMovement(InputAction.CallbackContext context)
    {
        forwardBackward = context.phase == InputActionPhase.Performed ? context.ReadValue<float>() : 0f;
        UpdateMovement();
        // forwardBackward = controls.PlayerInputMap.ForwardBackward.ReadValue<float>();
        // Vector2 moveDirection = new Vector2(0f, forwardBackward);       
        // UpdateMovement();
        // firtPersonMovement.SetForwardDirection(moveDirection);
    

    }

    private void OnSideMovement(InputAction.CallbackContext context)
    {
        rightLeft = context.phase == InputActionPhase.Performed ? context.ReadValue<float>() : 0f;
        UpdateMovement();
        // rightLeft = controls.PlayerInputMap.RightLeft.ReadValue<float>(); 
        // Vector2 moveDirection = new Vector2(rightLeft, 0f);         
        // UpdateMovement();
        // firtPersonMovement.SetRightDirection(moveDirection);
      

    }


    private void UpdateMovement()
    {
        Vector2 moveDirection = new Vector2(rightLeft, forwardBackward);
        firstPersonMovement.SetMoveDirection(moveDirection);
        firstPersonMovement.SetForwardDirection(new Vector2(0f, forwardBackward));
        firstPersonMovement.SetRightDirection(new Vector2(rightLeft, 0f));   
        firstPersonMovement.UpdateCurrentStateDirections(new Vector2(0f, forwardBackward), new Vector2(rightLeft, 0f));
        

    } 
 
  
}

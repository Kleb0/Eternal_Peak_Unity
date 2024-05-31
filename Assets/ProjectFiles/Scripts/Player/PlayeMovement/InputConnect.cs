using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputConnect : MonoBehaviour
{

    private PlayerControls controls;
    private Firtpersonmovement firtPersonMovement;
    private float forwardBackward;
    private float rightLeft;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.PlayerInputMap.ForwardBackward.performed += ctx => OnMove();
        controls.PlayerInputMap.ForwardBackward.canceled += ctx => OnMove();

        controls.PlayerInputMap.RightLeft.performed += ctx => OnMove();
        controls.PlayerInputMap.RightLeft.canceled += ctx => OnMove();

        firtPersonMovement = GetComponent<Firtpersonmovement>();
     
        
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }


    private void OnMove()
    {
        forwardBackward = controls.PlayerInputMap.ForwardBackward.ReadValue<float>();
        rightLeft = controls.PlayerInputMap.RightLeft.ReadValue<float>();

        Vector2 moveDirection = new Vector2(rightLeft, forwardBackward);
        firtPersonMovement.SetMoveDirection(moveDirection);
       
    }

    // Start is called before the first frame update
  
}

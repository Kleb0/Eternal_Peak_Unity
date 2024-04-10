using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsTrigger : MonoBehaviour
{
    public GameObject climbingHold;
    public bool isGrabbing = false;

    public Firtpersonmovement player;
 
    private void OnTriggerEnter (Collider other)
    {
        print("Collided with " + other.gameObject.name);

        if (player.isClimbing)
        {
              if (other.gameObject.name.Contains("Climbing"))
                {
                    climbingHold = other.gameObject;
                    print("Collided with " + other.gameObject.name);
                    isGrabbing = true;
                    player.rightHandIsGrabbing = true;
                    player.rightHandClimbingHold = climbingHold;
                }           
        }
    }
}

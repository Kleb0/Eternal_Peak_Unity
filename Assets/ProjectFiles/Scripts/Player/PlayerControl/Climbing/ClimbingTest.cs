using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingTest : MonoBehaviour
{

    public float climbSpeed = 0.1f;
    public GameObject player;
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        OnPressClimb();
        
    }

    private void OnPressClimb()
    {

        // When space is pressed, the player will climb call Debug log each frame
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
           Debug.Log("Climbing");
           player.transform.position += Vector3.up * climbSpeed;
                 
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            player.transform.position += Vector3.down * climbSpeed;         
            
        }      
    }
}

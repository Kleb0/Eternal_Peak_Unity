using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandCollider : MonoBehaviour
{
	// Start is called before the first frame update
	private GameObject player;
	PlayerController playerController;

	HandsStateController handsStateController;
	void Start()
	{
		player = GameObject.Find( "Player" );
		playerController = player.GetComponent<PlayerController>(); 
		handsStateController = player.GetComponent<HandsStateController>();
		
	}
	public void OnTriggerEnter( Collider other )
	{
		
		Debug.Log( "Right Hand Collided with " + other.name );

		//we get the child of the object that the right hand is colliding with
		playerController.rightHandHoldingGrip = other.gameObject.transform.GetChild(0).gameObject;
		Debug.Log("Right hand is holding " + playerController.rightHandHoldingGrip.name);
		handsStateController.ChangeRightHandStateToHoldingGrip();


	}
}

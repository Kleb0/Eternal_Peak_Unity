using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandCollider : MonoBehaviour
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
		Debug.Log( "Left Hand Collided with " + other.name );
		handsStateController.ChangeLeftHandStateToHoldingGrip();
	}

}
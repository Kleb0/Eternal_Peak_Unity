using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandRaycast : MonoBehaviour
{

	private GameObject player;
	PlayerController playerController;

	HandsStateController handsStateController;
	Vector3 rayCastDirection = Vector3.forward;
	float maxRayDistance = 0.08f;

	void Start()
	{
		rayCastDirection = -transform.parent.forward;

		player = GameObject.Find( "Player" );
		playerController = player.GetComponent<PlayerController>(); 
		handsStateController = player.GetComponent<HandsStateController>();
		
	}

	void Update()
	{
		TryRaycast();
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, rayCastDirection * 0.08f);
	}

	public void TryRaycast()
	{	
	
		rayCastDirection = -transform.parent.forward;
		Ray ray = new Ray(transform.position, rayCastDirection);
		RaycastHit hit;

		Debug.DrawRay(transform.position, rayCastDirection * maxRayDistance, Color.green);	

		if (Physics.Raycast(ray, out hit, maxRayDistance))
		{
			if (hit.collider.CompareTag("Climbing_Point"))
			{
				playerController.leftHandHoldingAGrip = true;
				playerController.leftHandHoldingGrip = hit.collider.gameObject;
				// Debug.Log("Right hand is holding " + playerController.leftHandHoldingGrip.name);
				handsStateController.ChangeLeftHandStateToHoldingGrip();
			}			
		}
	}
}

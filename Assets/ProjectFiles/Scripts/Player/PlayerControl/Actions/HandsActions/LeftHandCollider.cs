using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandCollider : MonoBehaviour
{
	// Start is called before the first frame update
	public void OnTriggerEnter( Collider other )
	{
		Debug.Log( "Left Hand Collided with " + other.name );
	}

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class RiseHand : MonoBehaviour
{
	public static float IncrementIKWeight(float currentWeight, float rate)
	{
		currentWeight += rate * Time.deltaTime;

		if (currentWeight > 1)
		{
			currentWeight = 1;
			return currentWeight;
		}
		return currentWeight;
	}
	// public static void EnableTarget(GameObject target)
	// {
	// 	target.SetActive(true);
	// }

	// public static void DisableTarget(GameObject target)
	// {
	// 	target.SetActive(false);
	// }   
}

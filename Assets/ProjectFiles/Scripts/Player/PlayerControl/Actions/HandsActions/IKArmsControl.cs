using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using Unity.Burst.Intrinsics;
using System;

public class IKArmsControl : MonoBehaviour
{
	public static void EnableIKTarget(GameObject IKtarget)
	{
		IKtarget.SetActive(true);
	}
	public static void DisableIKTarget(GameObject IKtarget)
	{
		IKtarget.SetActive(false);
	}
	public static void EnableIkArm(ArmIK armIK)
	{
		armIK.enabled = true;	
	}

	// we will use this func to change 
	public static void ChangeIKarmTarget(ArmIK armIK, GameObject newtarget, GameObject previousTarget)
	{
		armIK.solver.arm.target = newtarget.transform;
		Debug.Log("New target is " + newtarget.name);
		Debug.Log("Previous target is " + previousTarget.name);
		previousTarget.SetActive(false);
		
	}

	public static void ResetFPSTarget(ArmIK armIK, GameObject FPStarget)
	{
		FPStarget.SetActive(false);
		armIK.solver.arm.target = FPStarget.transform;
	}

	public static void DisableIkArm(ArmIK armIK)
	{
		armIK.enabled = false;
	}

	public static bool IncrementLeftIkWeight(ArmIK armIK, ref float leftIKWeight, ref float lefIKRotationWeight, float rate)
	{
		leftIKWeight += rate * Time.deltaTime;
		armIK.solver.IKPositionWeight = leftIKWeight;

		lefIKRotationWeight += rate * Time.deltaTime;
		armIK.solver.IKRotationWeight = lefIKRotationWeight;

		if (leftIKWeight > 1) leftIKWeight = 1;
		if (lefIKRotationWeight > 1) lefIKRotationWeight = 1;

		return leftIKWeight == 1 && lefIKRotationWeight == 1;
	}

	// we return a boolean to send a kind of signal to the state handController
	public static bool DecrementLeftIkWeight(ArmIK armIK, ref float leftIKWeight, ref float lefIKRotationWeight, float rate)
	{
		leftIKWeight -= rate * Time.deltaTime;
		armIK.solver.IKPositionWeight = leftIKWeight;

		lefIKRotationWeight -= rate * Time.deltaTime;
		armIK.solver.IKRotationWeight = lefIKRotationWeight;

		if (leftIKWeight < 0) leftIKWeight = 0;
		if (lefIKRotationWeight < 0) lefIKRotationWeight = 0;

		// the bool is true when the IKWeight is 0 and the IKRotationWeight is 0
		return leftIKWeight == 0 && lefIKRotationWeight == 0;
	}

	public static bool IncrementRightIkWeight(ArmIK armIK, ref float IKWeight, ref float IKRotationWeight, float rate)
	{
		IKWeight += rate * Time.deltaTime;
		armIK.solver.IKPositionWeight = IKWeight;

		IKRotationWeight += rate * Time.deltaTime;
		armIK.solver.IKRotationWeight = IKRotationWeight;

		if (IKWeight > 1) IKWeight = 1;
		if (IKRotationWeight > 1) IKRotationWeight = 1;

		return IKWeight == 1 && IKRotationWeight == 1;
	}

	public static bool DecrementRightIkWeight(ArmIK armIK, ref float IKWeight, ref float IKRotationWeight, float rate)
	{
		IKWeight -= rate * Time.deltaTime;
		armIK.solver.IKPositionWeight = IKWeight;

		IKRotationWeight -= rate * Time.deltaTime;
		armIK.solver.IKRotationWeight = IKRotationWeight;

		if (IKWeight < 0) IKWeight = 0;
		if (IKRotationWeight < 0) IKRotationWeight = 0;

		return IKWeight == 0 && IKRotationWeight == 0;
	}

	public static float CalcDistBetweenLeftHandAndLeftShoulder(ArmIK armIK)
	{

		if (armIK.solver.isLeft)
		{

			Vector3 lefthandPosition = armIK.solver.hand.transform.position;
			Vector3 leftShoulderPosition = armIK.solver.shoulder.transform.position;

			float distance = Vector3.Distance(lefthandPosition, leftShoulderPosition);

			float minDistance = 0.30f;
			float maxDistance = 0.45f;

			// we normalize the distance on a field of 0 to 1
			float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
			return normalizedDistance;		

		}
		
		return 0f; // if the arm is not left, we don't need to return a distance, so we return 0
	}
	public static float CalcDistBetweenRighttHandAndRightShoulder(ArmIK armIK)
	{

		if (!armIK.solver.isLeft)
		{
			
			Vector3 righthandPosition = armIK.solver.hand.transform.position;
			Vector3 rightShoulderPosition = armIK.solver.shoulder.transform.position;

			float distance = Vector3.Distance(righthandPosition, rightShoulderPosition);

			float minDistance = 0.30f;
			float maxDistance = 0.45f;

			// we normalize the distance on a field of 0 to 1
			float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
			return normalizedDistance;		
		}
		return 0f;
		
	} 
}


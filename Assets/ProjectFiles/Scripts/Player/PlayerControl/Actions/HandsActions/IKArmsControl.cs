using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using Unity.Burst.Intrinsics;

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
}


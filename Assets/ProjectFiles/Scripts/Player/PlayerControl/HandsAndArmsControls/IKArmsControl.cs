using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using System;
using UnityEngine.InputSystem;

public class IKArmsControl : MonoBehaviour
{
	static float testValue = 0f;
	static Vector3 leftHandPosition = Vector3.zero;

	private static float inertialValue = 0f; 

	public static void startPlayerControllerCoroutine(PlayerController playerController)
	{
		playerController.StartTestCoroutine();
	}

	public static void EnableIKTarget(PlayerController playerController, GameObject IKtarget, GameObject BendingTarget, ArmIK armIK)
	{
		IKtarget.SetActive(true);	
	}
	public static void DisableIKTarget(GameObject IKtarget, GameObject BendingTarget)
	{
		BendingTarget.SetActive(false);
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

	public static bool IncrementLeftIkWeight(PlayerController playerController, ArmIK armIK, ref float leftIKWeight, ref float lefIKRotationWeight, float rate)
	{
		leftIKWeight += rate * Time.deltaTime;
		armIK.solver.IKPositionWeight = leftIKWeight;

		lefIKRotationWeight += rate * Time.deltaTime;
		armIK.solver.IKRotationWeight = lefIKRotationWeight;

		if (leftIKWeight > 1) leftIKWeight = 1;
		if (lefIKRotationWeight > 1) lefIKRotationWeight = 1;

		leftHandPosition = playerController.leftPalm.transform.position;
		return leftIKWeight == 1 && lefIKRotationWeight == 1;
	}

	// we return a boolean to send a kind of signal to the state handController
	public static bool DecrementLeftIkWeight(PlayerController playerController, ArmIK armIK, ref float leftIKWeight, ref float lefIKRotationWeight, float rate)
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

	public static bool IncrementRightIkWeight(PlayerController playerController, ArmIK armIK, ref float IKWeight, ref float IKRotationWeight, float rate)
	{
		IKWeight += rate * Time.deltaTime;
		armIK.solver.IKPositionWeight = IKWeight;

		IKRotationWeight += rate * Time.deltaTime;
		armIK.solver.IKRotationWeight = IKRotationWeight;

		if (IKWeight > 1) IKWeight = 1;
		if (IKRotationWeight > 1) IKRotationWeight = 1;

		return IKWeight == 1 && IKRotationWeight == 1;
	}

	public static bool DecrementRightIkWeight(PlayerController playerController, ArmIK armIK, ref float IKWeight, ref float IKRotationWeight, float rate)
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
			float maxDistance = 0.60f;

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
			float maxDistance = 0.55f;

			// we normalize the distance on a field of 0 to 1
			float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
			return normalizedDistance;		
		}
		return 0f;
		
	}

	public static void CorrectIkHandRaycastDirection(ArmIK armIk, GameObject palm)
	{

		if (armIk.solver.isLeft)
		{
			Transform leftShoulder = armIk.solver.shoulder.transform;
			Transform leftHand = armIk.solver.hand.transform;

			Vector3 Direction = leftShoulder.position - leftHand.position;

			palm.transform.rotation = Quaternion.LookRotation(Direction);
			
		}
		else
		{
			Transform rightShoulder = armIk.solver.shoulder.transform;
			Transform rightHand = armIk.solver.hand.transform;

			Vector3 Direction = rightShoulder.position - rightHand.position;

			palm.transform.rotation = Quaternion.LookRotation(Direction);
		}
	} 

	public static void AlignIKTargetCorrectly(ArmIK armIk, GameObject IKtarget, Camera cam, float radius = 2.0f, float leftOffset = 0.3f)
	{
		if (armIk.solver.isLeft)
		{
			Vector3 newIkTargetPosition;

			if(cam.orthographic)
			{
				newIkTargetPosition = cam.transform.position + cam.transform.forward * radius;

			}
			else
			{
				Vector3 cameraForward = cam.transform.forward;
				cameraForward.Normalize();

				Vector3 cameraLeft = -cam.transform.right;
				cameraLeft.Normalize();

				newIkTargetPosition = cam.transform.position + cameraForward * radius + cameraLeft * leftOffset;

				Vector3 cameraToIkTarget = newIkTargetPosition - cam.transform.position ;
				if (cameraToIkTarget.magnitude > radius)
				{
					newIkTargetPosition = cam.transform.position + cameraToIkTarget.normalized * radius;
				}
				
				IKtarget.transform.position = newIkTargetPosition;

			}

			
		}
	}

	public static void AlignBendingIKTargetCorrectly(ArmIK armIK, GameObject bendingtarget, Vector3 leftHandPosition)
	{

		if (armIK.solver.isLeft)
		{
			bendingtarget.transform.position = leftHandPosition;
			//Debug.Log($"left hand position is : {leftHandPosition}  {bendingtarget.name} position: {bendingtarget.transform.position}");	
		}
	}

	public static void ChangeIkArmTargetToBendingTarget(ArmIK armIK, GameObject bendingtarget)
	{
		if (armIK.solver.isLeft)
		{
			armIK.solver.arm.target = bendingtarget.transform;
		}
	}

	public static void ControlLeftArmBendingOnMouseScroll(ArmIK armIK, float adjustmentValue, GameObject bendingtarget)
	{

		if (armIK.solver.isLeft)
		{
			
			testValue += adjustmentValue * Time.deltaTime;
			testValue = Mathf.Clamp(testValue, -1f, 1f);

			Vector3 movementDirection = adjustmentValue > 0 ? bendingtarget.transform.forward : -bendingtarget.transform.forward;

			Vector3 targetPosition = bendingtarget.transform.position + movementDirection * Mathf.Abs(testValue);
			float smoothTime = 0.001f;
			Vector3 currentVelocity = Vector3.zero;

			bendingtarget.transform.position = Vector3.SmoothDamp(bendingtarget.transform.position, targetPosition, ref currentVelocity, smoothTime);

			// Vector3 movement = movementDirection * Mathf.Abs(testValue);
			// bendingtarget.transform.position += movement;
		}
	}

	
	public static void GuideleftHandByMouse(PlayerController playercontroller, ArmIK leftarmIk, float TimeSinceStart, Vector2 mouseDirection, string DirectionName)
	{
		
		if(TimeSinceStart > 0)
		{
			// Debug.Log($"Left hand is guided by mouse since {TimeSinceStart} seconds, mouse direction is {mouseDirection}, direction name is {DirectionName}, target position is {targetTransform.position}");
			//Debug.Log($"Left hand is guided by mouse since {TimeSinceStart} seconds, mouse direction is {mouseDirection}, direction name is {DirectionName}");
			
			if(leftarmIk.solver.arm.target != null)
			{
				Transform targetTransform = leftarmIk.solver.arm.target;
				Vector3 movementDirection = Vector3.zero;
				float movementSpeed = 2f;

				switch(DirectionName)
				{
					case "North":
						movementDirection = targetTransform.up;
						break;
					case "South":
						movementDirection = -targetTransform.up;
						break;	
					case "East":
						movementDirection = -targetTransform.right;
						break;
					case "West":
						movementDirection = targetTransform.right;
						break;
					case "North-East":
						movementDirection = targetTransform.up + targetTransform.right;
						break;
					case "North-West":
						movementDirection = targetTransform.up - targetTransform.right;
						break;
					case "South-East":
						movementDirection = -targetTransform.up + targetTransform.right;
						break;
					case "South-West":
						movementDirection = -targetTransform.up - targetTransform.right;
						break;
				}
	


				Vector3 targetPosition = targetTransform.position + movementDirection * movementSpeed * Time.deltaTime;
				targetTransform.position = targetPosition;


		
				// Debug.Log($"Left hand is guided by mouse since {TimeSinceStart} seconds, mouse direction is {mouseDirection}, direction name is {DirectionName}, target position is {targetTransform.position}");


			
	
				// Vector3 movementDirection = new Vector3(mouseDirection.x, 0, mouseDirection.y);				
			}
		}

	}

	// private static Vector3 GetCardinalDirection(Vector2 mouseDirection)
	// {
	// 	Vector2[] directions = new Vector2[]
	// 	{
	// 		new Vector2(0, 1),   // North
	// 		new Vector2(0, -1),  // South
	// 		new Vector2(1, 0),   // East
	// 		new Vector2(-1, 0),  // West
	// 		new Vector2(1, 1),   // North-East
	// 		new Vector2(-1, 1),  // North-West
	// 		new Vector2(1, -1),  // South-East
	// 		new Vector2(-1, -1)  // South-West	
	// 	};

	// 	int closestIndex = 0;
	// 	float maxDot = -1;

	// 	for (int i = 0; i < directions.Length; i++)
	// 	{
	// 		float dot = Vector2.Dot(mouseDirection.normalized, directions[i]);
	// 		if (dot > maxDot)
	// 		{
	// 			maxDot = dot;
	// 			closestIndex = i;
	// 		}
	// 	}

	// 	Vector2 bestMatch = directions[closestIndex];
	// 	Debug.Log($"closest Direction: {bestMatch}");
	// 	return new Vector3(bestMatch.x, 0, bestMatch.y);

	// }
}
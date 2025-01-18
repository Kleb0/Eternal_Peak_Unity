using UnityEngine;
using RootMotion.FinalIK;
using System;

public class IKArmsControl : MonoBehaviour
{
	static float bendingValue = 0f;
	static Vector3 leftHandPosition = Vector3.zero;


	public static void EnableIKTarget(PlayerController playerController, GameObject IKtarget, ArmIK armIK)
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
			Vector3 newIkTargetPosition = Vector3.zero;
			AlignIk(newIkTargetPosition, cam, armIk);			
		}

		if(!armIk.solver.isLeft)
		{
			Vector3 newIkTargetPosition = Vector3.zero;
			AlignIk(newIkTargetPosition, cam, armIk);		
		}
	}


	public static void AlignIk(Vector3 newIkTargetPosition, Camera cam, ArmIK armik )
	{
		if (cam.orthographic)
		{
			newIkTargetPosition = cam.transform.position + cam.transform.forward * 2.0f;
		}
		else
		{
			Vector3 cameraForward = cam.transform.forward;
			cameraForward.Normalize();

			Vector3 cameraLeft = -cam.transform.right;
			cameraLeft.Normalize();

			newIkTargetPosition = cam.transform.position + cameraForward * 2.0f + cameraLeft * 0.3f;

			Vector3 cameraToIkTarget = newIkTargetPosition - cam.transform.position ;
			if (cameraToIkTarget.magnitude > 2.0f)
			{
				newIkTargetPosition = cam.transform.position + cameraToIkTarget.normalized * 2.0f;
			}
			armik.solver.arm.target.position = newIkTargetPosition;
			
		}
	}

	public static void ChangeIkArmTargetToBendingTarget(ArmIK armIK, GameObject bendingTarget, GameObject handTarget)
	{
				

		if (armIK.solver.isLeft)
		{
			
			Debug.Log("Change the left arm Ik target to the left bending target" + bendingTarget.name);
			armIK.solver.arm.target = bendingTarget.transform;

			Vector3 directionToTarget = bendingTarget.transform.position - handTarget.transform.position;

			if(directionToTarget != Vector3.zero)
			{
				bendingTarget.transform.rotation = Quaternion.LookRotation(directionToTarget);
			}
		}
		else
		{
			//Debug.Log("Change the right arm Ik target to the right bending target" + bendingTarget.name);
			armIK.solver.arm.target = bendingTarget.transform;

			Vector3 directionToTarget = bendingTarget.transform.position - handTarget.transform.position;

			if(directionToTarget != Vector3.zero)
			{
				bendingTarget.transform.rotation = Quaternion.LookRotation(directionToTarget);
			}
		}
	}

	public static void ControlArmBendingOnMouseScroll(ArmIK armIK, float adjustmentValue, GameObject bendingtarget, Vector3 currentHandPosition)
	{

		if (armIK.solver.isLeft)
		{
			Debug.Log("Left arm is controlled by mouse scroll");
			guideOnScroll(adjustmentValue, bendingtarget, currentHandPosition);
		}

		else if(!armIK.solver.isLeft)
		{
			Debug.Log("Right arm is controlled by mouse scroll");
			guideOnScroll(adjustmentValue, bendingtarget, currentHandPosition);
	
		}
	}

	
	public static void GuideHandByMouse(PlayerController playercontroller, ArmIK armIk, float TimeSinceStart, Vector2 mouseDirection, string DirectionName)
	{
		
		if(TimeSinceStart > 0)
		{
			// Debug.Log($"Left hand is guided by mouse since {TimeSinceStart} seconds, mouse direction is {mouseDirection}, direction name is {DirectionName}, target position is {targetTransform.position}");
			//Debug.Log($"Left hand is guided by mouse since {TimeSinceStart} seconds, mouse direction is {mouseDirection}, direction name is {DirectionName}");
			
			if (mouseDirection == Vector2.zero || string.IsNullOrEmpty(DirectionName) || DirectionName == "None")
			{		
				return;
			}

			if(armIk.solver.isLeft && armIk.solver.arm.target != null)
			{
				// Debug.Log("Left arm is guided by mouse");
				GuideOnMouse(playercontroller, armIk, DirectionName, 3f);
				
			}
			if(!armIk.solver.isLeft)
			{
				// Debug.Log("Right arm is guided by mouse");
				GuideOnMouse(playercontroller, armIk, DirectionName, 3f);
			}
		}
	}


#region mouse guide functions

	public static void guideOnScroll(float adjustmentValue, GameObject bendingtarget, Vector3 currentHandPosition)
	{
			bendingValue = 0f;
			Debug.Log("Left arm is controlled by mouse scroll");
			bendingValue += adjustmentValue * Time.deltaTime;
			Debug.Log("bending value is " + bendingValue);
			bendingValue = Mathf.Clamp(bendingValue, -1f, 1f);

			//movement direction based on the hand position
			Vector3 movementDirection = adjustmentValue > 0 ? bendingtarget.transform.forward : -bendingtarget.transform.forward;

			//calculate the new position based on the actual pos of the hand
			Vector3 targetPosition = currentHandPosition + movementDirection * Mathf.Abs(bendingValue);

			float smoothTime = 0.001f;
			Vector3 currentVelocity = Vector3.zero;

			bendingtarget.transform.position = Vector3.SmoothDamp(bendingtarget.transform.position, targetPosition, ref currentVelocity, smoothTime);

	}

	private static void GuideOnMouse(PlayerController playerController, ArmIK armIK, string directionName, float movementSpeed)
	{
		// if (!armIK.solver.isLeft)
		// {
		// 	Debug.Log("Right arm is guided by mouse");
		// }
		if (armIK.solver.arm.target == null || string.IsNullOrEmpty(directionName) || directionName == "None")
			return;

		Transform targetTransform = armIK.solver.arm.target;
		Vector3 movementDirection = Vector3.zero;
		Camera camera = playerController.cam;
		Vector3 screenBounds = new Vector3(Screen.width, Screen.height, 0f);

		switch (directionName)
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
		Vector3 interpolatedPosition = Vector3.Lerp(targetTransform.position, targetPosition, 0.5f);

		Vector3 screenPosition = camera.WorldToScreenPoint(interpolatedPosition);
		screenPosition.x = Mathf.Clamp(screenPosition.x, 0, screenBounds.x);
		screenPosition.y = Mathf.Clamp(screenPosition.y, 0, screenBounds.y);

		Vector3 clampedWorldPosition = camera.ScreenToWorldPoint(screenPosition);
		targetTransform.position = clampedWorldPosition;
	}

	

#endregion
}
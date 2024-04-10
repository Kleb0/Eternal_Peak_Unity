using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]

public class Climber : MonoBehaviour
{
	
	private CustomControls customControls;

	[Header ("===== Player GameObject =====")]
	[Space (10)]

	public GameObject player;

	[Header ("===== Lef Hand =====")]
	[Space (10)]

	public Rigidbody leftHandRigidbody;
	public GameObject leftHand;
	public GameObject lefArmRig;
	public GameObject leftHandInitialPosition;

	[Header ("===== Right Hand =====")]	
	[Space (10)]

	public Rigidbody rightHandRigidbody;
	public GameObject rightHand;
	public GameObject rightArmRig;
	public GameObject rightHandInitialPosition;

	[Header ("===== Body =====")]
	[Space (10)]

	public Rigidbody bodyRigidbody;

	[Header ("===== Controlled Hand =====")]
	[Space (10)]

	public Rigidbody controlledHandRigidbody;
	private bool isLeftHandControlled = true;
	private bool isConctrolledHandGrabbing = false;
	private bool canMovecontrolledHand = true;

	private bool isRisingHand = false;

	private float forceMagnitude = 5f;
	private float maxDistance = 3f;

	[Header ("===== Body Constraints =====")]
	[Space (10)]
	public float velocityDamping = 0.95f;
	public float maxYDifference = 1f;

	[Header ("===== Forces =====")]
	[Space (10)]
	public float handUpdwardForce = 2f;
	private float maxHandHeight;

	public float limiteYfromBody = 2f;
	public float adjustReichValue = 1f;
	private Vector3 initialHandPosition;

	[Header ("===== Climbing Holds =====")]
	[Space (10)]
	public GameObject rightHoldedClimb;
	public GameObject leftHoldedClimb;
	public bool areTwoHandsHolding = false;

	[Header ("===== Hands Rotation Limits =====")]
	[Space (10)]
	public float maxAngle = 60f;
	public float minAngle = -60f;
	public float circleRadius = 0.2f;

	[Header ("=====  mouse sensitivity =====")]
	[Space (10)]

	public float mouseSensitivity = 1.0f;

	[Header ("===== Hand Movement =====")]
	[Space (10)]
	public float minXLimit = -2f;
	public float maxXLimit = 2f;
	private float minLimitFromControlledHand;
	private float maxLimitFromControlledHand;
		
	public float handSpeed = 10.0f;

	private bool hasBeenInitialized = false;


	void Awake()
	{
		customControls = new CustomControls();
		
	}

	void OnEnable()
	{
		customControls.Enable();
		customControls.PlayerMap.RiseHand.performed += ctx => onInputPerformed(ctx);
		customControls.PlayerMap.RiseHand.canceled += ctx => onInputCanceled(ctx);
	}

	void OnDisable()
	{
		customControls.Disable();
		customControls.PlayerMap.RiseHand.performed -= ctx => onInputPerformed(ctx);
		customControls.PlayerMap.RiseHand.canceled -= ctx => onInputCanceled(ctx);
		
	}

	void onInputPerformed(InputAction.CallbackContext ctx)
	{
		Debug.Log("Input Performed");
		Debug.Log("maxHandHeight: " + maxHandHeight);
		isRisingHand = true;		
	}
	void onInputCanceled(InputAction.CallbackContext ctx)
	{
		isRisingHand = false;
	}

	void Start()
	{	
		PlaceHandOnStart();
		SetMovementLimits();
		//Lock Cursor
		Cursor.lockState = CursorLockMode.Locked;

		//freeze the position y of controlled hand

		controlledHandRigidbody = leftHandRigidbody;
		controlledHandRigidbody.constraints = RigidbodyConstraints.FreezePositionY;
		calculate_maxHandHeight();
		hasBeenInitialized = true;	
			
	}

	// Update is called once per frame
	void Update()
	{
		LimitHandMovement();
		ApplyForceOnControlledHand();
		ConstraintBody();
		pullBody();
		HandleMouseClick();
	//	UpdateArmPos();
	}
	
	// Manage the input for the Left Hand and apply the forces
	
	// Constraint the body to the left hand

	void PlaceHandOnStart()
	{
		leftHandRigidbody.position = leftHandInitialPosition.transform.position;
		rightHandRigidbody.position = rightHandInitialPosition.transform.position;
		Debug.Log("place both hands on start");

	}
	void SetMovementLimits()
	{
		minLimitFromControlledHand = controlledHandRigidbody.transform.localPosition.x + minXLimit;
		maxLimitFromControlledHand = controlledHandRigidbody.transform.localPosition.x + maxXLimit;
	}

	void calculate_maxHandHeight()
	{
		maxHandHeight = bodyRigidbody.transform.localPosition.y + limiteYfromBody;
	}
	void ConstraintBody()
	{
		if (hasBeenInitialized)
		{
			float midPointX = (leftHandRigidbody.position.x + rightHandRigidbody.position.x) / 2;

			Vector3 targetPosition = new Vector3(midPointX, bodyRigidbody.position.y, bodyRigidbody.position.z);

			float smoothSpeed = 5f;
			Vector3 smoothedPosition = Vector3.Lerp(bodyRigidbody.position, targetPosition, smoothSpeed * Time.deltaTime);
			bodyRigidbody.MovePosition(smoothedPosition);

		}
		
	}

	void pullBody()
	{

		float midPointY = (leftHandRigidbody.position.y + rightHandRigidbody.position.y) / 2;
		float distanceToMidPointY = midPointY - bodyRigidbody.position.y;
		Vector3 forceDirectionY = new Vector3(0, distanceToMidPointY, 0).normalized;

		float forceMuliplierY = Mathf.Clamp01(Mathf.Abs(distanceToMidPointY) / maxDistance);
		Vector3 pullForceY = forceDirectionY * forceMagnitude * forceMuliplierY;
		bodyRigidbody.AddForce(pullForceY, ForceMode.Force);

		Vector3 dampingForce = -bodyRigidbody.velocity * velocityDamping;
		bodyRigidbody.AddForce(dampingForce, ForceMode.Force);
	}

	void HandleMouseClick()
	{
		if (Input.GetMouseButtonDown(0))
			{		
				Debug.Log("Mouse Clicked");
				controlledHandRigidbody.isKinematic = true;
				isRisingHand = false;
				canMovecontrolledHand = false;
				maxHandHeight += controlledHandRigidbody.transform.localPosition.y;
				calculate_maxHandHeight();
				ToggleHandSwitch();				
			}
	}

	// public void OnClimberGrabbing()
	// {
	// 	if (rightHandScript.isGrabbing)
	// 	{
	// 		Debug.Log(" Right Hand call On Climber Grabbing");
	// 		rightHandRigidbody.isKinematic = true;
	// 		rightHandRigidbody.position = rightHandScript.holdingClimb.transform.position + new Vector3(0, 0, -0.5f);			
	// 	}
	// 	if (leftHandScript.isGrabbing)
	// 	{
	// 		Debug.Log(" Left Hand call On Climber Grabbing");
	// 		leftHandRigidbody.isKinematic = true;
	// 		leftHandRigidbody.position = leftHandScript.holdingClimb.transform.position + new Vector3(0, 0, -0.5f);
	// 		canMovecontrolledHand = false;
	// 	}
	// }

	void ToggleHandSwitch()
	{
		isLeftHandControlled = !isLeftHandControlled;

		if (isLeftHandControlled)
		{
			controlledHandRigidbody = leftHandRigidbody;
			rightHandRigidbody.isKinematic = true;
			leftHandRigidbody.isKinematic = false;
		
		}
		else
		{
			controlledHandRigidbody = rightHandRigidbody;
			leftHandRigidbody.isKinematic = true;
			rightHandRigidbody.isKinematic = false;

		}

		controlledHandRigidbody.isKinematic = false;
		canMovecontrolledHand = true;
		UpdateMovementLimits();
	}

	void ApplyForceOnControlledHand()
	{

		if(canMovecontrolledHand)
		{
			if (isRisingHand)
			{
				initialHandPosition.y = bodyRigidbody.transform.localPosition.y - 1f;
				controlledHandRigidbody.isKinematic = false;
				controlledHandRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;

				if(controlledHandRigidbody.transform.localPosition.y < maxHandHeight)
				{
				
					controlledHandRigidbody.AddForce(Vector3.up * handUpdwardForce, ForceMode.Force);

				}
				else
				{
					Vector3 antiGravityForce = -Physics.gravity * controlledHandRigidbody.mass;
					controlledHandRigidbody.AddForce(antiGravityForce, ForceMode.Force);

					controlledHandRigidbody.transform.localPosition =
					new Vector3(controlledHandRigidbody.transform.localPosition.x,
					maxHandHeight,
					controlledHandRigidbody.transform.localPosition.z);
					
				}				
			}	
			else 
			{
				
				controlledHandRigidbody.velocity = new Vector3(controlledHandRigidbody.velocity.x, +Physics.gravity.y * controlledHandRigidbody.mass
				, controlledHandRigidbody.velocity.z);

				if (controlledHandRigidbody.transform.localPosition.y < initialHandPosition.y)
				{
					float dampingFactor = 0.95f;
					Vector3 dampingForce = -controlledHandRigidbody.velocity * dampingFactor;
					controlledHandRigidbody.AddForce(dampingForce, ForceMode.Force);

					if(controlledHandRigidbody.velocity.y < 0.05f)
					{
						controlledHandRigidbody.constraints = RigidbodyConstraints.FreezePositionY;
					}

				}
				
			}		
			
		}
		
	
	}

	void LimitHandMovement()
	{
		if(hasBeenInitialized)
		{
			if(canMovecontrolledHand)
			{
				float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

				Vector3 newPosition = controlledHandRigidbody.transform.localPosition + new Vector3(mouseX, 0, 0) * handSpeed;
				newPosition.x = Mathf.Clamp(newPosition.x, minLimitFromControlledHand, maxLimitFromControlledHand);

				controlledHandRigidbody.transform.localPosition = newPosition;
				
			}		
		}
	}

	void UpdateMovementLimits()
	{
		float handPositionX = controlledHandRigidbody.transform.localPosition.x;
		minLimitFromControlledHand = handPositionX + minXLimit;
		maxLimitFromControlledHand = handPositionX + maxXLimit;
	}

	void UpdateArmPos()
	{
		// lefArmRig.transform.position = leftHandRigidbody.transform.position;
		// rightArmRig.transform.position = rightHandRigidbody.transform.position;

	}

	void SetupClimber()
	{
		
	}
	
}

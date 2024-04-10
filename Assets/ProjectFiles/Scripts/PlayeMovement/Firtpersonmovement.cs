using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class Firtpersonmovement : MonoBehaviour
{
	
	private CharacterController controller;
	private Camera cam;
	private Collider coll;
	private Animator anim;
	private float speed = 2f;
	private float sprintSpeed = 5f;
	private float mouseSensitivity = 300f;
	private float jumpForce = 5f;

	public bool isClimbing = false;

	private Vector3 currentVelocity;

	//gravity is equal to one defined in project settings
	private float gravity = Physics.gravity.y;
	private float xRotation;

	private bool isInterpolating = false;
	private bool invertInterpolation = false;

	[Header("==== Left Arm ====")]
	[Space(10)]

	public GameObject leftArm;
	public GameObject leftArmPos;
	public GameObject leftHandPos;
	public GameObject IKLeftHand;

	[Header("==== Right Arm ====")]
	[Space(10)]
	public GameObject rightArm;
	public GameObject IkRightOriginalPosition;
	public GameObject IkRightTarget;
	public GameObject rightHandCtrllr;

	private Quaternion originalRightHandRotation;
	public GameObject rightHandDetector;
	public GameObject rightHandClimbingHold;

	public bool rightHandIsGrabbing = false;

	[Header("==== Right Arm Rig Layer ====")]
	[Space(10)]
	public Rig rightArmRigLayer;

	private Vector3 originalIkRightTargetPos;

	[Header("==== Cameras positions and aiming ====")]	

	public GameObject aimingPos;
	public GameObject newCamPos;
	public GameObject originalCamPos;

	[Header("Player Manager")]
	[Space(10)]
	public GameObject playerManager;

	private float verticalVelocity;
	private float xRotationSmooth = 0f;
	private float rotationSmoothTime = 0.01f;
	private bool returnToOriginalPos = false;

	// 
	// Start is called before the first frame update
	void Start()
	{
	
		originalRightHandRotation = rightHandCtrllr.transform.localRotation;
		controller = GetComponent<CharacterController>();
		cam = GetComponentInChildren<Camera>();
		Cursor.lockState = CursorLockMode.Locked;
		rightArmRigLayer.weight = 0f;
		
	}

	// Update is called once per frame
	void Update()
	{
		Move();
		Look();
		UpdateCamPosition();				
		ToggleClimb();		
	}

	void applyGravity()
	{
		if (controller.isGrounded)
		{
			verticalVelocity = 0f;
		}
		else
		{
			verticalVelocity += gravity * Time.deltaTime;
		}
		Vector3 gravityMove = new Vector3(0f, verticalVelocity, 0f);
		controller.Move(gravityMove * Time.deltaTime);

	}

	void applyJump()
	{
		if (controller.isGrounded)
		{
			Debug.Log("Jump");
			verticalVelocity = jumpForce;
		}		
	}

	void Look()
	{
	
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
		
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		xRotationSmooth = Mathf.Lerp(xRotationSmooth, xRotation, rotationSmoothTime / Time.deltaTime);

		transform.Rotate(Vector3.up * mouseX);

		cam.transform.localRotation = Quaternion.Euler(xRotationSmooth, 0f, 0f);	
		leftArm.transform.localRotation = Quaternion.Euler(xRotationSmooth, 0f, 0f);

		if(rightHandIsGrabbing == false)
		{
			rightArm.transform.localRotation = Quaternion.Euler(xRotationSmooth, 0f, 0f);
		}

		if (isInterpolating == false)
		{
			IkRightTarget.transform.localPosition = IkRightOriginalPosition.transform.localPosition;
		
		}


	}

	void ToggleClimb()
	{		
		// when the right mouse button is pressed, the ik controller will move the right hand
		// the rig layer will be set to 1 with interpolation

		if (Input.GetMouseButton(1))
		{	
			rightArmRigLayer.weight = 1;
			returnToOriginalPos = false;
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			Vector3 rayPoint = ray.GetPoint(1f);
			Vector3 newPos = rayPoint;
			isClimbing = true;

			rightHandCtrllr.transform.localRotation = Quaternion.Euler(70f, 60f, 65f);

			if (rightHandIsGrabbing)
			{
				IkRightTarget.transform.position = rightHandClimbingHold.transform.position + new Vector3(0f, 0f, 1f);
						
			}
			else
			{
				IkRightTarget.transform.position = newPos;			
			}
		}

		// the button is released, so the right hand will return to the original position

		if (Input.GetMouseButtonUp(1))
		{
			isClimbing = false;			
		}

		// The right hand is not grabbing anymore, so the ik controller will return to the original position and
		// the rig layer will be set to 0 with interpolation

		if (isClimbing == false)
		{
			rightArmRigLayer.weight = 0;
			isInterpolating = false;
			invertInterpolation = false;	
			rightHandIsGrabbing = false;
			rightHandClimbingHold = null;
			IkRightTarget.transform.localPosition = originalIkRightTargetPos;
			rightHandCtrllr.transform.localRotation = originalRightHandRotation;			
		}	
	}
	
	void Move()
	{

		//if we're not climbing, we can move the player
		if (rightHandIsGrabbing == false)
		{
			
			float x = Input.GetAxis("Horizontal");
			float z = Input.GetAxis("Vertical");

			Vector3 move = transform.right * x + transform.forward * z;	
			
			float currentSpeed = move.magnitude;	

			if (Input.GetKey(KeyCode.LeftShift) && currentSpeed > 0f)
			{
				currentSpeed = sprintSpeed;
			}
			else
			{
				currentSpeed *= speed;
			}	
			
			controller.Move(move.normalized * currentSpeed * Time.deltaTime);
			
		}
		else
		{
			// apply the climb logic here
		}
			
	}

	void UpdateCamPosition()
	{
		Vector3 newPos = aimingPos.transform.position;
		newPos = (leftHandPos.transform.position + IkRightOriginalPosition.transform.position) / 2f;

		aimingPos.transform.position = newPos;
		aimingPos.transform.localEulerAngles = cam.transform.localEulerAngles;

		Vector3 targetPos;

		if(cam.transform.localRotation.x < 0.2f)
		{
			targetPos = newCamPos.transform.position;	
		}
		else
		{
			targetPos = originalCamPos.transform.position;
		}
		
		cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, Time.deltaTime * 10f);		
	}
}

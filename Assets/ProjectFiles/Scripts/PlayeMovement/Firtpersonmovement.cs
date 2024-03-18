using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Firtpersonmovement : MonoBehaviour
{

	private CharacterController controller;
	private Camera cam;
	private Collider coll;
	private PlayerControls playerControls;
	private Animator anim;

	private float speed = 2f;
	private float sprintSpeed = 5f;
	private float mouseSensitivity = 300f;
	private float jumpForce = 5f;

	//gravity is equal to one defined in project settings
	private float gravity = Physics.gravity.y;
	private float xRotation;

	

	private float verticalVelocity;

	// 
	// Start is called before the first frame update
	void Start()
	{
		playerControls = new PlayerControls();
		playerControls.Enable();

		coll = GetComponent<Collider>();
		controller = GetComponent<CharacterController>();
		cam = GetComponentInChildren<Camera>();
		anim = GetComponent<Animator>();
		Cursor.lockState = CursorLockMode.Locked;
		
	}

	// Update is called once per frame
	void Update()
	{
		applyGravity();
		applyJump();		
		Look();
		Move();
	}

	void applyGravity()
	{
		if (controller.isGrounded)
		{
			verticalVelocity = 0;
		}
		else
		{
			verticalVelocity += gravity * Time.deltaTime;
		}
		Vector3 gravityMove = new Vector3(0, verticalVelocity, 0);
		controller.Move(gravityMove * Time.deltaTime);
	}

	void applyJump()
	{
		if (controller.isGrounded && playerControls.PlayerMap.Jump.triggered)
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

		cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
		transform.Rotate(Vector3.up * mouseX);
	}

	void Move()
	{

		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		Vector3 move = transform.right * x + transform.forward * z;
		float currentSpeed = move.magnitude;

		if (Input.GetKey(KeyCode.LeftShift) && currentSpeed > 0)
		{
			currentSpeed = sprintSpeed;
		}
		else
		{
			currentSpeed *= speed;
		}

		controller.Move(move.normalized * currentSpeed * Time.deltaTime);
		anim.SetFloat("speed", currentSpeed);
	
	}
}

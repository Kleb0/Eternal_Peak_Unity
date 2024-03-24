using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Climber : MonoBehaviour
{

    public Rigidbody leftHandRigidbody;
    public Rigidbody bodyRigidbody;
    public Transform bodyPivot;
    private float forceMagnitude = 10f;
    public float maxDistance = 2f;
    public float minDistance = 2f;
    public float velocityDamping = 0.95f;

    public float maxYDifference = 1f;

    private Quaternion targetRotation;
    private bool needsNewTarget = true;

    // Update is called once per frame
    void Update()
    {

        inputLeftHandMovement();
        ConstraintBody();
        ApplyRotation();  
        
    }

    // Manage the input for the Left Hand and apply the forces
    void inputLeftHandMovement()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("A pressed");
            Vector3 force = new Vector3(-forceMagnitude, 0, 0);
            leftHandRigidbody.AddForce(force, ForceMode.Force);

            PullBody();
            
        }
        else
        {
            leftHandRigidbody.velocity = Vector3.zero;
            StopPulling();
            

        }
    }

    // Pull the body towards the left hand
    void PullBody()
    {
        Vector3 directionToPull = leftHandRigidbody.position - bodyRigidbody.position;
        directionToPull.y = 0;
        float currentDistance = Vector3.Distance(leftHandRigidbody.position, bodyRigidbody.position);

        if (currentDistance < minDistance || currentDistance > maxDistance)
        {

            bodyRigidbody.AddForce(directionToPull.normalized * forceMagnitude, ForceMode.Force);
            
        }
    }

    // Stop pulling the body
    void StopPulling()
    {
        leftHandRigidbody.velocity *= velocityDamping;
        bodyRigidbody.velocity *= velocityDamping;

        if (leftHandRigidbody.velocity.magnitude < 0.01f)
        {
            leftHandRigidbody.velocity = Vector3.zero;
        }
        if (bodyRigidbody.velocity.magnitude < 0.01f)
        {
            bodyRigidbody.velocity = Vector3.zero;
        }
    }
    // Constraint the body to the left hand
    void ConstraintBody()
    {
        float yDifference = leftHandRigidbody.position.y - bodyRigidbody.position.y;

        if (yDifference > maxYDifference)
        {

            float verticalVelocity = bodyRigidbody.velocity.y;

            if (verticalVelocity < 0)
            {
                Vector3 upwardforce = new Vector3(0, forceMagnitude, 0);
                bodyRigidbody.AddForce(upwardforce, ForceMode.Force);

            }

            else
            {
                float reducedForced = forceMagnitude * (1 - Mathf.Clamp(verticalVelocity / 10, 0, 1));
                Vector3 upwardForce = new Vector3(0, reducedForced, 0);
                bodyRigidbody.AddForce(upwardForce, ForceMode.Force);
            }
            
        }

    }

    //make a pendulum effect
    void ApplyRotation()
    {

        float maxRotationAngle = 30f;
        float period = 3f;
        float speedRotation = 0.25f;

        float xAngle = maxRotationAngle * Mathf.Sin(Time.time * speedRotation / period * Mathf.PI *2);
        float yAngle = maxRotationAngle * Mathf.Sin((Time.time * speedRotation / period * Mathf.PI *2) + (2 * Mathf.PI / 3));
        float zAngle = maxRotationAngle * Mathf.Sin((Time.time * speedRotation / period * Mathf.PI *2) + (4 * Mathf.PI / 3));
        
        Quaternion pendulumRotation = Quaternion.Euler(xAngle, yAngle, zAngle);

        bodyPivot.rotation = Quaternion.Lerp(bodyPivot.rotation, pendulumRotation, Time.deltaTime * speedRotation);

    }
}

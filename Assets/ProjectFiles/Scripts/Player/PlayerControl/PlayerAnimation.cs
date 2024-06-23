using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    public Animator animator;
	public Avatar avatar;

	public GameObject LeftArmIkTarget;

    void Start()
	{


	}

    public void SetWalking(bool walking)
    {
        animator.SetBool("isWalking", walking);
    }

    public void SetRunning(float running)
    {
        animator.SetFloat("Running", running);
    }

    public float RunWalkBlending(float currentVelocity, float rate, float target)
	{
		
		currentVelocity += rate * Time.deltaTime;

		// if the rate is positive, we return the minimum value between the current velocity and the target
		// if the rate is negative, we return the maximum value between the current velocity and the target

		if (rate> 0)
		{
			return Mathf.Min(currentVelocity, target);
		}
		else
		{
			return Mathf.Max(currentVelocity, target);
		}
	}

}

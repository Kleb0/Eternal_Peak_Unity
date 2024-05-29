using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    public Animator animator;
    // Start is called before the first frame update

    public void SetWalking(bool walking)
    {
        animator.SetBool("isWalking", walking);
    }

    public void SetRunning(float running)
    {
        animator.SetFloat("Running", running);
    }


}

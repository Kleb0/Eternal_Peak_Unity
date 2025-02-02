using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerProcessJumping : MonoBehaviour
{
    public static void ProcessJumping(
        PlayerController playerController,
        CharacterController controller,
        HandsStateController handsStateController,
        Vector2 inputDirection,
        float jumpSpeed,
        float currentSpeed
    )
    {
        //Debug.Log($"Process Jumping current speed is {currentSpeed}");
        float gravity = -9.81f;
        
        playerController.jumpEllapsedTime += Time.deltaTime;
        playerController.verticalVelocity += gravity * Time.deltaTime;

        float airControlFactor = 0.4f;
    
        float circularEffectFactor = 0.1f;
        float circularEffect = Mathf.Sin(playerController.jumpEllapsedTime * Mathf.PI) * circularEffectFactor;

        Vector3 jumpMove = playerController.transform.forward * inputDirection.y 
                         + playerController.transform.right * (inputDirection.x + circularEffect);

    
        if (jumpMove.sqrMagnitude  > 0.001f)
        {
            jumpMove.Normalize();

        }

        jumpMove *= (currentSpeed * airControlFactor);

        jumpMove.y = playerController.verticalVelocity;

        // Déplacement du joueur
        controller.Move(jumpSpeed * Time.deltaTime * jumpMove);

        // Détection de l'atterrissage
        if (playerController.isGrounded)
        {
            playerController.haspressedJump = false;
            playerController.isInAir = false;
            playerController.jumpEllapsedTime = 0f;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProcessJumping : MonoBehaviour
{
    public static void ProcessJumping(
        PlayerController playerController,
        CharacterController controller,
        HandsStateController handsStateController,
        Vector2 inputDirection,
        float jumpSpeed
    )
    {
        Debug.Log("Process Jumping");
        float gravity = -9.81f;

        // Ajout de la gravité et gestion de la vélocité verticale
        playerController.jumpEllapsedTime += Time.deltaTime;
        playerController.verticalVelocity += gravity * Time.deltaTime;

        // Mouvement en l'air avec la possibilité de guider le joueur
        float circularEffect = Mathf.Sin(playerController.jumpEllapsedTime * Mathf.PI) * 0.5f;

        Vector3 jumpMove;
        if (inputDirection == Vector2.zero)
        {
            jumpMove = Vector3.zero;
        }
        else
        {
            jumpMove = playerController.transform.forward * inputDirection.y +
                       playerController.transform.right * (inputDirection.x + circularEffect);
        }

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

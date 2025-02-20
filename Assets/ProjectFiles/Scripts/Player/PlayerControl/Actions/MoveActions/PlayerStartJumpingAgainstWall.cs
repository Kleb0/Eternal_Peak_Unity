using UnityEngine;


public class PlayerStarJumpAgainstWall : PlayerMove
{
    private static float gravity = -9.81f;
    public static void StartJumpAgainstWall(
        PlayerController playerController,
        CharacterController controller,
        HandsStateController handsStateController,
        Vector2 jumpHeight,
        Vector2 inputDirection,
        float jumpSpeed,
        float currentSpeed

    )
    {
        if (playerController.isGrounded)
        {
            playerController.verticalVelocity = Mathf.Max(Mathf.Sqrt(jumpHeight.y * -2f * gravity), 0.1f);
            playerController.isInAir = true;
        }

    }
}
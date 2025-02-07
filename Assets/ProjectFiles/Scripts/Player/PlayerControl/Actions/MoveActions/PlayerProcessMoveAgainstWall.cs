using UnityEngine;

public class PlayerProcessMoveAgainstWall
{
    public static void MoveAgainstWall(
        PlayerController playerController,
        CharacterController controller,
        float rangelimit,
        out bool canMoveLeft,
        out bool canMoveRight,
        out bool areBothHandsHoldingGrip,
        out Vector3 leftIkTargetPosition,
        out bool canMoveBackward,
        out bool canMoveForward,
        out bool canMoveLaterally,
        GameObject leftIkTarget
    )
    {
        canMoveLeft = false;
        canMoveRight = false;
        areBothHandsHoldingGrip = false;
        leftIkTargetPosition = Vector3.zero;
        canMoveBackward = false;
        canMoveForward = false;
        canMoveLaterally = false;     
        
        if (playerController.leftHandHoldingAGrip)
        {
            // Debug.Log($"Lateral Limit: {lateralLimit}");
                          
            bool isLeftHandHoldingGrip = playerController.leftHandHoldingAGrip;
            bool isRightHandHoldingGrip = playerController.rightHandHoldingAGrip;
            areBothHandsHoldingGrip = isLeftHandHoldingGrip && isRightHandHoldingGrip;

            float rightArmsBendingValue = playerController.rightArmBendingValue;
            float leftArmsBendingValue = playerController.leftArmBendingValue;

            Vector2 forwardBackward = playerController.playerSetDirection.GetForwardDirection();
            Vector2 rightLeft = playerController.playerSetDirection.GetRightDirection();
            
            if (leftIkTarget == null)
            {
                Debug.LogWarning("Left IK target is null in MoveAgainstWall.");
                return;
            }

         leftIkTargetPosition = leftIkTarget.transform.position;
        Vector3 playerPosition = controller.transform.position;

        // Offset relative to the target
        Vector3 offset = playerPosition - leftIkTargetPosition;
        float distanceToGrip = offset.magnitude;
        Vector3 normalizedOffset = offset.normalized;

        // Project the offset onto the local axes of the target
        float lateralDistance = Vector3.Dot(offset, leftIkTarget.transform.right);

        // Compute the fraction relative to the spherical boundary
        float fractionX = Mathf.Clamp(lateralDistance / rangelimit, -1f, 1f);

        // Ensure the movement stays within the spherical boundary
        bool armFullyExtended = leftArmsBendingValue >= 0.9f;
        bool notAtRightLimit = fractionX < 1f;
        bool notAtLeftLimit = fractionX > -1f;
        bool notAtForwardLimit = leftArmsBendingValue > 0.15f;
        bool notAtBackwardLimit = leftArmsBendingValue <= 0.9f;

        // Allow movement back toward the center when the arm is fully extended
        bool canReturnLeft = rightLeft.x < 0f && fractionX < 0f && armFullyExtended;  
        bool canReturnRight = rightLeft.x > 0f && fractionX > 0f && armFullyExtended; 

        // Compute movement constraints based on the spherical approach
        canMoveBackward = forwardBackward.y < 0f && notAtBackwardLimit;
        canMoveForward = forwardBackward.y > 0f && notAtForwardLimit;

        // Allow movement only if inside the limits or if returning to center
        canMoveLeft = rightLeft.x < 0f && notAtLeftLimit && !armFullyExtended || canReturnRight;
        canMoveRight = rightLeft.x > 0f && notAtRightLimit && !armFullyExtended || canReturnLeft;

        canMoveLaterally = canMoveLeft || canMoveRight;
     
        
        }     

    }
}

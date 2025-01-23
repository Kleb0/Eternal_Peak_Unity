using UnityEngine;

public class PlayerHoldingAGrip
{
    public static void MoveAgainstWall(
        PlayerController playerController,
        CharacterController controller,
        float lateralLimit,
        out bool canMoveLeft,
        out bool canMoveRight,
        out bool areBothHandsHoldingGrip,
        out Vector3 leftIkTargetPosition,
        out bool canMoveBackward,
        out bool canMoveForward,
        out bool canMoveLaterally
    )
    {
        
        canMoveLeft = false;
        canMoveRight = false;
        areBothHandsHoldingGrip = false;
        leftIkTargetPosition = Vector3.zero;
        canMoveBackward = false;
        canMoveForward = false;
        canMoveLaterally = false;

       
        // if (playerController == null || controller == null)
        // {
        //     Debug.LogError("PlayerController or CharacterController is null in MoveAgainstWall.");
        //     return;
        // }

      
        bool isLeftHandHoldingGrip = playerController.leftHandHoldingAGrip;
        bool isRightHandHoldingGrip = playerController.rightHandHoldingAGrip;
        areBothHandsHoldingGrip = isLeftHandHoldingGrip && isRightHandHoldingGrip;

        float rightarmsBendingValue = playerController.rightArmBendingValue;
        float leftarmsBendingValue = playerController.leftArmBendingValue;

        Vector2 forwardBackward = playerController.playerSetDirection.GetForwardDirection();
        Vector2 rightLeft = playerController.playerSetDirection.GetRightDirection();

        GameObject leftIkTarget = null;

        if (isLeftHandHoldingGrip)
        {
            leftIkTarget = playerController.leftHandHoldingGrip;
        }
        if (isRightHandHoldingGrip)
        {
            leftIkTarget = playerController.rightHandHoldingGrip;
        }

        
        if (isLeftHandHoldingGrip && isRightHandHoldingGrip)
        {
            areBothHandsHoldingGrip = true;
        }

        
        if (leftIkTarget == null)
        {
            Debug.LogWarning("Left IK target is null in MoveAgainstWall.");
            return;
        }

        Vector3 leftLimit = leftIkTarget.transform.position + leftIkTarget.transform.right * lateralLimit;
        Vector3 rightLimit = leftIkTarget.transform.position - leftIkTarget.transform.right * lateralLimit;
        Vector3 playerPosition = controller.transform.position;

    
        if (areBothHandsHoldingGrip)
        {
            canMoveLeft = playerPosition.x > leftLimit.x - 1f;
            canMoveRight = playerPosition.x < rightLimit.x - 1f;
        }
        else
        {
            canMoveLeft = playerPosition.x > leftLimit.x;
            canMoveRight = playerPosition.x < rightLimit.x;
        }

        
        canMoveBackward = forwardBackward.y < 0f && leftarmsBendingValue < 1f && rightarmsBendingValue < 1f;
        canMoveForward = forwardBackward.y > 0f && (leftarmsBendingValue > 0.15f || rightarmsBendingValue > 0.15f);
        canMoveLaterally = (rightLeft.x < 0f && canMoveLeft) || (rightLeft.x > 0f && canMoveRight);

      
        leftIkTargetPosition = leftIkTarget.transform.position;

        // Debug.Log(" Forward Backward value : " + forwardBackward.y + 
        //           " Left Arm Bending value : " + leftarmsBendingValue + 
        //           " Right Arm Bending Value : " + rightarmsBendingValue);
    }
}

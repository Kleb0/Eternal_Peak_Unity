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
        out Vector3 rightIkTargetPosition,
        out bool canMoveBackward,
        out bool canMoveForward,
        out bool canMoveLaterally,
        GameObject leftIkTarget,
        GameObject rightIkTarget
    )
    {
        canMoveLeft = false;
        canMoveRight = false;
        areBothHandsHoldingGrip = false;
        leftIkTargetPosition = Vector3.zero;
        rightIkTargetPosition = Vector3.zero;
        canMoveBackward = false;
        canMoveForward = false;
        canMoveLaterally = false;     
        
        if (playerController.leftHandHoldingAGrip || playerController.rightHandHoldingAGrip)
        {
            // Debug.Log($"Lateral Limit: {lateralLimit}");
                          
            WalkAgainstWallWithOneHand(
                playerController, controller, rangelimit, out canMoveLeft,
                out canMoveRight, out areBothHandsHoldingGrip, out leftIkTargetPosition, out rightIkTargetPosition,
                out canMoveBackward, out canMoveForward, out canMoveLaterally, leftIkTarget, rightIkTarget
            );     
        
        }

    }

    private static void WalkAgainstWallWithOneHand(
        PlayerController playerController,
        CharacterController controller,
        float rangelimit,
        out bool canMoveLeft,
        out bool canMoveRight,
        out bool areBothHandsHoldingGrip,
        out Vector3 leftIkTargetPosition,
        out Vector3 rightIkTargetPosition,
        out bool canMoveBackward,
        out bool canMoveForward,
        out bool canMoveLaterally,
        GameObject leftIkTarget,
        GameObject rightIkTarget
    )
    {
        canMoveLeft = false;
        canMoveRight = false;
        areBothHandsHoldingGrip = false;
        leftIkTargetPosition = Vector3.zero;
        rightIkTargetPosition = Vector3.zero;
        canMoveBackward = false;
        canMoveForward = false;
        canMoveLaterally = false;

        bool isLeftHandHoldingGrip = playerController.leftHandHoldingAGrip;
        bool isRightHandHoldingGrip = playerController.rightHandHoldingAGrip;
        areBothHandsHoldingGrip = isLeftHandHoldingGrip && isRightHandHoldingGrip;

        float rightArmsBendingValue = playerController.rightArmBendingValue;
        float leftArmsBendingValue = playerController.leftArmBendingValue;

        Vector2 forwardBackward = playerController.playerSetDirection.GetForwardDirection();
        Vector2 rightLeft = playerController.playerSetDirection.GetRightDirection();

        GameObject activeIkTarget = isLeftHandHoldingGrip ? leftIkTarget : rightIkTarget;
        
        Vector3 activeIkTargetPosition = Vector3.zero;
        float activeArmBendingValue = isLeftHandHoldingGrip ? leftArmsBendingValue : rightArmsBendingValue;

        if (activeIkTarget == null)
        {
            string hand = isLeftHandHoldingGrip ? "Left" : "Right";
            Debug.LogWarning($"Active {hand} hand Ik target is in Player Process Move Against Wall and target is {activeIkTarget}");
            return;
        }
        // else
        // {
        //     string hand = isLeftHandHoldingGrip ? "Left" : "Right";
        //     Debug.Log($"Active {hand} hand Ik target is {activeIkTarget.name}");
        // }

        activeIkTargetPosition = activeIkTarget.transform.position;

        if (isLeftHandHoldingGrip)
        {
                leftIkTargetPosition = activeIkTargetPosition;
        }            
        else
        {
            rightIkTargetPosition = activeIkTargetPosition;
        }
        
        Vector3 playerPosition = controller.transform.position;
        
        Vector3 offset = playerPosition - activeIkTargetPosition;

        float lateralDistance = Vector3.Dot(offset, controller.transform.right);

        float fractionX =Mathf.Clamp(lateralDistance / rangelimit, -1f, 1f);

        // Ensure the movement stays within the spherical boundary
        bool armFullyExtended = activeArmBendingValue >= 0.9f;
        bool notAtRightLimit = fractionX < 1f;
        bool notAtLeftLimit = fractionX > -1f;
        bool notAtForwardLimit = activeArmBendingValue > 0.15f;
        bool notAtBackwardLimit = activeArmBendingValue <= 0.9f;

        bool canReturnLeft = rightLeft.x < 0f && fractionX < 0f && armFullyExtended;  
        bool canReturnRight = rightLeft.x > 0f && fractionX > 0f && armFullyExtended; 

        canMoveBackward = forwardBackward.y < 0f && notAtBackwardLimit;
        canMoveForward = forwardBackward.y > 0f && notAtForwardLimit;

        canMoveLeft = rightLeft.x < 0f && notAtLeftLimit && !armFullyExtended || canReturnRight;
        canMoveRight = rightLeft.x > 0f && notAtRightLimit && !armFullyExtended || canReturnLeft;

        canMoveLaterally = canMoveLeft || canMoveRight;
    }

}

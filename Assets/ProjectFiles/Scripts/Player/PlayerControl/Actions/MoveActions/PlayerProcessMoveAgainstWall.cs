using UnityEngine;

public class PlayerProcessMoveAgainstWall
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

            // 1/ caculate the offset in function of the target
            Vector3 offeset = playerPosition - leftIkTargetPosition;

            // 2/ Project the offset on the right axis of the target
            float xLocal = Vector3.Dot(offeset, leftIkTarget.transform.right);

            // 3/ Convert this distance to a fraction in [-1 ; +1] if lateral limit represent
            // the max value to be able to reach ±1
            float fractionX = xLocal / lateralLimit;

            // 4/ Clamp the fraction to [-1 ; +1]
            fractionX = Mathf.Clamp(fractionX, -1f, 1f);

            bool notAtRightLimit = (fractionX > -1f);

            // bool notAtLeftLimit = (fractionX > -1f);
            bool notAtLeftLimit = (fractionX < 1f);
           
            // Vector3 leftLimit = leftIkTarget.transform.position + leftIkTarget.transform.right * lateralLimit;
            // Vector3 rightLimit = leftIkTarget.transform.position - leftIkTarget.transform.right * lateralLimit;
            // Vector3 playerPosition = controller.transform.position;

            canMoveBackward  = (forwardBackward.y < 0f && leftArmsBendingValue  < 1f && rightArmsBendingValue < 1f);
            canMoveForward   = (forwardBackward.y > 0f && (leftArmsBendingValue > 0.15f || rightArmsBendingValue > 0.15f));
            canMoveLeft = (rightLeft.x < 0f  && notAtLeftLimit && leftArmsBendingValue < 0.8f && rightArmsBendingValue < 0.8f); 
            canMoveRight = (rightLeft.x > 0f && notAtRightLimit && (leftArmsBendingValue > 0.15f || rightArmsBendingValue > 0.15f));

            Debug.Log(
                            $"PlayerPosition = {playerPosition:F1} (range is -1..1)"
                        );
            canMoveLaterally = (canMoveLeft || canMoveRight);
            // Debug.Log($"[MoveAgainstWall] canMoveLeft = {canMoveLeft}, canMoveRight = {canMoveRight}");
        }     

    }
}

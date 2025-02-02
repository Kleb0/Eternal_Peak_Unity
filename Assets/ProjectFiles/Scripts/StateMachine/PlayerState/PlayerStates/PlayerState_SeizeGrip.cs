using UnityEngine;

public class PlayerState_SeizeGrip : PlayerState
{
        public override string stateName { get; protected set; } = "Seize Grip";

        public override void EnterState()
        {
            Debug.Log("Start Seize Grip");
        }

}
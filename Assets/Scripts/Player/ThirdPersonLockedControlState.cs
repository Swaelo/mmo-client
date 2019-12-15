// ================================================================================================================================
// File:        ThirdPersonLockedControlState.cs
// Description: Active while the player is in third person mode and locked onto an enemy target
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class ThirdPersonLockedControlState : State
{
    //State Machine Controller
    private LocalPlayerController Controller;
    private void Awake() { Controller = GetComponent<LocalPlayerController>(); }

    protected override void OnStateInitialize(StateMachine Machine = null)
    {
        
    }

    protected override void OnStateEnter()
    {

    }

    protected override void OnStateExit()
    {

    }

    protected override void OnStateUpdate()
    {

    }

    protected override void OnStateFixedUpdate()
    {

    }

    protected override void OnStateLateUpdate()
    {

    }
}

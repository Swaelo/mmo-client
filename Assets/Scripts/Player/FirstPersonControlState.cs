// ================================================================================================================================
// File:        FirstPersonControlState.cs
// Description: Active while the player is in first person mode
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class FirstPersonControlState : State
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

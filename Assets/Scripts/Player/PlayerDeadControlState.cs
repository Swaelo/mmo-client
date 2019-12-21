// ================================================================================================================================
// File:        PlayerDeadControlState.cs
// Description:	Player cant move their character at all, camera rotates around their corpse until they choose to respawn
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerDeadControlState : State
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

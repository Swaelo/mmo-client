// ================================================================================================================================
// File:        PlayerIdleState.cs
// Description: Active while the player is standing still without performing any actions, any action can be performed from here
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerIdleState : State
{
    //Acquire and store a reference to the PlayerController class once the game starts so we always have access to it
    private PlayerCharacterController Controller;
    private void Awake() { Controller = GetComponent<PlayerCharacterController>(); }

    protected override void OnStateInitialize(StateMachine Machine = null)
    {
        base.OnStateInitialize(Machine);
    }

    protected override void OnStateEnter()
    {
        //Update the animator so it should be playing the idle animation
        Controller.AnimatorComponent.SetBool("IsGrounded", true);
    }

    protected override void OnStateExit()
    {
        
    }

    protected override void OnStateUpdate()
    {
        //Run a seperate update method ignore all user input while they are typing a message into the chat log
        if(ChatMessageInput.Instance.IsTyping)
        {
            UpdateNoInput();
            return;
        }

        //If the player is no longer touching the ground then we transition to the fall state
        if(!Controller.IsGrounded)
        {
            //Transition to the fall state
            Controller.Machine.SetState(GetComponent<PlayerFallState>());

            //Exit from this update function as we arent in this state anymore
            return;
        }

        //If the player performs an attack transition to the attack state
        if(Input.GetMouseButtonDown(0))
        {
            //Transition to the attack state
            Controller.Machine.SetState(GetComponent<PlayerAttackState>());

            //Exit from this update function as we arent in this state anymore
            return;
        }

        //Fetch a new movement vector to apply to the player and store it in the character controller
        Controller.NewMovementVector = Controller.ComputeMovementVector();

        //Adjust the characters YVelocity, apply it to the movement vector and transition to the falling state when they perform a jump action
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Update the characters YVelocity and apply that to our movement vector
            Controller.YVelocity = Controller.GetJumpHeight();
            Controller.NewMovementVector.y += Controller.YVelocity;

            //Set this movement vector to be applied to the player, and set that it needs to be moved into the Falling state
            Controller.NewState = "Fall";
            Controller.NewStateToTransition = true;

            //Exit from this update function as we arent in this state anymore
            return;
        }

        //If the travel distance exceeds the minimum amount then we need to transition to the move state
        if(Controller.MovementSinceLastUpdate > 0.05f)
        {
            //Transition to the move state
            Controller.Machine.SetState(GetComponent<PlayerMoveState>());

            //Exit from this update function as we arent in this state anymore
            return;
        }

        //Calculate and lerp towards a new target rotation value to face towards the direction the player is moving in
        Controller.NewRotation = Controller.ComputeTargetRotation(Controller.NewMovementVector);
        //Only apply the rotation if some movement was applied to the player
        Controller.QuaternionToApply = (Controller.NewMovementVector.x != 0f || Controller.NewMovementVector.z != 0f);
    }

    private void UpdateNoInput()
    {
        //Send empty movement vector to the character controller
        Controller.NewMovementVector = Vector3.zero;

        //Transition to falling state if we arent touching the ground
        if(!Controller.IsGrounded)
        {
            Controller.Machine.SetState(GetComponent<PlayerFallState>());
            return;
        }
    }
}

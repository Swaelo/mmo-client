// ================================================================================================================================
// File:        PlayerMoveState.cs
// Description: Active while the player is on the ground and moving somewhere, anything but attacking can be performed from here
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerMoveState : State
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

    }

    protected override void OnStateExit()
    {
        
    }

    protected override void OnStateUpdate()
    {
        //Run a seperate update method ignoring all user input while they are typing a message into the chat window
        if(ChatMessageInput.Instance.IsTyping)
        {
            UpdateNoInput();
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

            //Set this movement vector to be applied, and the fall state to be transition into
            Controller.NewState = "Fall";
            Controller.NewStateToTransition = true;

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

        //If the travel distance falls below a minimum amount then we transition to the idle state
        if(Controller.MovementSinceLastUpdate < 0.01f)
        {
            //Return to the idle state
            Controller.Machine.SetState(GetComponent<PlayerIdleState>());

            //Exit from this update function as we arent in this state anymore
            return;
        }

        //Calculate a new target rotation and lerp the player towards it so they face towards the direction they move
        Controller.NewRotation = Controller.ComputeTargetRotation(Controller.NewMovementVector);
        Controller.QuaternionToApply = true;
    }

    private void UpdateNoInput()
    {
        //Store empty movement vector in the character controller
        Controller.NewMovementVector = Vector3.zero;
        //Just transition to our idle state
        Controller.Machine.SetState(GetComponent<PlayerIdleState>());
    }
}

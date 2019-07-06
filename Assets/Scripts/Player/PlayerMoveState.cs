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
        //Fetch a new movement vector to apply to the player
        Vector3 MovementVector = Controller.ComputeMovementVector(false);

        //Adjust the characters YVelocity, apply it to the movement vector and transition to the falling state when they perform a jump action
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Update the characters YVelocity and apply that to our movement vector
            Controller.YVelocity = Controller.JumpHeight;
            MovementVector.y += Controller.YVelocity;

            //Apply this movement vector to the player and transition to the fall state
            Controller.ControllerComponent.Move(MovementVector * Controller.MoveSpeed * Time.deltaTime);
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

        //Apply movement to the character
        Controller.ControllerComponent.Move(MovementVector * Controller.MoveSpeed * Time.deltaTime);

        //Calculate how much distance the player has travelled and send that to the animator
        float HorizontalMovement = Controller.GetHorizontalMovement();
        Controller.AnimatorComponent.SetFloat("Movement", HorizontalMovement);

        //If the travel distance falls below a minimum amount then we transition to the idle state
        if(HorizontalMovement < 0.01f)
        {
            //Return to the idle state
            Controller.Machine.SetState(GetComponent<PlayerIdleState>());

            //Exit from this update function as we arent in this state anymore
            return;
        }

        //Calculate a new target rotation and lerp the player towards it so they face towards the direction they move
        Quaternion TargetRotation = Controller.ComputeTargetRotation(MovementVector);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, Controller.TurnSpeed * Time.deltaTime);
    }
}

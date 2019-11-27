// ================================================================================================================================
// File:        PlayerFallState.cs
// Description: Active while the player is currently falling, still allows movement to be applied but the player cant attack
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerFallState : State
{
    //Fall state remains to a minimum of 1 quarter of a second to avoid automatically returning to the idle state when a jump first occurs and
    //the character is detected as touching the ground before they have ascended high enough in their jump
    private float StateTime = 0.25f;
    private float StateTimeLeft;

    //The player can jump a second time before leaving the falling state
    private bool DoubleJumped = false;

    //Acquire and store a reference to the PlayerController class once the game starts so we always have access to it
    private PlayerCharacterController Controller;
    private void Awake() { Controller = GetComponent<PlayerCharacterController>(); }

    protected override void OnStateInitialize(StateMachine Machine = null)
    {
        base.OnStateInitialize(Machine);
    }

    protected override void OnStateEnter()
    {
        //Update the animator to start the falling animation and reset the state timer and doublejump flags
        Controller.AnimatorComponent.SetBool("IsGrounded", false);
        StateTimeLeft = StateTime;
        DoubleJumped = false;
    }

    protected override void OnStateExit()
    {
        //Update the animator to leave the falling animation
        Controller.AnimatorComponent.SetBool("IsGrounded", true);
    }

    protected override void OnStateUpdate()
    {
        //Use different update method ignoring all user input while player is typing a message into the chat
        if(ChatMessageInput.Instance.IsTyping)
        {
            UpdateNoInput();
            return;
        }

        //Fetch new movement vector for the player and store it in the character controller
        Controller.NewMovementVector = Controller.ComputeMovementVector();

        //Check if the player tries to perform a second jump if they havnt already
        if (!DoubleJumped && Input.GetKeyDown(KeyCode.Space))
        {
            //Update the animator to perform the flip animation and set the double jump flag so they cant jump again
            PlayerManagementPacketSender.Instance.SendPlayAnimationAlert("Flip");
            Controller.AnimatorComponent.SetTrigger("DoubleJump");
            DoubleJumped = true;

            //Update the characters YVelocity and apply this to the movement vector to apply the new jump forces
            Controller.YVelocity = Controller.GetJumpHeight();
            Controller.NewMovementVector.y += Controller.YVelocity;
        }

        //Count down the statetimer, then once its depleted keep checking until the player hits the ground then transition out of the falling state
        StateTimeLeft -= Time.deltaTime;
        if (StateTimeLeft <= 0f && Controller.IsGrounded)
        {
            //Transition to idle with no movement detected
            if (Controller.MovementSinceLastUpdate < 0.01f)
                Controller.Machine.SetState(GetComponent<PlayerIdleState>());
            //Otherwise transition to move state
            else
                Controller.Machine.SetState(GetComponent<PlayerMoveState>());

            //Now that we have told the machine to transition out of this fall state we should immediately exit this states update function
            return;
        }

        //Calculate and lerp towards a new target rotation value to face towards the direction the player is moving in
        Controller.NewRotation = Controller.ComputeTargetRotation(Controller.NewMovementVector);
        //Only apply the rotation if some movement was applied to the player
        Controller.QuaternionToApply = (Controller.NewMovementVector.x != 0f || Controller.NewMovementVector.z != 0f);
    }

    private void UpdateNoInput()
    {
        //Fetch a new movement vector for the player ignoring all user input
        Controller.NewMovementVector = Controller.ComputeIgnoredMovementVector();

        //Count down the timer restricting us from leaving this state to early
        StateTimeLeft -= Time.deltaTime;
        if(StateTimeLeft <= 0f && Controller.IsGrounded)
        {
            //Transition into our idle state
            Controller.Machine.SetState(GetComponent<PlayerIdleState>());
            //Exit this as we are moving into the idle state
            return;
        }
    }
}

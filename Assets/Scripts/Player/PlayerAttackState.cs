﻿// ================================================================================================================================
// File:        PlayerAttackState.cs
// Description: Active while the player is in the middle of an attack action, allows them to continue their combo into additional
//              attacks, otherwise it returns back to Idle/Move state to once again allow locomotion
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerAttackState : State
{
    //Attack state remains for 1 half of a second
    private float StateTime = 0.5f;
    private float StateTimeRemaining;

    //If the player clicks the attack button again before this attack finished they will transition into a combo after this state
    private bool ContinueCombo = false;
    private bool ComboPerformed = false;

    //Acquire and store a reference to the PlayerController class once the game starts so we always have access to it
    private PlayerCharacterController Controller;
    private void Awake() { Controller = GetComponent<PlayerCharacterController>(); }

    //Base State class virtual function overrides
    protected override void OnStateInitialize(StateMachine Machine = null)
    {
        base.OnStateInitialize(Machine);
    }

    //When first entering the Attack state, trigger the initial Attack animation and reset combo counting variables
    protected override void OnStateEnter()
    {
        Controller.AnimatorComponent.SetTrigger("Attack");
        Controller.AnimatorComponent.SetBool("Attack2", false);
        StateTimeRemaining = StateTime;
        ContinueCombo = false;
        ComboPerformed = false;
    }

    protected override void OnStateExit() { }

    protected override void OnStateUpdate()
    {
        //If another attack input is detected before exiting this state then continue the combo into another attack
        if(Input.GetMouseButtonDown(0))
            ContinueCombo = true;

        //Count down the state timer, when it runs out we need to either leave this start or continue the attack combo
        StateTimeRemaining -= Time.deltaTime;
        if(StateTimeRemaining <= 0f)
        {
            //If no more attack inputs were detected then we transition out of the attack state
            if(!ContinueCombo || ComboPerformed)
            {
                //First update the animator computer so it knows to transition out of any current attack animation immediately\
                Controller.AnimatorComponent.SetTrigger("StopAttack");

                //Computer how much horizontal distance the player has travelled so we know whether to transition between the idle or move state
                float HorizontalMovement = Controller.GetHorizontalMovement();

                //Transition to idle with no movement detected
                if(HorizontalMovement < 0.01f)
                    Controller.Machine.SetState(GetComponent<PlayerIdleState>());
                //Otherwise transition to move state
                else
                    Controller.Machine.SetState(GetComponent<PlayerMoveState>());

                //Now that we have told the machine to transition out of this attack state we should immediately exit this states update function
                return;
            }

            //Otherwise reset the state timer and perform the next attack in the combo
            StateTimeRemaining = StateTime;
            ComboPerformed = true;  //Set this flag so the combo can only be performed once and not infinitely
            Controller.AnimatorComponent.SetBool("Attack2", true);
        }
    }
}

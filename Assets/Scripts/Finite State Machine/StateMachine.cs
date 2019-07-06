// ================================================================================================================================
// File:        StateMachine.cs
// Description: Based on Noah Bannister's modular character controller at https://noahbannister.blog/2017/12/19/building-a-modular-character-controller/
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected List<State> States = new List<State>();   //All available states being managed by this machine
    public State StartingState;                         //What State the machine should start in
    protected State CurrentState;                       //The current state of the machine

    //Set to the starting state once the game begins
    public void Start()
    {
        SetState(StartingState);
    }

    //Returns the current state object
    public State GetCurrentState { get { return CurrentState; }}

    //Switches the current state to a specific state object
    public virtual bool SetState(State NewState)
    {
        bool Success = false;

        //Switch to the new state if its valid and its not already the current state
        if(NewState && NewState != CurrentState)
        {
            State OldState = CurrentState;
            CurrentState = NewState;
            if(OldState)
                OldState.StateExit();
            CurrentState.StateEnter();
            Success = true;
        }

        return Success;
    }

    //Switches the current state to a state of the the given type
    public virtual bool SetState<StateType> () where StateType : State
    {
        bool Success = false;

        //Search through the list of available state for one which matches the given type
        foreach(State State in States)
        {
            //If we find a matching state then swap to that one
            if(State is StateType)
            {
                Success = SetState(State);
                return Success;
            }
        }

        //If we couldnt find any matching state check if there is one on this gameObject
        State StateComponent = GetComponent<StateType>();
        if(StateComponent)
        {
            //Update to this state if it was found on the gameObject
            StateComponent.Initialize(this);
            States.Add(StateComponent);
            Success = SetState(StateComponent);
            return Success;
        }

        //If we still couldnt find the state we were looking for then we need to initialize a new one to use
        State NewState = gameObject.AddComponent<StateType>();
        NewState.Initialize(this);
        States.Add(NewState);
        Success = SetState(NewState);
        return Success;
    }
}

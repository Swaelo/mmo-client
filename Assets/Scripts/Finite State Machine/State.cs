// ================================================================================================================================
// File:        State.cs
// Description: Inherit from this class and override the virtual methods to define your desired behaviour, based on Noah Bannisters
//              blog found at https://noahbannister.blog/2017/12/19/building-a-modular-character-controller/
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class State : MonoBehaviour
{
    //The StateMachine used for managing changing between this state and others that are available
    public StateMachine Machine;

    //Used for checking if a given state has been defined correctly and is valid for use before changing to it
    public static implicit operator bool (State State)
    {
        return State != null;
    }

    //Assigns this states StateMachine that will manage it, make sure you call this in your overloaded initialize function when
    //defining your own State objects
    public void Initialize(StateMachine Machine)
    {
        this.Machine = Machine;
        OnStateInitialize(Machine);
    }

    //All these functions need to be overloaded when inheriting from this class to define your own State objects
    protected virtual void OnStateInitialize(StateMachine Machine = null) { }
    protected virtual void OnStateExit() { }
    protected virtual void OnStateEnter() { }
    protected virtual void OnStateUpdate() { }

    //Called by the StateMachine when it enters into this State
    public void StateEnter()
    {
        enabled = true;
        OnStateEnter();
    }

    //Called by the StateMachine when its exits from this State
    public void StateExit()
    {
        OnStateExit();
    }

    //Called by the StateMachine every frame while this State is active
    public void StateUpdate()
    {
        OnStateUpdate();
    }

    //Ensures the enable function isnt called again if this state was already the active one
    public void OnEnable()
    {
        enabled = this != Machine.GetCurrentState;
    }

    //Ensures the disable function isnt called again if this state was already the active one
    public void OnDisable()
    {
        enabled = this == Machine.GetCurrentState;
    }
}

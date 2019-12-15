// ================================================================================================================================
// File:        ThirdPersonFreeControlState.cs
// Description: Active while the player is in third person mode without being locked onto a target
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class ThirdPersonFreeControlState : State
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
        //Get a new movement vector and apply velocity upon it
        Vector3 MovementVector = ComputeMovementVector();
        MovementVector.y += Controller.Velocity;

        //Apply new movement and rotation to the player
        Controller.Controller.Move(MovementVector * Controller.MoveSpeed * Time.deltaTime);
        if (MovementVector.x != 0f || MovementVector.z != 0f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, ComputeMovementQuaternion(MovementVector), Controller.TurnSpeed * Time.deltaTime);
    }

    protected override void OnStateFixedUpdate()
    {
        
    }

    protected override void OnStateLateUpdate()
    {
        
    }

    //Computes a new movement vector based on the users input and the current camera orientation relative to the player
    private Vector3 ComputeMovementVector()
    {
        Vector3 MovementVector = new Vector3();

        //Poll X/Y Input axes relative to the camera transform
        Vector3 MovementX = Vector3.Cross(transform.up, Controller.Camera.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;

        //Apply these axes to the movement vector
        MovementVector = Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;

        return MovementVector;
    }

    //Computes a new rotation based on the current movement vector to face the player in the direction they are moving
    private Quaternion ComputeMovementQuaternion(Vector3 MovementVector)
    {
        //Return a quaternion looking in the direction the player is moving, keeping their current X rotation value
        Quaternion Rotation = Quaternion.LookRotation(MovementVector);
        Vector3 Eulers = Rotation.eulerAngles;
        Eulers.x = transform.rotation.x;
        Rotation.eulerAngles = Eulers;
        return Rotation;
    }
}

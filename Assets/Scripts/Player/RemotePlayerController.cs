// ================================================================================================================================
// File:        RemotePlayerController.cs
// Description:	Controls the remote player characters and moves them towards their target position sent from the server
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerController : MonoBehaviour
{
    //Controller used to move this player around the scene
    private CharacterController Controller;
    //Current target position for the player to be moved towards
    public Vector3 TargetPosition;
    //How fast the player will move towards its target position
    public float MoveSpeed = 5f;
    //How fast the player will turn to face the direction it moving in
    public float TurnSpeed = 300f;
    //Animator Controller component used to control the players animations
    private Animator AnimationController;
    //Track how much distance is being travelled over time so we can send our movement speed to the animator
    private Vector3 PreviousFramePosition;

    void Awake()
    {
        //Assign a reference to the players character controller and animator components when the scene starts
        Controller = GetComponent<CharacterController>();
        AnimationController = GetComponent<Animator>();
        PreviousFramePosition = transform.position;
    }

    void Update()
    {
        //Get our current offset from our target location
        Vector3 TargetOffset = TargetPosition - transform.position;
        //If we are more than .1 units away from our target location then we want to be moving towards it
        if(TargetOffset.magnitude > .1f)
        {
            //Normalize the target offset and apply movement speed
            TargetOffset = TargetOffset.normalized * MoveSpeed;
            //Move the character closer towards its target location
            Controller.Move(TargetOffset * Time.deltaTime);

            //Calculate a new target rotation and lerp towards that so they player faces the direction they are moving
            transform.rotation = Quaternion.RotateTowards(transform.rotation, ComputeTargetRotation(TargetOffset), TurnSpeed * Time.deltaTime);

        }

        //Calculate how much distance has been travelled since the last frame and pass it on to the animation controller
        //so it knows when to transition between idle and walking animations
        float DistanceTravelled = Vector3.Distance(transform.position, PreviousFramePosition);
        AnimationController.SetFloat("Movement", DistanceTravelled);
        //Update the previous frame position for the next frames distance calculation
        PreviousFramePosition = transform.position;

        //Tell the animation controller if we are touching the ground or not
        AnimationController.SetBool("IsGrounded", IsTouchingGround());
    }

    //Shoots a raycast directly down from the players feet to check how far away the ground is to determine if they are falling or standing
    private bool IsTouchingGround()
    {
        RaycastHit GroundHit;   //Shoot a raycast directly downwards, if we hit something we need to check its distance
        if(Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out GroundHit, 1))
        {
            //If we hit something we need to see how far away it is, if its within a certain distance then we consider the character to be grounded
            float GroundDistance = Vector3.Distance(transform.position, GroundHit.point);
            return GroundDistance <= 0.5f;
        }

        //The raycast only goes to a distance of 1 ingame unit, if it didnt hit anything at all then the character obviously isnt on the ground
        return false;
    }

    //Calculates a new target rotation based on a new MovementVector for the player to lerp towards so they always face the direction they are moving
    public Quaternion ComputeTargetRotation(Vector3 MovementVector)
    {
        //Return the current rotation while no user input has been detected
        if(MovementVector == Vector3.zero)
            return transform.rotation;

        //Otherwise we want the player to face towards the direction they are moving so return that rotation
        Quaternion TargetRotation = Quaternion.LookRotation(MovementVector);
        Vector3 Eulers = TargetRotation.eulerAngles;
        Eulers.x = transform.rotation.x;
        TargetRotation.eulerAngles = Eulers;
        return TargetRotation;
    }
}

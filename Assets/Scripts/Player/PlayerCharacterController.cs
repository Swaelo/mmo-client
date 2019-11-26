// ================================================================================================================================
// File:        PlayerCharacterController.cs
// Description:	Stores various controller configurations for the player and controls the transition between different states
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    //Movement configuration variables
    public float MoveSpeed = 5;         //How fast the player character moves around the game world
    public float JumpHeight = 5;        //How high the player can jump
    public float TurnSpeed = 300.0f;    //How fast the character rotates to face towards their target direction

    //Tracking falling velocity / when jumping is available etc.
    public float YVelocity = 0.0f;      //Updated with gravity/jump forces and applied to the characters MovementVector before applying it
    public float FallSpeed = 0.1f;      //How much force is applied to the YVelocity while the character is falling to act as gravity
    public bool IsGrounded = false;     //This is kept up to date at all times, and checked by various player states to allow jumping at times

    //Components which are often accessed and used during the different player states, these should be assigned through the inspector so they
    //are always available for easy access and so that resources are never wasted having to make GetComponent calls during run time
    public Transform CameraTransform;   //Players movement directions are relative to the direction the camera is facing
    public CharacterController ControllerComponent; //Movement vectors are applied to this component to apply movement to the character
    public Animator AnimatorComponent;  //Various values and triggers are set during runtime to control the players animations
    public StateMachine Machine;    //StateMachine object used to manage the players current state and called upon to transition between them

    //Store which values were previously broadcasted to the game server
    private Vector3 LastPositionTransmission;
    private Vector3 LastMovementTransmission;
    private Quaternion LastRotationTransmission;
    public Vector3 CurrentMovementVector = Vector3.zero;

    public Vector3 PreviousFramePosition;   //Used to computing distance travelled over time, passed to animation controller for state transition control

    public void Awake()
    {
        //Assign some initial values to any member variables which need them
        PreviousFramePosition = transform.position;
        LastPositionTransmission = transform.position;
        LastMovementTransmission = Vector3.zero;
        LastRotationTransmission = transform.rotation;
    }

    public void Update()
    {
        //Keep track of when the character is grounded or in the air
        IsGrounded = IsTouchingGround();

        //Tell the state machine to update the current state
        Machine.GetCurrentState.StateUpdate();

        //Store the position for next frames movement distance calculations
        PreviousFramePosition = transform.position;
    }

    //Checks all the current values and broadcasts any to the game server which have changed since last transmission
    public void TransmitValues()
    {
        //Transmit all values if any of them have changed
        if(LastPositionTransmission != transform.position ||
            LastMovementTransmission != CurrentMovementVector ||
            LastRotationTransmission != transform.rotation)
        {
            //Send the new values to the server
            PlayerManagementPacketSender.Instance.SendLocalPlayerCharacterUpdate(transform.position, CurrentMovementVector, transform.rotation);
            //Store the sent values as the last ones transmitted
            LastPositionTransmission = transform.position;
            LastMovementTransmission = CurrentMovementVector;
            LastRotationTransmission = transform.rotation;
        }
    }

    //Shoots a raycast directly down from the players feet to check how far away the ground is to determine if they are falling or standing
    private bool IsTouchingGround()
    {
        RaycastHit GroundHit;   //Shoot a raycast directly downwards, if we hit something we need to check its distance
        if(Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out GroundHit, 1))
        {
            //If we hit something we need to see how far away it is, if its within a certain distance then we consider the character to be grounded
            float GroundDistance = Vector3.Distance(transform.position, GroundHit.point);
            return GroundDistance <= 0.25f;
        }

        //The raycast only goes to a distance of 1 ingame unit, if it didnt hit anything at all then the character obviously isnt on the ground
        return false;
    }

    //Calculates a new movement vector for the character based on user input and the cameras current location
    public Vector3 ComputeMovementVector(bool ApplyGravity = false) //Some states will want gravity to be applied while others wont
    {
        //First figure out what direction the X and Y movement axes should be based on the cameras position, this is so press W will
        //always move the character away from the camera, S will always move toward the camera etc.
        Vector3 MovementX = Vector3.Cross(transform.up, CameraTransform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;

        //Now use these movement direction with player input to calculate a new movement vector for the player
        Vector3 MovementVector = Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;

        //Apply the players current YVelocity value to the movement vector if we have been told to apply gravity to the movement vector
        if(ApplyGravity)
        {
            YVelocity -= FallSpeed;
            MovementVector.y += YVelocity;
        }

        //Store the movement vector
        CurrentMovementVector = MovementVector;

        //Return the final movement vector that was calculated
        return MovementVector;
    }

    public float GetHorizontalMovement()    //Calcualtes how much horizontal (X/Z axis) distance has been travelled since last frame
    {
        Vector3 CurrentPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 PreviousPosition = new Vector3(PreviousFramePosition.x, 0f, PreviousFramePosition.z);
        return Vector3.Distance(CurrentPosition, PreviousPosition);
    }

    //Calculates a new movement vector for the character while ignoring all user input
    public Vector3 ComputeIgnoredMovementVector(bool ApplyGravity = false)
    {
        //Start with an empty movement vector
        Vector3 MovementVector = Vector3.zero;

        //Apply gravity to it if asked to
        if(ApplyGravity)
        {
            YVelocity -= FallSpeed;
            MovementVector.y += YVelocity;
        }

        //retrun the final movement vector
        return MovementVector;
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

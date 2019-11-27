// ================================================================================================================================
// File:        PlayerCharacterController.cs
// Description:	Stores various controller configurations for the player and controls the transition between different states
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    //Movement configuration variables
    private float MoveSpeed = 8f;         //How fast the player character moves around the game world
    private float JumpHeight = 2f;        //How high the player can jump
    private float TurnSpeed = 300f;    //How fast the character rotates to face towards their target direction

    //Movement Variable Getters
    public float GetJumpHeight() { return JumpHeight; }

    //Tracking falling velocity / when jumping is available etc.
    public float YVelocity = 0.0f;      //Updated with gravity/jump forces and applied to the characters MovementVector before applying it
    private float FallSpeed = 8f;      //How much force is applied to the YVelocity while the character is falling to act as gravity
    public bool IsGrounded = false;     //This is kept up to date at all times, and checked by various player states to allow jumping at times

    //Components which are often accessed and used during the different player states, these should be assigned through the inspector so they
    //are always available for easy access and so that resources are never wasted having to make GetComponent calls during run time
    public Transform CameraTransform;   //Players movement directions are relative to the direction the camera is facing
    public CharacterController ControllerComponent; //Movement vectors are applied to this component to apply movement to the character
    public Animator AnimatorComponent;  //Various values and triggers are set during runtime to control the players animations
    public StateMachine Machine;    //StateMachine object used to manage the players current state and called upon to transition between them

    //Sets a MovementVector to be applied to the character in the next LateUpdate function
    public Vector3 NewMovementVector = Vector3.zero;
    public string NewState;
    public bool NewStateToTransition = false;

    //Sets a new target rotation to be moved towards in the next LateUpdate function
    public Quaternion NewRotation;
    public bool QuaternionToApply = false;

    //Used to calculate distance travelled over time, passed onto animation controller for idle/walk transitioning
    public float MovementSinceLastUpdate = 0f;
    private Vector3 PreviousXZ;

    //How often to send our updated position/rotation/movement input values to the game server
    private float PlayerUpdateInterval = 0.25f;
    private float NextPlayerUpdate = 0.25f;
    //Store which values were previously broadcasted to the game server
    private Vector3 LastPositionUpdate = Vector3.zero;
    private Vector3 LastMovementUpdate = Vector3.zero;
    private Quaternion LastRotationUpdate = Quaternion.identity;

    public void Awake()
    {
        PreviousXZ = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    public void Update()
    {
        //Keep track of when the character is grounded or in the air
        IsGrounded = IsTouchingGround();

        //Tell the state machine to update the current state
        Machine.GetCurrentState.StateUpdate();

        //Count down the timer for transmitting new values to the server
        NextPlayerUpdate -= Time.deltaTime;
        if(NextPlayerUpdate <= 0f)
        {
            //Reset the transmission timer
            NextPlayerUpdate = PlayerUpdateInterval;

            //Check if any values have changed since we last sent them to the game server
            bool NewValues = LastPositionUpdate != transform.position ||
                LastMovementUpdate != NewMovementVector ||
                LastRotationUpdate != transform.rotation;

            //Send our current values to the server if they have changed
            if(NewValues)
            {
                //Send the current values
                PlayerManagementPacketSender.Instance.SendLocalPlayerCharacterUpdate(transform.position, NewMovementVector, transform.rotation);

                //Store them all as being those that were last sent to the server
                LastPositionUpdate = transform.position;
                LastMovementUpdate = NewMovementVector;
                LastRotationUpdate = transform.rotation;
            }
        }
    }

    public void FixedUpdate()
    {
        //Apply gravity to the movement vector if we arent touching the ground
        if (!IsGrounded)
            YVelocity -= FallSpeed * Time.fixedDeltaTime;
        NewMovementVector.y += YVelocity;

        //Apply movement vector if we have one to apply
        ControllerComponent.Move(NewMovementVector * MoveSpeed * Time.fixedDeltaTime);

        //Apply new rotation if we have one to apply
        if (QuaternionToApply)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, NewRotation, TurnSpeed * Time.fixedDeltaTime);
            QuaternionToApply = false;
        }

        //Transition to new state if we have one to move into
        if (NewStateToTransition)
        {
            Machine.SetState(GetComponent<PlayerFallState>());
            NewStateToTransition = false;
        }

        //Calculate and store distance moved since last time movement was applied
        Vector3 CurrentXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        MovementSinceLastUpdate = Vector3.Distance(CurrentXZ, PreviousXZ);
        PreviousXZ = CurrentXZ;

        //Pass new movement distance value onto the animation controller
        AnimatorComponent.SetFloat("Movement", MovementSinceLastUpdate);
    }

    //Shoots a raycast directly down from the players feet to check how far away the ground is to determine if they are falling or standing
    private bool IsTouchingGround()
    {
        RaycastHit GroundHit;   //Shoot a raycast directly downwards, if we hit something we need to check its distance
        if(Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out GroundHit, 1))
        {
            //If we hit something we need to see how far away it is, if its within a certain distance then we consider the character to be grounded
            float GroundDistance = Vector3.Distance(transform.position, GroundHit.point);
            return GroundDistance <= 0.1f;
        }

        //The raycast only goes to a distance of 1 ingame unit, if it didnt hit anything at all then the character obviously isnt on the ground
        return false;
    }

    //Calculates a new movement vector for the character based on user input and the cameras current location
    public Vector3 ComputeMovementVector() //Some states will want gravity to be applied while others wont
    {
        //First figure out what direction the X and Y movement axes should be based on the cameras position, this is so press W will
        //always move the character away from the camera, S will always move toward the camera etc.
        Vector3 MovementX = Vector3.Cross(transform.up, CameraTransform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;

        //Now use these movement direction with player input to calculate a new movement vector for the player
        Vector3 MovementVector = Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;

        //Return the final movement vector that was calculated
        return MovementVector;
    }

    //Calculates a new movement vector for the character while ignoring all user input
    public Vector3 ComputeIgnoredMovementVector()
    {
        //Start with an empty movement vector
        Vector3 MovementVector = Vector3.zero;

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

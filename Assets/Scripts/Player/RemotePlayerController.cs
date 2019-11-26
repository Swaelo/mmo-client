// ================================================================================================================================
// File:        RemotePlayerController.cs
// Description:	Controls the remote player characters and moves them towards their target position sent from the server
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class RemotePlayerController : MonoBehaviour
{
    //Movement Control
    private CharacterController MovementController; //Used to move the player around the scene
    private float MovementSpeed = 5f;   //How fast the player moves through the game world
    private float RotationSpeed = 300f; //How fast the player turns around
    //Animation Control
    private Animator AnimationController;   //Used to control the players animation states
    private Vector3 PreviousFramePosition;  //For computing distance travelled over time, sent to animation controller for idle/moving state transitions
    //Values last sent to us from the game server and if they are new needing to be processed
    private Vector3 ServerSidePosition = Vector3.zero;
    private Quaternion ServerSideRotation = Quaternion.identity;
    private Vector3 ServerSideMovement = Vector3.zero;
    private bool NewPosition = false;

    private void Awake()
    {
        //Assign references and set default values for member variables
        MovementController = GetComponent<CharacterController>();
        AnimationController = GetComponent<Animator>();
        PreviousFramePosition = transform.position;
    }

    private void Update()
    {
        //Snap player to new position values whenever they are provided from the server
        if(NewPosition)
        {
            //Check difference between current and new position value
            float MovementDrift = Vector3.Distance(transform.position, ServerSidePosition);
            //Snap players position if they have drifted too far
            if (MovementDrift > 3f)
                transform.position = ServerSidePosition;
            //Turn off the new values flag now that its been updated
            NewPosition = false;
        }
        //Whenever Updating without having just recieved new position values from the server, we move the client where we think they will do
        else
        {
            //Keep moving player in direction the server told us they are going
            if(ServerSideMovement != Vector3.zero)
            {
                Vector3 MovementVector = ServerSideMovement * MovementSpeed * Time.deltaTime;
                MovementController.Move(MovementVector);
            }
            else
            {
                //Lerp towards current server side position
                transform.position = Vector3.Lerp(transform.position, ServerSidePosition, MovementSpeed * Time.deltaTime);
            }

            //Check if the player character is currently moving or not
            bool IsMoving = Vector3.Distance(transform.position, PreviousFramePosition) > 0.1f;
            //While they are moving rotate them to face in the same direction they are moving in
            if(IsMoving)
            {
                Vector3 MovementDirection = PreviousFramePosition - transform.position;
                Vector3 TargetPosition = transform.position - MovementDirection;
                Vector3 TargetOffset = TargetPosition - transform.position;
                TargetOffset = TargetOffset.normalized * MovementSpeed;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, ComputeTargetRotation(TargetOffset), RotationSpeed * Time.deltaTime);
            }
            //While standing still, we lerp their rotation towards the server side rotation value
            else
                transform.rotation = Quaternion.RotateTowards(transform.rotation, ServerSideRotation, RotationSpeed * Time.deltaTime);
        }

        //Calculate distance travelled since last frame update, and check if the player is standing on the ground or not
        float DistanceTravelled = Vector3.Distance(transform.position, PreviousFramePosition);
        bool OnGround = IsGrounded();
        //Pass these values onto the animation controller so it knows when to transition between animation states correctly
        AnimationController.SetFloat("DistanceTravelled", DistanceTravelled);
        AnimationController.SetBool("IsGrounded", OnGround);

        //Store the players current location for computing distance travelled in the next frame update
        PreviousFramePosition = transform.position;
    }

    //Updates the remote player with new values passed in
    public void UpdateValues(Vector3 NewPosition, Vector3 NewMovement, Quaternion NewRotation)
    {
        //Set the NewPosition flag if we receieved a new value
        if (NewPosition != ServerSidePosition)
            this.NewPosition = true;

        //Store all the new values that have been provided
        ServerSidePosition = NewPosition;
        ServerSideMovement = NewMovement;
        ServerSideRotation = NewRotation;
    }

    //Uses raycasting to check the distance between the players feet and whatever ground is below them to determine if they are standing or in the air
    private bool IsGrounded()
    {
        RaycastHit GroundHit;   //Where information will be stored about what object the raycast hit
        //Shoot a raycast from the characters feet directly down and see if it hits anything they are able to stand on
        if(Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out GroundHit, 1))
        {
            //If the raycast hit something, check how far away it is, if its close enough then we know the player is standing on it
            float GroundDistance = Vector3.Distance(transform.position, GroundHit.point);
            //If the ground is less than half a unit of distance away then we know the player is standing on it
            return GroundDistance <= 0.5f;
        }

        //If the raycast didnt hit anything then we know the player is definitely in the air
        return false;
    }

    //Calculates a new target rotation value to face in the direction the player is moving towards
    private Quaternion ComputeTargetRotation(Vector3 MovementVector)
    {
        //Return current rotation is movement is zero
        if (MovementVector == Vector3.zero)
            return transform.rotation;

        //Otherwise we want the player to face in the direction they are moving in
        Quaternion TargetRotation = Quaternion.LookRotation(MovementVector);
        Vector3 Eulers = TargetRotation.eulerAngles;
        Eulers.x = transform.rotation.x;
        TargetRotation.eulerAngles = Eulers;
        return TargetRotation;
    }
}
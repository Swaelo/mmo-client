// ================================================================================================================================
// File:        LocalPlayerController.cs
// Description:	Allows the player to move their character throughout the game world
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class LocalPlayerController : MonoBehaviour
{
    //Component References
    public Transform Camera;
    public CharacterController Controller;
    public Animator Animator;

    //Movement Settings
    public float MoveSpeed = 8f;
    public float TurnSpeed = 300f;
    public float FallSpeed = 8f;

    //Jump Settings
    public bool IsGrounded;
    public float JumpHeight = 2f;
    public float DoubleJumpHeight = 3f;
    public bool DoubleJumped = false;

    //State Control
    public StateMachine PlayerMachine;
    public string NextState;
    public bool ChangeState = false;

    //Movement Values
    public float Velocity = 0f;
    private Vector3 PreviousPosition;

    //Respawn Settings
    private bool ShouldRespawn;
    private Vector3 RespawnPosition;
    private Quaternion RespawnRotation;

    //Network State Broadcasting
    private float BroadcastInterval = 0.1f;
    private float NextBroadcast = 0.1f;
    private Vector3 LastBroadcastPosition;
    private Quaternion LastBroadcastRotation;

    public void Awake()
    {
        //Store position for distance calculations
        PreviousPosition = transform.position;
    }

    public void Update()
    {
        //Store currented grounded value from the character controller
        IsGrounded = Controller.isGrounded;

        //Allow jumping, apply gravity to velocity
        JumpFall();

        //Force move to spawn pos when told to do so
        if(ShouldRespawn)
        {
            transform.position = RespawnPosition;
            transform.rotation = RespawnRotation;
            ShouldRespawn = false;
        }
        //Otherwise just update the current state to move normally
        else
            PlayerMachine.GetCurrentState.StateUpdate();

        //Pass values on to the animator component
        UpdateAnimator();

        //Periodically broadcast the players values to the server
        BroadcastValues();
    }

    public void FixedUpdate()
    {
        PlayerMachine.GetCurrentState.StateFixedUpdate();
    }

    public void LateUpdate()
    {
        PlayerMachine.GetCurrentState.StateLateUpdate();
    }

    //Broadcasts the players current position and rotation values to the game server so they know where we are
    private void BroadcastValues()
    {
        //Count time the timer until the next broadcast event
        NextBroadcast -= Time.deltaTime;
        if(NextBroadcast <= 0.0f)
        {
            //Unity and Bepu have opposite Z directions, so we need to flip that before we send it to the server
            Vector3 PositionZFlip = new Vector3(transform.position.x, transform.position.y, -transform.position.z);
            //Broadcast position and rotation to the server whenever the previous broadcast is out of date
            if(PositionZFlip != LastBroadcastPosition && PlayerManagementPacketSender.Instance != null)
            {
                PlayerManagementPacketSender.Instance.SendPlayerPositionUpdate(PositionZFlip);
                LastBroadcastPosition = PositionZFlip;
            }
            if(transform.rotation != LastBroadcastRotation && PlayerManagementPacketSender.Instance != null)
            {
                PlayerManagementPacketSender.Instance.SendPlayerRotationUpdate(transform.rotation);
                LastBroadcastRotation = transform.rotation;
            }
            //Reset the broadcast timer
            NextBroadcast = BroadcastInterval;
        }
    }

    //Computes and passed on all the required values to the animator for it to function correctly
    private void UpdateAnimator()
    {
        //Compute horizontal distance travelled since last update
        Vector3 CurrentXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 PreviousXZ = new Vector3(PreviousPosition.x, 0f, PreviousPosition.z);
        float TravelDistance = Vector3.Distance(CurrentXZ, PreviousXZ);
        //Store current distance for next updates distance calculation
        PreviousPosition = transform.position;

        //Pass all values to the animator
        Animator.SetFloat("Movement", TravelDistance);
        Animator.SetBool("IsGrounded", TouchingGround());
    }

    //Uses raycasting to check if the player is touching the ground
    public bool TouchingGround()
    {
        return IsGrounded;
    }

    //Allows jumping, updates velocity and applies gravity
    private void JumpFall()
    {
        //Check if the character is grounded
        bool Grounded = TouchingGround();

        //Pressing SpaceBar while grounded lets the player jump
        if (Grounded && Input.GetKeyDown(KeyCode.Space))
            Velocity = JumpHeight;

        //Pressing SpaceBar while in the air and having not yet performed a double jump lets them double jump
        if (!Grounded && !DoubleJumped && Input.GetKeyDown(KeyCode.Space))
        {
            //Apply velocity, set the double jumped flag and trigger the flip animation
            Velocity = DoubleJumpHeight;
            DoubleJumped = true;
            Animator.SetTrigger("DoubleJump");
        }

        //Reset Velocity and DoubleJump flag while grounded
        if (Grounded)
            DoubleJumped = false;
        //Apply gravity while not grounded
        else
            Velocity -= FallSpeed * Time.deltaTime;
    }
    
    //Flags the character to force move itself to the set spawn location provided by the game server
    public void Respawn(Vector3 SpawnPosition, Quaternion SpawnRotation)
    {
        ShouldRespawn = true;
        RespawnPosition = SpawnPosition;
        RespawnRotation = SpawnRotation;
    }
}
// ================================================================================================================================
// File:        LocalPlayerController.cs
// Description:	Allows the player to move their character throughout the game world
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class LocalPlayerController : MonoBehaviour
{
    [Header("Component References")]
    public Transform CameraTransform;       //Reference to the player's camera so we can apply new position and rotations to it
    public Camera CameraComponent;          //Reference to the cameras Camera component, used to determine what enemies are in view when searching for a new target to lock onto
    public CharacterController Controller;  //Reference to the players CharacterController component, movement vectors are applied to this to move the character around the scene
    public Animator Animator;               //Reference to the character Animator component, values such as distance travelled are passed on so it knows when to transition between animation states

    [Header("Player Movement Settings")]
    public float MoveSpeed = 8f;    //How fast the player character can move
    public float TurnSpeed = 300f;  //How fast the player character can turn around
    public float FallSpeed = 8f;    //How much gravity is applied to the character while in the air

    [Header("Player Jump Settings")]
    public bool IsGrounded;             //Tracks when the player is on the ground or in the air
    public float JumpHeight = 2f;       //How high the player can jump from off the ground
    public float DoubleJumpHeight = 3f; //How high the player jumps from in the air while doing a flip for their double jump
    public bool DoubleJumped = false;   //Tracks if the player has performed a double jump since they last landed on the ground

    [Header("Camera Targetting Settings")]
    public GameObject CameraDummy;              //Moved around the scene, then faced towards the current player target to figure out what the player cameras rotation should be while in the LockedControlState
    public GameObject TargettingReticleObject;  //GameObject Reference containing the TargettingReticle UI sprite to indicate which target the player is currently locked onto
    public RectTransform CanvasRect;            //The RectTransform component of the UI Canvas
    public RectTransform TargettingReticleRect; //The RectTransform component of the TargettingReticle object which is modified so the reticle remains above the current target lock
    public float MaxTargetRange = 5.0f;         //The maximum distance enemies can be from the player and still be picked up as a new target when trying to lock onto something
    public GameObject PlayerCameraPivot;        //The players camera pivots around this object, placed roughly at the players head (instead of just pivoting around the actual player, which would place the camera at feet level)
    public GameObject PlayerEnemyTarget = null; //The players current enemy target lock

    [Header("Camera Mouse Sensitivity Settings")]
    public float CameraRotationSpeed = 50f;     //How fast the camera rotates around the player while moving the mouse from side to side
    public float CameraPanSpeed = 50f;          //How fast the camera pans above/below the player while moving the mouse up and down
    public float MouseDampening = 0.02f;    //Dampening value applied to any horizontal/vertical mouse input that is received for pivoting/panning the camera around the player while in the FreeControlState

    [Header("Camera Zoom Levels")]
    public float CameraZoomSpeed = 5f;      //How fast the camera zooms in and out while scrolling the mouse wheel
    public float CameraZoom = 3.5f;    //The current zoom distance setting how far the camera should be away from the player
    public float CameraCloseZoomLimit = 1f; //Limits how far the camera can zoom in towards the player
    public float CameraFarZoomLimit = 8f;   //Limits how far the camera can zoom out away from the player

    [Header("Camera Rotation Values")]
    public float CameraRotation = 0f;       //The cameras current rotation around the player character
    public float CameraPan = 0f;            //The cameras panning above/below the player
    public float CameraPanDownLimit = -25f; //How far down the camera can pan towards the ground
    public float CameraPanUpLimit = 80f;    //How far up the camera can pan above the players head

    [Header("Camera Override Settings")]
    public bool NewCameraValues = false;    //Flag set once we recieve instructions from the server to force set the camera to some new values
    public float NewCameraZoom;             //New zoom level to be applied as instructed by the game server
    public float NewCameraXRotation;        //New x rotation value to be applied as instructed by the game server
    public float NewCameraYRotation;        //New y rotation value to be applied as instructed by the game server

    [Header("Player Override Settings")]
    private bool ShouldRespawn;         //Flag set once we recieve instructions from the server to force set the player to some new values
    private Vector3 RespawnPosition;    //New position to apply to the player as instructed by the game server
    private Quaternion RespawnRotation; //New rotation to apply to the player as instructed by the game server

    [Header("Network Camera State Broadcasting")]
    private float CameraBroadcastInterval = 5f; //How often to broadcast the cameras current zoom and rotation settings to the game server
    private float NextCameraBroadcast = 5f;     //How long until the next broadcast event occurs where we send the current camera zoom/rotation values to the game server

    [Header("Network Player State Broadcasting")]
    private float PlayerBroadcastInterval = 0.1f;   //How often to broadcast the players position/rotation values to the game server
    private float NextPlayerBroadcast = 0.1f;       //How long until the next broadcast event occurs where we send the players current position/rotation values to the game server
    private Vector3 LastPlayerPositionBroadcast;    //The last position value that was broadcast out to the game server, compared with current as to not broadcast the same values to the server again
    private Quaternion LastPlayerRotationBroadcast; //The last rotation value that was broadcast out to the game server, compared with current as to not broadcast the same values to the server again

    [Header("State Control")]
    public StateMachine StateMachine;   //State Machine which handles the current player controller state, accessed by individual states so it can be told when to change to other states

    [Header("Player Movement Values")]
    public float Velocity = 0f;         //Players current vertical velocity, applied to the .y value of the movement vector every frame before that vector is passed into the Controller.Move function
    private Vector3 PreviousPosition;   //Players location in the previous frame, used to calculate distance travelled since then which is passed to the Animator for transitioning between walk/run animations

    [Header("Cursor Lock Settings")]
    public bool CursorLocked = false;   //Tracks when the cursor is locked to the screen

    public void Awake()
    {
        CameraRotation = CameraTransform.rotation.eulerAngles.y;
        CameraPan = CameraTransform.rotation.eulerAngles.x;
        CameraZoom = Vector3.Distance(CameraTransform.position, PlayerCameraPivot.transform.position);

        //Assign targetting reticle sprite reference and disable it
        TargettingReticleObject = GameObject.Find("Targetting Reticle");
        TargettingReticleRect = TargettingReticleObject.GetComponent<RectTransform>();
        TargettingReticleObject.SetActive(false);

        //Store position for distance calculations
        PreviousPosition = transform.position;
    }

    public void Update()
    {
        //Give ways to manage the cursor lockstate while testing in the editor
#if UNITY_EDITOR
            ManageCursorLockState();
#endif

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
            StateMachine.GetCurrentState.StateUpdate();

        //Pass values on to the animator component
        UpdateAnimator();

        //Periodically broadcast the players values to the server
        BroadcastPlayerValues();
        BroadcastCameraValues();
    }

    public void FixedUpdate()
    {
        StateMachine.GetCurrentState.StateFixedUpdate();
    }

    public void LateUpdate()
    {
        StateMachine.GetCurrentState.StateLateUpdate();
    }

    //Manages the mouse lockstate while testing in the unity editor
    public void ManageCursorLockState()
    {
        //Clicking on the game locks the cursor if its not already locked
        if (!CursorLocked && Input.GetMouseButtonDown(0))
        {
            CursorLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Pressing escape unlocks the cursor if its already locked
        if (CursorLocked && Input.GetKeyDown(KeyCode.Escape))
        {
            CursorLocked = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //Toggles the mouse lockstate, controls when the player is able to control the camera
    public void SetCursorLockState(bool LockCursor)
    {
        CursorLocked = LockCursor;
    }

    //Broadcasts the player cameras current zoom and rotation values to the server
    private void BroadcastCameraValues()
    {
        //Count the timer until it expires
        NextCameraBroadcast -= Time.deltaTime;
        if(NextCameraBroadcast <= 0.0f)
        {
            //Broadcast the current camera values to the server
            if(PlayerManagementPacketSender.Instance != null)
                PlayerManagementPacketSender.Instance.SendLocalPlayerCameraUpdate(CameraZoom, CameraRotation, CameraPan);

            //Reset the timer
            NextCameraBroadcast = CameraBroadcastInterval;
        }
    }

    //Broadcasts the players current position/rotation values to the server
    private void BroadcastPlayerValues()
    {
        //Count the timer until it expires
        NextPlayerBroadcast -= Time.deltaTime;
        if(NextPlayerBroadcast <= 0.0f)
        {
            //Unity and Bepu have opposite pointing z axis, so we need to flip that before we send it to the server
            Vector3 PlayerPositionZFlip = new Vector3(transform.position.x, transform.position.y, -transform.position.z);

            //Broadcast the values only when they have changed from what was last broadcast to the server
            if(PlayerManagementPacketSender.Instance != null && PlayerPositionZFlip != LastPlayerPositionBroadcast)
            {
                PlayerManagementPacketSender.Instance.SendPlayerPositionUpdate(PlayerPositionZFlip);
                LastPlayerPositionBroadcast = PlayerPositionZFlip;
            }
            if(PlayerManagementPacketSender.Instance != null && transform.rotation != LastPlayerRotationBroadcast)
            {
                PlayerManagementPacketSender.Instance.SendPlayerRotationUpdate(transform.rotation);
                LastPlayerRotationBroadcast = transform.rotation;
            }

            //Reset the timer
            NextPlayerBroadcast = PlayerBroadcastInterval;
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

    //Flags the character to force move itself to a new position/rotation instructed by the server
    public void ForceSetPlayer(Vector3 NewPosition, Quaternion NewRotation)
    {
        ShouldRespawn = true;
        RespawnPosition = NewPosition;
        RespawnRotation = NewRotation;
    }

    //Flags the camera to force move itself to new zoom and rotation values instructed by the server
    public void ForceSetCamera(float NewZoom, float NewXRotation, float NewYRotation)
    {
        NewCameraValues = true;
        NewCameraZoom = NewZoom;
        NewCameraXRotation = NewXRotation;
        NewCameraYRotation = NewYRotation;
    }

    //Applies new camera values as instructed by the game server
    public void ForceSetCameraValues()
    {
        CameraRotation = NewCameraXRotation;
        CameraPan = NewCameraYRotation;
        CameraZoom = NewCameraZoom;
        NewCameraValues = false;
    }

    //Limits how far the player can pan the camera up and down so gimbal lock doesnt occur
    public float PreventGimbalLock(float CameraPan)
    {
        if (CameraPan < -360f)
            CameraPan += 360f;
        if (CameraPan > 360f)
            CameraPan -= 360f;
        return Mathf.Clamp(CameraPan, CameraPanDownLimit, CameraPanUpLimit);
    }

    //Limits how far the player can zoom the camera to/from the player
    public float LimitCameraZoom(float CameraZoom)
    {
        return Mathf.Clamp(CameraZoom, CameraCloseZoomLimit, CameraFarZoomLimit);
    }
}
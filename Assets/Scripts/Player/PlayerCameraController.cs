// ================================================================================================================================
// File:        PlayerCameraController.cs
// Description:	Implements a WoW like 3rd person camera controller
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    //Adjustable movement speed values for mouselook speeds
    public float MouseXSpeed = 30;
    public float MouseYSpeed = 30;
    public float MouseDampening = 0.02f;

    //Current/Min and Max camera distance values, adjusted using mouse scrollwheel
    public float CurrentCameraDistance = 3.5f;
    private float MinimumCameraDistance = 1.0f;
    private float MaximumCameraDistance = 8.0f;

    //Current/Min and Max rotation values that are allowed, adjusted while moving the cursor and holding down the LMB
    public float CurrentXRotation = 0f;
    public float CurrentYRotation = 0f;
    private float MinimumYRotation = -20f;
    private float MaximumYRotation = 80f;

    //Components which are often accessed and used during run time, these should be set in the inspector so the script always has access
    //to them and never has to waste resources using GetComponent calls to find them
    public Transform PlayerTransform;       //This is the root transform of the player character object
    public GameObject CameraPivotTarget;    //This is a child object of the player character which the camera will rotate around

    //For hard setting the camera to a specific zoom/rotation, values are stored here, then set during the next LateUpdate
    private float NewCameraDistance;
    private float NewXRotation;
    private float NewYRotation;
    private bool NewValuesToSet = false;

    //How often to send our updated camera values to the game server
    private float CameraUpdateInterval = 5.0f;
    private float NextCameraUpdate = 5.0f;

    //Store which values were previously broadcasted to the game server
    private float LastZoomUpdate = 0f;
    private float LastXRotationUpdate = 0f;
    private float LastYRotationUpdate = 0f;

    //Track when the mouse cursor is locked to the window
    private bool CursorLocked = false;

    //Fetch and store the current camera rotation values when the scene first begins
    private void Start()
    {
        //Store current rotation values
        CurrentXRotation = transform.eulerAngles.x;
        CurrentYRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        ToggleCursorLock();
    }

    private void ToggleCursorLock()
    {
        //Click on the screen somewhere to enable the cursor lock
        if (!CursorLocked && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            CursorLocked = true;
        }

        //Press escape to disable the cursor lock
        if (CursorLocked && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            CursorLocked = false;
        }
    }

    private void LateUpdate()
    {
        //Set camera to some specific values if some have been provided
        if (NewValuesToSet)
            SetNewValues();
        //Otherwise we just track input to update camera values as normal
        else
        {
            RotateCamera();
            ZoomCamera();
        }

        //Compute and apply new camera position/rotation values
        Quaternion NewRotation = Quaternion.Euler(CurrentYRotation, CurrentXRotation, 0f);
        Vector3 NewPosition = NewRotation * new Vector3(0f, 0f, -CurrentCameraDistance) + CameraPivotTarget.transform.position;
        transform.position = NewPosition;
        transform.rotation = NewRotation;
    }

    private void SetNewValues()
    {
        CurrentXRotation = NewXRotation;
        CurrentYRotation = NewYRotation;
        CurrentCameraDistance = NewCameraDistance;
        NewValuesToSet = false;
    }

    //While LMB is held down, track cursor movement and use that to change the cameras rotation
    private void RotateCamera()
    {
        //Only rotate the camera around while the cursor is locked
        if(CursorLocked)
        {
            CurrentXRotation += Input.GetAxis("Mouse X") * MouseXSpeed * CurrentCameraDistance * MouseDampening;
            CurrentYRotation -= Input.GetAxis("Mouse Y") * MouseYSpeed * MouseDampening;
            CurrentYRotation = ClampAngle(CurrentYRotation, MinimumYRotation, MaximumYRotation);
        }
    }

    //Track the mouse scrollwheel and use that to change the cameras zoom level
    private void ZoomCamera()
    {
        //Check if the mouse is currently hovering over the chat window
        if(ChatWindowCursorTracker.IsMouseOverChat)
        {
            //Ignore any input for zooming the camera while the mouse is over the chat window
            return;
        }

        //Scrolling the mouse wheel adjusts the current distance between the camera and the player allowing you to zoom in and out
        float CameraZoomAdjustment = Input.GetAxis("Mouse ScrollWheel");
        float TargetCameraDistance = CurrentCameraDistance - CameraZoomAdjustment * 5f;
        CurrentCameraDistance = Mathf.Clamp(CurrentCameraDistance - CameraZoomAdjustment * 5f, MinimumCameraDistance, MaximumCameraDistance);
    }

    //Helper function used while calculating new YRotation values to avoid gimbal lock from happening and keeping the values within acceptable parameters
    private float ClampAngle(float CurrentAngle, float MinimumValue, float MaximumValue)
    {
        if(CurrentAngle < -360f)
            CurrentAngle += 360f;
        if(CurrentAngle > 360f)
            CurrentAngle -= 360f;
        return Mathf.Clamp(CurrentAngle, MinimumValue, MaximumValue);
    }

    //Takes in custom camera zoom and rotation values and sets the camera to match these values
    public void SetCamera(float Zoom, float XRotation, float YRotation)
    {
        //Store these values in the class and set the flag so they are applied in the next LateUpdate call
        NewCameraDistance = Zoom;
        NewXRotation = XRotation;
        NewYRotation = YRotation;
        NewValuesToSet = true;
    }
}





//private void Update()
//{

//    ////Count down the timer for transmitting new values to the server
//    //NextCameraUpdate -= Time.deltaTime;
//    //if(NextCameraUpdate <= 0f)
//    //{
//    //    //Reset the transmission timer
//    //    NextCameraUpdate = CameraUpdateInterval;

//    //    //Check if any values have changed since we last sent them to the game server
//    //    bool NewValues = LastZoomUpdate != CurrentCameraDistance ||
//    //        LastXRotationUpdate != CurrentXRotation ||
//    //        LastYRotationUpdate != CurrentYRotation;

//    //    //Send out current values to the server if they have changed
//    //    if(NewValues)
//    //    {
//    //        //Send the current values
//    //        if(PlayerManagementPacketSender.Instance != null)
//    //            PlayerManagementPacketSender.Instance.SendLocalPlayerCameraUpdate(CurrentCameraDistance, CurrentXRotation, CurrentYRotation);

//    //        //Store them all as being those that were last sent to the server
//    //        LastZoomUpdate = CurrentCameraDistance;
//    //        LastXRotationUpdate = CurrentXRotation;
//    //        LastYRotationUpdate = CurrentYRotation;
//    //    }
//    //}
//}
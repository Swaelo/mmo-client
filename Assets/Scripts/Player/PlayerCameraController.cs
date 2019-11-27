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

    //Current/Min and Max rotation values that are allowed, adjusted while moving the cursor and holding down the RMB
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

    //Fetch and store the current camera rotation values when the scene first begins
    private void Start()
    {
        //Store current rotation values
        CurrentXRotation = transform.eulerAngles.x;
        CurrentYRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        //Lock and hide the cursor when the user starts holding the RMB down
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Unlock and view the cursor when they release the RMB
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //Count down the timer for transmitting new values to the server
        NextCameraUpdate -= Time.deltaTime;
        if(NextCameraUpdate <= 0f)
        {
            //Reset the transmission timer
            NextCameraUpdate = CameraUpdateInterval;

            //Check if any values have changed since we last sent them to the game server
            bool NewValues = LastZoomUpdate != CurrentCameraDistance ||
                LastXRotationUpdate != CurrentXRotation ||
                LastYRotationUpdate != CurrentYRotation;

            //Send out current values to the server if they have changed
            if(NewValues)
            {
                //Send the current values
                PlayerManagementPacketSender.Instance.SendLocalPlayerCameraUpdate(CurrentCameraDistance, CurrentXRotation, CurrentYRotation);

                //Store them all as being those that were last sent to the server
                LastZoomUpdate = CurrentCameraDistance;
                LastXRotationUpdate = CurrentXRotation;
                LastYRotationUpdate = CurrentYRotation;
            }
        }
    }

    private void LateUpdate()
    {
        //If we have new values that the camera needs to be set to then we apply those this frame and ignore any player input until the next frame
        if(NewValuesToSet)
        {
            //Force the camera to the new settings values, then disable the flag after the values have been updated
            CurrentXRotation = NewXRotation;
            CurrentYRotation = NewYRotation;
            CurrentCameraDistance = NewCameraDistance;
            NewValuesToSet = false;
        }
        //If not, then we just track user input to update camera values as normal
        else
        {
            //Update the camera X and Y rotation values based on cursor movement whenever the RMB is being held down
            RotateCamera();
            //Update the cameras zoom level based on mouse wheel input
            ZoomCamera();
        }

        //Compute new updated target position and rotation values for the camera
        Quaternion TargetRotation = Quaternion.Euler(CurrentYRotation, CurrentXRotation, 0f);
        Vector3 TargetPosition = TargetRotation * new Vector3(0f, 0f, -CurrentCameraDistance) + CameraPivotTarget.transform.position;

        //Apply these new values to set the camera to its target location
        transform.position = TargetPosition;
        transform.rotation = TargetRotation;
    }

    //While RMB is held down, track cursor movement and use that to change the cameras rotation
    private void RotateCamera()
    {
        if (Input.GetMouseButton(1))
        {
            //Holding RMB + moving the cursor will rotate the camera around the players pivot point
            CurrentXRotation += Input.GetAxis("Mouse X") * MouseXSpeed * CurrentCameraDistance * MouseDampening;
            CurrentYRotation -= Input.GetAxis("Mouse Y") * MouseYSpeed * MouseDampening;
            CurrentYRotation = ClampAngle(CurrentYRotation, MinimumYRotation, MaximumYRotation);
        }
    }

    //Track the mouse scrollwheel and use that to change the cameras zoom level
    private void ZoomCamera()
    {
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

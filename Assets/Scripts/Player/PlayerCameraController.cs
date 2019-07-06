// ================================================================================================================================
// File:        PlayerCameraController.cs
// Description:	Implements a WoW like 3rd person camera controller
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    //Adjustable movement speed values for mouselook speeds
    public float MouseXSpeed = 30;
    public float MouseYSpeed = 30;
    public float MouseDampening = 0.02f;

    //Current/Min and Max camera distance values, adjusted using mouse scrollwheel
    private float CurrentCameraDistance = 3.5f;
    private float MinimumCameraDistance = 1.0f;
    private float MaximumCameraDistance = 8.0f;

    //Current/Min and Max rotation values that are allowed, adjusted while moving the cursor and holding down the RMB
    private float CurrentXRotation = 0f;
    private float CurrentYRotation = 0f;
    private float MinimumYRotation = -20f;
    private float MaximumYRotation = 80f;

    //Inside the Update function we keep track of when the RMB is and isnt being held down, then inside LateUpdate we apply camera movement when its held down
    private bool RMBHeld = false;

    //Components which are often accessed and used during run time, these should be set in the inspector so the script always has access
    //to them and never has to waste resources using GetComponent calls to find them
    public Transform PlayerTransform;       //This is the root transform of the player character object
    public GameObject CameraPivotTarget;    //This is a child object of the player character which the camera will rotate around

    //Fetch and store the current camera rotation values when the scene first begins
    private void Start()
    {
        CurrentXRotation = transform.eulerAngles.x;
        CurrentYRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        //Keep track of when the RMB is being held down
        RMBHeld = Input.GetMouseButton(1);
        //Hide and Lock the mouse cursor whenever the RMB is being held down, Display and Unlock it at all other times
        Cursor.visible = !RMBHeld;
        Cursor.lockState = RMBHeld ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void LateUpdate()
    {
        //Update the camera X and Y rotation values based on cursor movement whenever the RMB is being held down
        if(RMBHeld)
        {
            //Holding RMB + moving the cursor will rotate the camera around the players pivot point
            CurrentXRotation += Input.GetAxis("Mouse X") * MouseXSpeed * CurrentCameraDistance * MouseDampening;
            CurrentYRotation -= Input.GetAxis("Mouse Y") * MouseYSpeed * MouseDampening;
            CurrentYRotation = ClampAngle(CurrentYRotation, MinimumYRotation, MaximumYRotation);
        }

        //Scrolling the mouse wheel adjusts the current distance between the camera and the player allowing you to zoom in and out
        float CameraZoomAdjustment = Input.GetAxis("Mouse ScrollWheel");
        float TargetCameraDistance = CurrentCameraDistance - CameraZoomAdjustment * 5f;
        CurrentCameraDistance = Mathf.Clamp(CurrentCameraDistance - CameraZoomAdjustment * 5f, MinimumCameraDistance, MaximumCameraDistance);

        //Compute new updated target position and rotation values for the camera
        Quaternion TargetRotation = Quaternion.Euler(CurrentYRotation, CurrentXRotation, 0f);
        Vector3 TargetPosition = TargetRotation * new Vector3(0f, 0f, -CurrentCameraDistance) + CameraPivotTarget.transform.position;

        //Apply these new values to set the camera to its target location
        transform.position = TargetPosition;
        transform.rotation = TargetRotation;
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
}

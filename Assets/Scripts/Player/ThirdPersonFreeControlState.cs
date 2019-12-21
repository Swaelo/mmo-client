// ================================================================================================================================
// File:        ThirdPersonFreeControlState.cs
// Description: Active while the player is in third person mode without being locked onto a target
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
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
        //Clicking middle mouse / tapping Q will lock onto an enemy target if one can be found
        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Q))
            TryTargetLock();

        //Get a new movement vector and apply velocity upon it
        Vector3 MovementVector = ComputeMovementVector();
        MovementVector.y += Controller.Velocity;

        //Apply new movement and rotation to the player
        Controller.Controller.Move(MovementVector * Controller.MoveSpeed * Time.deltaTime);
        if (MovementVector.x != 0f || MovementVector.z != 0f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, ComputeMovementRotation(MovementVector), Controller.TurnSpeed * Time.deltaTime);
    }

    protected override void OnStateFixedUpdate()
    {
        
    }

    protected override void OnStateLateUpdate()
    {
        //Apply new override settings when instructed to by the game server
        if (Controller.NewCameraValues)
            Controller.ForceSetCameraValues();
        //Otherwise we just process input and update from that
        else
        {
            //Do nothing with the camera if the cursor isnt locked
            if (!Controller.CursorLocked)
                return;

            //Poll input which will be applied onto the camera to adjust the current rotation/pan values
            float RotationInput = Input.GetAxis("Mouse X") * Controller.CameraRotationSpeed * Controller.MouseDampening;
            float PanInput = -Input.GetAxis("Mouse Y") * Controller.CameraPanSpeed * Controller.MouseDampening;

            //Apply these values to the cameras current rotation and panning values
            Controller.CameraRotation += RotationInput;
            Controller.CameraPan += PanInput;
            Controller.CameraPan = Controller.ClampCameraAngle(Controller.CameraPan, Controller.CameraPanDownLimit, Controller.CameraPanUpLimit);

            //Zoom the camera
            if (!ChatWindowCursorTracker.IsMouseOverChat)
            {
                float ZoomAdjustment = Input.GetAxis("Mouse ScrollWheel");
                float NewZoomLevel = Controller.CameraZoom - ZoomAdjustment * Controller.CameraZoomSpeed;
                Controller.CameraZoom = Mathf.Clamp(NewZoomLevel, Controller.CameraCloseZoomLimit, Controller.CameraFarZoomLimit);
            }
        }

        //Compute new target position and rotation values for the camera
        Quaternion NewCameraRotation = Quaternion.Euler(Controller.CameraPan, Controller.CameraRotation, 0f);
        Vector3 NewCameraPosition = NewCameraRotation * new Vector3(0f, 0f, -Controller.CameraZoom) + Controller.PlayerCameraPivot.transform.position;

        //Apply the new camera settings
        Controller.CameraTransform.transform.position = NewCameraPosition;
        Controller.CameraTransform.transform.rotation = NewCameraRotation;
    }

    //Locks onto whatever enemy is available then changed to LockedControlState if able to
    private void TryTargetLock()
    {
        //Start by grabbing a list of all the enemies in the scene
        List<GameObject> Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));

        //Filter through the list so we only have enemies that are able to be targetting by the player
        List<GameObject> ValidTargets = new List<GameObject>();
        foreach(GameObject Enemy in Enemies)
        {
            //Enemies are valid targets if they're withing the players targetting range and they can be seen by the player
            bool InRange = Vector3.Distance(Controller.PlayerCameraPivot.transform.position, Enemy.transform.position) <= Controller.MaxTargetRange;
            bool InView = Enemy.GetComponent<VisibilityTracker>().IsVisible;

            //Add them to the list of valid targets if they passed all the tests
            if (InRange && InView)
                ValidTargets.Add(Enemy);
        }

        //Exit the function if we couldnt find any valid targets
        if (ValidTargets.Count == 0)
            return;

        //Out of all the valid targets, we want to find the one which is closest to the center of the players screen
        GameObject Closest = null;
        float Dot = -2f;
        foreach(GameObject Target in ValidTargets)
        {
            Vector3 LocalPoint = Controller.CameraTransform.InverseTransformPoint(Target.transform.position).normalized;
            float Test = Vector3.Dot(LocalPoint, Vector3.forward);
            if(Test > Dot)
            {
                Dot = Test;
                Closest = Target;
            }
        }

        //Now lock in the new target and transition to the LockedControlState
        if(Closest != null)
        {
            Controller.PlayerEnemyTarget = Closest;
            Controller.StateMachine.SetState(GetComponent<ThirdPersonLockedControlState>());
        }
    }

    //Computes a new movement vector based on the users input and the current camera orientation relative to the player
    private Vector3 ComputeMovementVector()
    {
        //Poll X/Y Input axes relative to the camera transform
        Vector3 MovementX = Vector3.Cross(transform.up, Controller.CameraTransform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;

        //Return a new movement vector calculated using these input axes
        return Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;
    }

    //Computes a new rotation based on the current movement vector to face the player in the direction they are moving
    private Quaternion ComputeMovementRotation(Vector3 MovementVector)
    {
        //Return a quaternion looking in the direction the player is moving, keeping their current X rotation value
        Quaternion Rotation = Quaternion.LookRotation(MovementVector);
        Vector3 Eulers = Rotation.eulerAngles;
        Eulers.x = transform.rotation.x;
        Rotation.eulerAngles = Eulers;
        return Rotation;
    }
}

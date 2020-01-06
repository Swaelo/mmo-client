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
        ////Clicking middle mouse / tapping Q will lock onto an enemy target if one can be found
        //if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Q))
        //    TryTargetLock();

        //Get a new movement vector and apply velocity upon it
        Vector3 MovementVector = ComputeMovementVector(Controller.CursorLocked);
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
        //Camera movement is not possible while the cursor is not locked to the screen
        if (!Controller.CursorLocked)
            return;

        //Apply new override settings when instructed to by the game server
        if (Controller.NewCameraValues)
            Controller.ForceSetCameraValues();
        //Otherwise process input and update camera location when the cursor is locked
        else
            ApplyFreeCameraMovement();
    }

    //Applys free camera movement from the users mouse input
    private void ApplyFreeCameraMovement()
    {
        //Apply users mouse input to update the cameras Rotation/Pan values around the player
        Controller.CameraRotation += Input.GetAxis("Mouse X") * Controller.CameraRotationSpeed * Controller.MouseDampening;
        Controller.CameraPan = Controller.PreventGimbalLock(Controller.CameraPan + -Input.GetAxis("Mouse Y") * Controller.CameraPanSpeed * Controller.MouseDampening);

        //Mouse input should also be applied to update the cameras zoom distance, but only while the mouse is not hovered above the chat window
        if (!ChatWindowCursorTracker.IsMouseOverChat)
            Controller.CameraZoom = Controller.LimitCameraZoom(Controller.CameraZoom - Input.GetAxis("Mouse ScrollWheel") * Controller.CameraZoomSpeed);

        //Compute a new target position/rotation value for the camera based on these values
        Quaternion TargetCameraRotation = Quaternion.Euler(Controller.CameraPan, Controller.CameraRotation, 0f);
        Vector3 TargetCameraPosition = TargetCameraRotation * new Vector3(0f, 0f, -Controller.CameraZoom) + Controller.PlayerCameraPivot.transform.position;

        //Find the vector direction from the camera pivot to the new target position, and the distance between these two points
        Vector3 DirectionPivotToTarget = (TargetCameraPosition - Controller.PlayerCameraPivot.transform.position).normalized;
        float DistancePivotToTarget = Vector3.Distance(TargetCameraPosition, Controller.PlayerCameraPivot.transform.position);

        //Get the size of the cameras view frustum in world units, then use those to define the dimensions we will use for our boxcast (width, height, length)
        float FrustumHeight = 2.0f * 0.3f * Mathf.Tan(Controller.CameraComponent.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float FrustumWidth = FrustumHeight * Controller.CameraComponent.aspect;
        Vector3 HalfBoxDimensions = new Vector3(FrustumWidth / 2f, FrustumHeight / 2f, 0.1f);

        //Do a BoxCast from the cameras pivot to the new target location to make sure theres no obstacles prevent the camera from moving there
        RaycastHit BoxHit;
        bool HitDetected = Physics.BoxCast(Controller.PlayerCameraPivot.transform.position, HalfBoxDimensions, DirectionPivotToTarget, out BoxHit, Quaternion.LookRotation(TargetCameraPosition - Controller.PlayerCameraPivot.transform.position), DistancePivotToTarget);

        //If no collision with any obstacles was detected we move the camera straight to its target location
        if (!HitDetected)
            Controller.CameraComponent.transform.position = TargetCameraPosition;
        //Otherwise we move it the location where the boxhit detection occured, if its not closer to the player than should be allowed
        else
            Controller.CameraComponent.transform.position = Controller.PlayerCameraPivot.transform.position + (TargetCameraPosition - Controller.PlayerCameraPivot.transform.position).normalized * (BoxHit.distance + Controller.CameraComponent.nearClipPlane);

        //Apply rotation to the camera
        Controller.CameraComponent.transform.rotation = TargetCameraRotation;
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
    private Vector3 ComputeMovementVector(bool IsCursorLocked)
    {
        //Create new X and Y movement vectors based on the current direction of the camera relative to the player and the players input (ignored while mouse cursor is not locked to the screen)
        Vector3 MovementX = IsCursorLocked ? Vector3.Cross(transform.up, Controller.CameraTransform.forward).normalized : Vector3.zero;
        Vector3 MovementY = IsCursorLocked ? Vector3.Cross(MovementX, transform.up).normalized : Vector3.zero;

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

// ================================================================================================================================
// File:        ThirdPersonLockedControlState.cs
// Description: Active while the player is in third person mode and locked onto an enemy target
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonLockedControlState : State
{
    //State Machine Controller
    private LocalPlayerController Controller;
    private void Awake() { Controller = GetComponent<LocalPlayerController>(); }

    protected override void OnStateInitialize(StateMachine Machine = null)
    {
        
    }

    protected override void OnStateEnter()
    {
        //Activate the targetting reticle
        Controller.TargettingReticleObject.SetActive(true);
    }

    protected override void OnStateExit()
    {
        //Deactivate the targetting reticle
        Controller.TargettingReticleObject.SetActive(false);
    }

    protected override void OnStateUpdate()
    {
        //Player movement is not possible while the cursor is not locked to the screen
        if (!Controller.CursorLocked)
            return;

        //Clicking middle mouse / tapping Q will release the enemy target and return back to FreeControlState
        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Q))
        {
            ReleaseTargetLock();
            return;
        }

        //Keep the targetting reticle above the current target
        PositionTargettingReticle();

        //Get a new movement vector and apply velocity to it
        Vector3 MovementVector = ComputeMovementVector();
        MovementVector.y += Controller.Velocity;

        //Apply this new movement vector to the player if its not an empty value
        if (MovementVector.x != 0f || MovementVector.z != 0f)
            Controller.Controller.Move(MovementVector * Controller.MoveSpeed * Time.deltaTime);

        //Rotate the player to face forwards toward their current target lock
        transform.rotation = ComputeMovementRotation();

        //Vector3 ElevatedTargetPosition = GetElevatedTargetPosition();
        //Vector3 DirectionCharacterToTarget = (Controller.transform.position - ElevatedTargetPosition).normalized;
        //Quaternion NewRotation = Quaternion.LookRotation(DirectionCharacterToTarget);
        //transform.rotation = NewRotation;




        ////Find where the player would be after applying this new movement vector to the controller component
        //Vector3 PositionAfterMovement = Controller.transform.position + MovementVector * Controller.MoveSpeed * Time.deltaTime;

        ////Find the enemy targets position if placed on the same elevation as the player character
        //Vector3 ElevatedTargetPosition = GetElevatedTargetPosition();

        ////Find the distance from the players current position, to both the position after movement and the elevated targets position
        //float DistanceToNewPosition = Vector3.Distance(Controller.transform.position, PositionAfterMovement);
        //float DistanceToElevatedTargetPosition = Vector3.Distance(Controller.transform.position, ElevatedTargetPosition);

        ////By comparing these distances, check if the new position after applying this movement vector would



        ////Find where the player would be after this movement, and find the enemies position if placed on the same elevation as the player
        //Vector3 NewPositionAfterMovement = transform.position + MovementVector * Controller.MoveSpeed * Time.deltaTime;
        //Vector3 ElevatedTargetPosition = GetElevatedTargetPosition();
        ////Find the distance between these two positions 


        ////Apply the new movement vector to the player and check if it ended up where we thought that it would
        //if (MovementVector.x != 0f || MovementVector.z != 0f)
        //{
        //    Controller.Controller.Move(MovementVector * Controller.MoveSpeed * Time.deltaTime);

        //    Debug.Log("Predicted player would move to: " + NewPositionAfterMovement.ToString() + ", they actually moved to: " + Controller.transform.position.ToString());
        //}

        ////Find the targets elevated position, and the distance between that and what the characters new position would be after applying the new movement vector
        //Vector3 ElevatedTargetPosition = GetElevatedTargetPosition();
        //float ElevatedTargetDistance = Vector3.Distance(NewPositionAfterMovement, ElevatedTargetPosition);

        ////Apply the new movement vector to the player if the player isnt already too close to the elevated target location
        //if (ElevatedTargetDistance > 0.1f)
        //    Controller.Controller.Move(MovementVector * Controller.MoveSpeed * Time.deltaTime);

        ////Apply rotation to the player
        //transform.rotation = ComputeMovementRotation();

        ////Apply new movement and rotation to the player
        //if (MovementVector.x != 0f || MovementVector.z != 0f)
        //{
        //    Controller.Controller.Move(MovementVector * Controller.MoveSpeed * Time.deltaTime);
        //    transform.rotation = ComputeMovementRotation();
        //}
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
        //Otherwise we process input and update the camera as normal
        else
            ApplyLockedCameraMovement();
    }

    //Applies movement to the players camera during this state
    private void ApplyLockedCameraMovement()
    {
        //Compute new target position for the camera, which is just the position of the locked camera offset object
        Vector3 TargetCameraPosition = Controller.LockedCameraOffset.transform.position;

        //Find the vector direction from the camera pivot to this new target location, and the distance between those objects
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
        Controller.CameraTransform.LookAt(Controller.PlayerEnemyTarget.transform.position);
    }

    //Releases the current target lock and returns to the FreeControlState
    private void ReleaseTargetLock()
    {
        //Clear the current target lock and transition back to the FreeControlState
        Controller.PlayerEnemyTarget = null;
        Controller.StateMachine.SetState(GetComponent<ThirdPersonFreeControlState>());
    }

    //Keeps the targetting reticle above the current lock target, method taken from "eses" reply in this unity thread https://forum.unity.com/threads/create-ui-health-markers-like-in-world-of-tanks.432935/
    private void PositionTargettingReticle()
    {
        //Offset position above the enemies head in world space
        float OffsetY = Controller.PlayerEnemyTarget.transform.position.y;

        //Compute the markers target position in world space
        Vector3 OffsetPos = new Vector3(Controller.PlayerEnemyTarget.transform.position.x, OffsetY, Controller.PlayerEnemyTarget.transform.position.z);

        //Calculate screen position
        Vector2 ScreenPoint = Controller.CameraComponent.WorldToScreenPoint(OffsetPos);

        //Convert screen position to Canvas / RectTransform space (leave camera null if using Screen Space Overlay)
        Vector2 CanvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Controller.CanvasRect, ScreenPoint, null, out CanvasPos);

        //Set the reticles screen position
        Controller.TargettingReticleRect.localPosition = CanvasPos;
    }

    //Computes a new target location for the player character based on user input and the direction to the current lock target
    private Vector3 ComputeTargetLocation()
    {
        //Find the targets elevated position and the direction from the player to that position
        Vector3 ElevatedTargetPosition = GetElevatedTargetPosition();
        Vector3 DirectionPlayerToElevatedTargetPosition = (Controller.transform.position - ElevatedTargetPosition).normalized;



        return Vector3.zero;
    }

    //Computes a new movement vector based on the users input and the direction the player is facing
    private Vector3 ComputeMovementVector()
    {
        //Compute new movement axes based on the direction the player is facing
        Vector3 MovementX = Vector3.Cross(transform.up, transform.forward).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;

        //Find a new movement vector by applying the users input to these new movement axes
        return Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;
    }

    //Computes a new rotation so the player always faces forwards towards its current target lock
    private Quaternion ComputeMovementRotation()
    {
        //Find the direction from the player to the targets elevated position
        Vector3 ElevatedTargetDirection = (GetElevatedTargetPosition() - Controller.transform.position).normalized;

        //If the target direction is the same location as the player return the current transform
        if (ElevatedTargetDirection == Vector3.zero)
            return Controller.transform.rotation;
        //Otherwise return a quaternion having the player look in the direction of the lock target
        else
            return Quaternion.LookRotation(ElevatedTargetDirection);
    }

    //Returns the current target locks position as if it was on the same elevation as the player
    private Vector3 GetElevatedTargetPosition()
    {
        Vector3 ElevatedTargetPosition = Controller.PlayerEnemyTarget.transform.position;
        ElevatedTargetPosition.y = Controller.transform.position.y;
        return ElevatedTargetPosition;
    }
}

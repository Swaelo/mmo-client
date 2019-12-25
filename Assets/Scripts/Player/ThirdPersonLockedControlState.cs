// ================================================================================================================================
// File:        ThirdPersonLockedControlState.cs
// Description: Active while the player is in third person mode and locked onto an enemy target
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

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

        //Apply new movement and rotation to the player
        Controller.Controller.Move(MovementVector * Controller.MoveSpeed * Time.deltaTime);
        transform.rotation = ComputeMovementRotation();
    }

    protected override void OnStateFixedUpdate()
    {

    }

    protected override void OnStateLateUpdate()
    {
        //Apply new override settings when instructed to by the game server
        if (Controller.NewCameraValues)
            Controller.ForceSetCameraValues();
        //Otherwise we process input and update the camera as normal
        else
        {
            //Do nothing with the camera if the cursor isnt locked
            if (!Controller.CursorLocked)
                return;

            //ApplyCameraZoomPan();
        }
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

    //Computes a new movement vector based on the users input and the current target lock
    private Vector3 ComputeMovementVector()
    {
        //Figure out the direction from the player to the current target lock
        Vector3 TargetDirection = (Controller.PlayerEnemyTarget.transform.position - transform.position).normalized;

        //Poll X/Y input axes relative to the current target lock
        Vector3 MovementX = Vector3.Cross(transform.up, TargetDirection).normalized;
        Vector3 MovementY = Vector3.Cross(MovementX, transform.up).normalized;

        //Return a new movement vector calculated using these input axes
        return Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;
    }

    //Computes a new rotation so the player faces towards its current target lock
    private Quaternion ComputeMovementRotation()
    {
        //Get the direction from the player to the current target lock
        Vector3 TargetDirection = (Controller.PlayerEnemyTarget.transform.position - transform.position).normalized;

        //Return a quaternion having the player look toward that
        Quaternion Rotation = Quaternion.LookRotation(TargetDirection);
        Vector3 Eulers = Rotation.eulerAngles;
        Eulers.x = transform.rotation.x;
        Rotation.eulerAngles = Eulers;
        return Rotation;
    }
}

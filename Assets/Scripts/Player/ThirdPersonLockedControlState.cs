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

        ApplyCameraZoomPan();
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

            ApplyCameraZoomPan();
        }
    }

    private void ApplyCameraZoomPan()
    {
        //Compare the elevation difference between the player and the new target
        float ElevationDifference = Controller.PlayerEnemyTarget.transform.position.y - Controller.PlayerCameraPivot.transform.position.y;

        //Map this onto the Pan and Zoom ranges to get the new camera values
        float NewCameraPan = MapToPanRange(ElevationDifference);
        NewCameraPan = Controller.ClampCameraAngle(NewCameraPan, Controller.CameraPanDownLimit, Controller.CameraPanUpLimit);

        float NewCameraZoom = MapToZoomRange(ElevationDifference);

        //Find the current direction from the player to the target lock
        Vector3 DirectionPlayerToTarget = (Controller.PlayerCameraPivot.transform.position - Controller.PlayerEnemyTarget.transform.position).normalized;
        DirectionPlayerToTarget.y = 0f;

        //Move the Camera Dummy around until its exactly where we want the players camera to be
        Quaternion NewCameraRotation = Quaternion.Euler(NewCameraPan, 0f, 0f);
        Vector3 NewCameraPosition = Controller.PlayerCameraPivot.transform.position + DirectionPlayerToTarget * NewCameraZoom;
        Controller.CameraDummy.transform.position = NewCameraPosition;

        //Get the distance between the CameraDummy and the CameraPivot to make sure they arent too close
        float DummyDistance = Vector3.Distance(Controller.CameraDummy.transform.position, Controller.PlayerCameraPivot.transform.position);
        Log.Chat(DummyDistance.ToString());
        //If the dummy is closer than the zoom limit then move it back a bit
        if(DummyDistance < Controller.CameraCloseZoomLimit)
        {
            //Figure out how far we need to move the dummy back so it doesnt get too close to the player
            float MoveDistance = Controller.CameraCloseZoomLimit - DummyDistance;
            //Now move it back
            Vector3 NewDummyPos = Controller.CameraDummy.transform.position - Controller.CameraDummy.transform.forward * MoveDistance;
            Debug.Log("Moving Dummy back from " + Controller.CameraDummy.transform.position + " to " + NewDummyPos + ", this is a distance of " + Vector3.Distance(Controller.CameraDummy.transform.position, NewDummyPos));
            Controller.CameraDummy.transform.position = NewDummyPos;
        }

        

        Controller.CameraDummy.transform.rotation = NewCameraRotation;
        Controller.CameraDummy.transform.LookAt(Controller.PlayerEnemyTarget.transform.position);

        //Copy the Camera Dummy values on to the players camera
        Controller.CameraTransform.position = Controller.CameraDummy.transform.position;
        Controller.CameraTransform.rotation = Controller.CameraDummy.transform.rotation;
    }

    //Maps the given elevation value onto the CameraPan range
    private float MapToPanRange(float ElevationDifference)
    {
        //Before we start mapping the ElevationDifference onto the other number lines, make sure it stays between the extreme values -5 and 5
        if (ElevationDifference > 5)
            ElevationDifference = 5;
        if (ElevationDifference < -5)
            ElevationDifference = -5f;

        //X is the value we want to map onto the Camera Pan Value Range
        float X = ElevationDifference;

        //X value between 0 and 5 maps onto the Mid-Low range
        if(X >= 0.0f)
        {
            //Find N using the Mid-Low elevation ranges
            float N = FindN(X, Controller.ElevMid, Controller.ElevLow);

            //Now map the N ratio onto the Camera Pan Mid-Low range
            return FindY(N, Controller.PanMid, Controller.PanLow);
        }
        //X Value between -5 and 0 maps onto the High-Mid range
        else
        {
            //Find N using the High-Mid elevation range
            float N = FindN(X, Controller.ElevHigh, Controller.ElevMid);

            //Now map the N ratio onto the Camrea Pan High-Mid range
            return FindY(N, Controller.PanHigh, Controller.PanMid);
        }
    }

    //Maps the given elevation value onto the CameraZoom range
    private float MapToZoomRange(float ElevationDifference)
    {
        if (ElevationDifference > 5)
            ElevationDifference = 5;
        if (ElevationDifference < -5)
            ElevationDifference = -5f;

        float X = ElevationDifference;

        if(X >= 0.0f)
        {
            float N = FindN(X, Controller.ElevMid, Controller.ElevLow);

            return FindY(N, Controller.ZoomMid, Controller.ZoomLow);
        }
        else
        {
            float N = FindN(X, Controller.ElevHigh, Controller.ElevMid);

            return FindY(N, Controller.ZoomHigh, Controller.ZoomMid);
        }
    }

    //Finds the Ratio of X along the range of Xmin-XMax
    private float FindN(float X, float XMin, float XMax)
    {
        return (X - XMin) / (XMax - XMin);
    }

    //Maps the Ratio N onto the rnage of YMin-YMax
    private float FindY(float N, float YMin, float YMax)
    {
        return N * (YMax - YMin) + YMin;
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

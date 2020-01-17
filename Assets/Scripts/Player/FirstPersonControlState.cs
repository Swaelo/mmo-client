// ================================================================================================================================
// File:        FirstPersonControlState.cs
// Description: Active while the player is in first person mode
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class FirstPersonControlState : State
{
    //State Machine Controller
    private LocalPlayerController Controller;
    private void Awake() { Controller = GetComponent<LocalPlayerController>(); }

    protected override void OnStateInitialize(StateMachine Machine = null)
    {
        
    }

    protected override void OnStateEnter()
    {
        //Reset the current camera tilt value
        Controller.FirstPersonCameraTilt = 0f;

        //Move the camera to the FPS position
        Controller.CameraTransform.position = Controller.FPSCameraPivot.transform.position;

        //Face the camera forward in the direction the player is facing
        Vector3 FacePosition = Controller.CameraTransform.position + Controller.transform.forward;
        Controller.CameraTransform.LookAt(FacePosition);
    }

    protected override void OnStateExit()
    {
        
    }

    protected override void OnStateUpdate()
    {
        //Apply movement to the character
        Vector3 MovementVector = ComputeMovementVector();
        MovementVector.y += Controller.Velocity;
        Controller.Controller.Move(MovementVector * Controller.MoveSpeed * Time.deltaTime);

        //Rotate the character left/right with horizontal mouse movement
        transform.RotateAround(Vector3.up, Input.GetAxis("Mouse X") * Controller.FirstPersonTurnSpeed * Time.deltaTime);
    }

    protected override void OnStateFixedUpdate()
    {

    }

    protected override void OnStateLateUpdate()
    {
        //Camera movement is not possible while the cursor is not locked to the screen
        if (!Controller.CursorLocked)
            return;

        //Get new camera tilt values based on users mouse movement, make sure it stays clamped within accepted values
        float NewCameraTilt = ClampTiltValue(Controller.FirstPersonCameraTilt + Input.GetAxis("Mouse Y") * Controller.FirstPersonTiltSpeed * Controller.MouseDampening);


        ////Pivot the camera up/down with vertical mouse movement
        //float CameraTiltAdjustment = Input.GetAxis("Mouse Y") * Controller.FirstPersonTiltSpeed * Controller.MouseDampening;
        //float NewCameraTilt = Controller.FirstPersonCameraTilt + CameraTiltAdjustment;
        //if (NewCameraTilt >= Controller.FirstPersonMinCameraTilt && NewCameraTilt <= Controller.FirstPersonMaxCameraTilt)
        //{
        //    //Apply the new rotation values onto the camera
        //    Controller.CameraTransform.position = Controller.FPSCameraPivot.transform.position;
        //    Controller.CameraTransform.LookAt(Controller.CameraTransform.position + Controller.transform.forward);
        //    Controller.CameraTransform.Rotate(-Controller.CameraTransform.right, NewCameraTilt);

        //    ////Use the camera dummy to get the perfect position/rotation for the new camera values
        //    //Controller.CameraDummy.transform.position = Controller.FPSCameraPivot.transform.position;
        //    //Controller.CameraDummy.transform.LookAt(Controller.CameraDummy.transform.position + Controller.transform.forward);
        //    //Controller.CameraDummy.transform.Rotate(-Controller.CameraDummy.transform.right, NewCameraTilt);



        //    ////Apply the new tilt adjustment if its within the accepted values
        //    //Controller.CameraTransform.RotateAround(-Controller.CameraTransform.right, CameraTiltAdjustment * Time.deltaTime);
        //    //Controller.FirstPersonCameraTilt = NewCameraTilt;
        //}

        //Check for user scrolling the mousewheel back out to return to the third person mode
        float CameraZoom = Input.GetAxis("Mouse ScrollWheel");
        if(CameraZoom < 0.0f)
        {
            Controller.StateMachine.SetState(GetComponent<ThirdPersonFreeControlState>());
            return;
        }
    }

    //Returns tilt value kept within accepted parameters
    private float ClampTiltValue(float CurrentValue)
    {
        //Stop value exceeding min/max values, then return it
        float ClampedValue = CurrentValue;
        if (ClampedValue > Controller.FirstPersonMaxCameraTilt)
            ClampedValue = Controller.FirstPersonMaxCameraTilt;
        if (ClampedValue < Controller.FirstPersonMinCameraTilt)
            ClampedValue = Controller.FirstPersonMinCameraTilt;
        return ClampedValue;
    }

    //Computes a new movement vector based on the users input
    private Vector3 ComputeMovementVector()
    {
        Vector3 MovementX = Controller.CursorLocked ? Vector3.Cross(transform.up, transform.forward).normalized : Vector3.zero;
        Vector3 MovementY = Controller.CursorLocked ? Vector3.Cross(MovementX, transform.up).normalized : Vector3.zero;
        return Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;
    }
}

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
        //Move the camera to the FPS position and parent it to the player when entering this state
        Controller.CameraTransform.position = Controller.FPSCameraPivot.transform.position;
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
        //Pivot the camera up/down with vertical mouse movement
        float CameraTiltAdjustment = Input.GetAxis("Mouse Y") * Controller.FirstPersonTiltSpeed * Controller.MouseDampening;
        float NewCameraTilt = Controller.FirstPersonCameraTilt + CameraTiltAdjustment;
        if (NewCameraTilt >= Controller.FirstPersonMinCameraTilt && NewCameraTilt <= Controller.FirstPersonMaxCameraTilt)
        {
            //Apply the new tilt adjustment if its within the accepted values
            Controller.CameraTransform.RotateAround(-Controller.CameraTransform.right, CameraTiltAdjustment * Time.deltaTime);
            Controller.FirstPersonCameraTilt = NewCameraTilt;
        }

        //Check for user scrolling the mousewheel back out to return to the third person mode
        float CameraZoom = Input.GetAxis("Mouse ScrollWheel");
        if(CameraZoom < 0.0f)
        {
            Controller.StateMachine.SetState(GetComponent<ThirdPersonFreeControlState>());
            return;
        }
    }

    //Computes a new movement vector based on the users input
    private Vector3 ComputeMovementVector()
    {
        Vector3 MovementX = Controller.CursorLocked ? Vector3.Cross(transform.up, transform.forward).normalized : Vector3.zero;
        Vector3 MovementY = Controller.CursorLocked ? Vector3.Cross(MovementX, transform.up).normalized : Vector3.zero;
        return Input.GetAxis("Horizontal") * MovementX + Input.GetAxis("Vertical") * MovementY;
    }
}

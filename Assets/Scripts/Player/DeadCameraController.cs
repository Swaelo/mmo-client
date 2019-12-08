// ================================================================================================================================
// File:        DeadCameraController.cs
// Description:	Camera just looks at and rotates around the players corpse until they choose to respawn
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadCameraController : MonoBehaviour
{
    public float CameraDistance = 3.5f; //How far the camera should stay away from the players corpse
    public float RotationSpeed = 30f;   //How fast the camera rotates around the players body
    private float CurrentXRotation = 0f;    //Camera constantly rotates around the players body, the current rotation amount
    public GameObject PivotTarget = null;   //The players corpse object to rotate around

    private void Start()
    {
        //Store the initial rotation values
        CurrentXRotation = transform.eulerAngles.x;
    }

    private void LateUpdate()
    {
        //Do nothing without a pivot target
        if (PivotTarget == null)
            return;

        //Keep rotating the camera around the players body
        CurrentXRotation += RotationSpeed * Time.deltaTime;
        if (CurrentXRotation < -360f)
            CurrentXRotation += 360f;
        if (CurrentXRotation > 360f)
            CurrentXRotation -= 360f;

        //Find and apply a new target position and rotation for the camera
        Quaternion TargetRotation = Quaternion.Euler(0f, CurrentXRotation, 0f);
        Vector3 TargetPosition = TargetRotation * new Vector3(0f, 0f, -CameraDistance) + PivotTarget.transform.position;
        TargetPosition.y += 2.5f;

        //Apply the new position and rotation values
        transform.position = TargetPosition;
        transform.rotation = TargetRotation;

        //and face the camera towards the players corpse
        transform.LookAt(PivotTarget.transform);
    }

    //Display a Respawn button on the UI whenever the player is dead
    private void OnGUI()
    {
        if (GUI.Button(new Rect(430, 10, 100, 50), "Respawn"))
            CombatPacketSender.Instance.SendPlayerRespawnRequest();
    }
}

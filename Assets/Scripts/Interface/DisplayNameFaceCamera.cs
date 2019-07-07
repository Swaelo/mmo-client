// ================================================================================================================================
// File:        DisplayNameFaceCamera.cs
// Description:	Keeps remote players display names facing toward the current scene camera
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class DisplayNameFaceCamera : MonoBehaviour
{
    void Update()
    {
        //Get the current scene camera
        GameObject PlayerCamera = GameObject.Find("Player Camera");

        //Face the display name towards this if the player camera was found
        if(PlayerCamera != null)
            transform.rotation = Quaternion.LookRotation(transform.position - PlayerCamera.transform.position);
    }
}

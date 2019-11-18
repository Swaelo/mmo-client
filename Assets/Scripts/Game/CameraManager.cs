// ================================================================================================================================
// File:        CameraManager.cs
// Description:	Manages the enabling/disabling of various cameras in the scene and gives easy access to all of them
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //Singleton instance
    public static CameraManager Instance = null;
    void Awake() { Instance = this; }

    //Camera references
    public GameObject MainCamera;

    public void ToggleMainCamera(bool ActiveStatus)
    {
        MainCamera.SetActive(ActiveStatus);
    }
}

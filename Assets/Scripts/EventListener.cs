// ================================================================================================================================
// File:        EventListener.cs
// Description:	Handles events sent from the webpage javascript to toggle cursors lockstate and windows fullscreen mode
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class EventListener : MonoBehaviour
{
    public LocalPlayerController PlayerController;

    public void LockCursor()
    {
        PlayerController.SetCursorLockState(true);
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void FreeCursor()
    {
        PlayerController.SetCursorLockState(false);
    }
}

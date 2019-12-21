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
        Log.Chat("cursor locked");
        PlayerController.SetCursorLockState(true);
    }

    public void FreeCursor()
    {
        Log.Chat("cursor unlocked");
        PlayerController.SetCursorLockState(false);
    }

    public void EnableFull()
    {
        Log.Chat("fullscreen enabled");
    }

    public void DisableFull()
    {
        Log.Chat("fullscreen disabled");
    }
}

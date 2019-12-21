// ================================================================================================================================
// File:        VisibilityTracker.cs
// Description:	Keeps track of when an object is and isnt visible by the players camera
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityTracker : MonoBehaviour
{
    public bool IsVisible = false;

    private void OnBecameInvisible()
    {
        IsVisible = false;
    }

    private void OnBecameVisible()
    {
        IsVisible = true;
    }
}

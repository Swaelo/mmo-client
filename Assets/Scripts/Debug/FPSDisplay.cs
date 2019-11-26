// ================================================================================================================================
// File:        FPSDisplay.cs
// Description:	Displays the current framerate to the UI
// Author:	    Unity 3D https://wiki.unity3d.com/index.php/FramesPerSecond
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    private float DeltaTime = 0.0f;
    public Text UIText;

    private void Update()
    {
        DeltaTime += (Time.unscaledDeltaTime - DeltaTime) * 0.1f;
        float FPS = 1.0f / DeltaTime;
        UIText.text = "FPS: " + FPS;
    }
}

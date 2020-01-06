// ================================================================================================================================
// File:        LoadedAlertCaller.cs
// Description:	
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using System.Runtime.InteropServices;

public class LoadedAlertCaller : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void AlertGameLoaded();

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        AlertGameLoaded();
#endif
    }
}

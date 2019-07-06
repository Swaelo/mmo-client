// ================================================================================================================================
// File:        PlayerPrefabs.cs
// Description:	
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefabs : MonoBehaviour
{
    //Singleton Instance
    public static PlayerPrefabs Instance = null;
    void Awake() { Instance = this; }

    public GameObject LocalPlayerPrefab;
    public GameObject RemotePlayerPrefab;
}

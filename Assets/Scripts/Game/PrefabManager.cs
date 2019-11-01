// ================================================================================================================================
// File:        PrefabManager.cs
// Description:	Used to quickly and easily spawn prefab objects into the game world
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    //Singleton instance for quick and easy global method access
    public static PrefabManager Instance = null;
    void Awake() { Instance = this; }

    //Prefab objects that can be used
    public GameObject LocalPlayerPrefab;
    public GameObject RemotePlayerPrefab;
}

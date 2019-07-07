// ================================================================================================================================
// File:        RemotePlayerHandler.cs
// Description:	Manages spawning and updating position of other players characters inside the game world
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerHandler : MonoBehaviour
{
    //Singleton Instance
    public static RemotePlayerHandler Instance = null;
    void Awake() { Instance = this; }

    //Current list of all the other remote players active inside the game world, each mapped to their characters name
    public Dictionary<string, GameObject> RemotePlayers = new Dictionary<string, GameObject>();

    //Adds a new remote player into the game world at the specified location with the given name
    public void AddRemotePlayer(string PlayerName, Vector3 PlayerLocation)
    {
        //Fetch the prefab manager that will be used to instantiate a new remote player prefab into the game
        PlayerPrefabs PrefabManager = GameObject.Find("Prefab Manager").GetComponent<PlayerPrefabs>();
        //Spawn a new remote player prefab into the game world at the given location
        GameObject NewRemotePlayer = GameObject.Instantiate(PrefabManager.RemotePlayerPrefab, PlayerLocation, Quaternion.identity);
        //Assign the players name to be displayed above their head
        NewRemotePlayer.GetComponent<RemotePlayerController>().AssignName(PlayerName);
        //Map this new remote player into the dictionary by its player name
        RemotePlayers.Add(PlayerName, NewRemotePlayer);
    }

    //Removes an already existing remote player from the game world
    public void RemoveRemotePlayer(string PlayerName)
    {
        //Fetch this remote players GameObject
        GameObject RemotePlayer = RemotePlayers[PlayerName];
        //Destroy the remote players game object
        GameObject.Destroy(RemotePlayer);
        //Remote them from the list of remote players
        RemotePlayers.Remove(PlayerName);
    }

    //Moves an already existing remote player to its new updated position
    public void UpdatePlayerPosition(string PlayerName, Vector3 PlayerLocation)
    {
        //Fetch the remote players GameObject from the dictionary thats going to be updated
        GameObject RemotePlayer = RemotePlayers[PlayerName];
        //Give the remote player this new target position for it to move towards
        RemotePlayer.GetComponent<RemotePlayerController>().TargetPosition = PlayerLocation;
    }
}

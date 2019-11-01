// ================================================================================================================================
// File:        PlayerManager.cs
// Description:	Manages the spawning, removing and updating of the local players character, and all remote players characters within the game world
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Singleton instance for easy global access
    public static PlayerManager Instance = null;
    void Awake() { Instance = this; }

    //Reference to the local players ingame character, and dictionary of other players character mapped by their character name
    private GameObject LocalPlayer = null;
    public Dictionary<string, GameObject> RemotePlayers = new Dictionary<string, GameObject>();

    /// <summary>
    /// Spawns someone elses player character into our game world
    /// </summary>
    /// <param name="PlayerName">The name of the character being added</param>
    /// <param name="PlayerLocation">The characters initial starting location</param>
    public void AddRemotePlayer(string PlayerName, Vector3 PlayerLocation)
    {
        //Spawn a new remote player object into the game world using the prefab manager
        GameObject RemotePlayer = GameObject.Instantiate(PrefabManager.Instance.RemotePlayerPrefab, PlayerLocation, Quaternion.identity);
        //Update the character name displayed above the new characters head
        RemotePlayer.GetComponent<RemotePlayerController>().AssignName(PlayerName);
        //Map this new player into the dictionary by its player name
        RemotePlayers.Add(PlayerName, RemotePlayer);
    }

    /// <summary>
    /// Spawns a local player prefab into the game world
    /// </summary>
    /// <param name="PlayerLocation">World Location where the player will be spawned at</param>
    public void AddLocalPlayer(Vector3 PlayerLocation)
    {
        LocalPlayer = GameObject.Instantiate(PrefabManager.Instance.LocalPlayerPrefab, PlayerLocation, Quaternion.identity);
    }

    /// <summary>
    /// Removes a remote player from the dictionary and destroys it
    /// </summary>
    /// <param name="PlayerName">Name of player to be removed</param>
    public void RemoveRemotePlayer(string PlayerName)
    {
        GameObject.Destroy(RemotePlayers[PlayerName]);
        RemotePlayers.Remove(PlayerName);        
    }

    /// <summary>
    /// Removes the current local player object from the game world
    /// </summary>
    public void RemoveLocalPlayer()
    {
        Debug.Log("remove local player");
        GameObject.Destroy(LocalPlayer);
    }

    /// <summary>
    /// Removes all existing remote player characters from the game world
    /// </summary>
    public void RemoveAllRemotePlayers()
    {
        Debug.Log("remove all remote players");

        //Place all of the remote player objects into a new list
        List<GameObject> RemotePlayerObjects = new List<GameObject>();
        foreach (KeyValuePair<string, GameObject> RemotePlayer in RemotePlayers)
            RemotePlayerObjects.Add(RemotePlayer.Value);
        //Loop through and destroy all the objects
        foreach (GameObject RemotePlayer in RemotePlayerObjects)
            GameObject.Destroy(RemotePlayer);
        //Empty the dictionary list
        RemotePlayers.Clear();
    }

    /// <summary>
    /// Updates a remote player with its new target location
    /// </summary>
    /// <param name="PlayerName">Name of players position to update</param>
    /// <param name="NewLocation">New target location for the player</param>
    public void UpdateRemotePlayerLocation(string PlayerName, Vector3 NewLocation)
    {
        //Pass the NewLocation value into the RemotePlayerController object
        RemotePlayers[PlayerName].GetComponent<RemotePlayerController>().TargetPosition = NewLocation;
    }
}
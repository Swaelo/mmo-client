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
    public GameObject LocalPlayer = null;
    public Dictionary<string, GameObject> RemotePlayers = new Dictionary<string, GameObject>();

    /// <summary>
    /// Spawns someone elses player character into our game world
    /// </summary>
    /// <param name="PlayerName">The name of the character being added</param>
    /// <param name="PlayerLocation">The characters initial starting location</param>
    public void AddRemotePlayer(string PlayerName, Vector3 PlayerLocation, Vector3 PlayerMovement, Quaternion PlayerRotation)
    {
        //Requests to add players we already know about should be ignored
        if (RemotePlayers.ContainsKey(PlayerName))
        {
            Log.Chat("Ignoring request to add already known player " + PlayerName);
            return;
        }

        //Spawn a new remote player object into the game world using the prefab manager, make sure they are set at the right starting location
        GameObject RemotePlayer = GameObject.Instantiate(PrefabManager.Instance.RemotePlayerPrefab, PlayerLocation, PlayerRotation);
        RemotePlayerController RemoteController = RemotePlayer.GetComponent<RemotePlayerController>();
        RemoteController.UpdateValues(PlayerLocation, PlayerMovement, PlayerRotation);

        //Update the character name displayed above the new characters head
        TextMesh PlayerNameDisplay = RemotePlayer.transform.Find("Character Name").GetComponent<TextMesh>();
        PlayerNameDisplay.text = PlayerName;
        //Map this new player into the dictionary by its player name
        RemotePlayers.Add(PlayerName, RemotePlayer);
    }

    /// <summary>
    /// Removes a remote player from the dictionary and destroys it
    /// </summary>
    /// <param name="PlayerName">Name of player to be removed</param>
    public void RemoveRemotePlayer(string PlayerName)
    {
        //Make sure we know about a character before we try to destroy it, if we dont know about it then it should be ignored
        if (!RemotePlayers.ContainsKey(PlayerName))
        {
            Log.Chat("Ignoring request to remove unknown player: " + PlayerName);
            return;
        }

        GameObject.Destroy(RemotePlayers[PlayerName]);
        RemotePlayers.Remove(PlayerName);
    }

    /// <summary>
    /// Function for updating a remote players values
    /// </summary>
    /// <param name="Name">Name of the remote player we are updating</param>
    /// <param name="Location">The remote players new location</param>
    /// <param name="Movement">The remote players new movement input</param>
    /// <param name="Rotation">The remote players new rotation</param>
    public void UpdateRemotePlayer(string Name, Vector3 Location, Vector3 Movement, Quaternion Rotation)
    {
        //Make sure this player exists in our game world before we try updating them
        if (RemotePlayers.ContainsKey(Name))
        {
            Log.Chat("Updating Remote Player: " + Name + " with new values.");
            RemotePlayers[Name].GetComponent<RemotePlayerController>().UpdateValues(Location, Movement, Rotation);
        }
        //If we dont have this player in our world yet then we need to have them added now
        else
        {
            Log.Chat("Adding Unknown Remote Player: " + Name + " to our game.");
            AddRemotePlayer(Name, Location, Movement, Rotation);
        }
    }

    /// <summary>
    /// Spawns a local player prefab into the game world
    /// </summary>
    /// <param name="PlayerLocation">World Location where the player will be spawned at</param>
    public void AddLocalPlayer(Vector3 PlayerLocation, Quaternion PlayerRotation)
    {
        LocalPlayer = GameObject.Instantiate(PrefabManager.Instance.LocalPlayerPrefab, PlayerLocation, PlayerRotation);
    }

    /// <summary>
    /// Removes the current local player object from the game world
    /// </summary>
    public void RemoveLocalPlayer()
    {
        GameObject.Destroy(LocalPlayer);
        LocalPlayer = null;
    }

    /// <summary>
    /// Updates our local player object with a new position
    /// </summary>
    /// <param name="Location">New position to apply to the local player character</param>
    public void UpdateLocalPlayer(Vector3 Location)
    {
        LocalPlayer.transform.position = Location;
    }
}
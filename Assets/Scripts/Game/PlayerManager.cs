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

    //Reference to player corpse and corpse camera
    public GameObject LocalPlayerCorpse = null;
    public GameObject LocalPlayerCorpseCamera = null;
    public Dictionary<string, GameObject> RemotePlayerCorpses = new Dictionary<string, GameObject>();

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

    //Updates the UI with the local players new health value
    public void DamageLocalPlayer(int NewHPValue)
    {
        LocalPlayer.GetComponent<PlayerHealthBar>().AdjustCurrentHealth(NewHPValue);
    }

    //Updates the text above a remote players head with their new health value
    public void DamageRemotePlayer(string PlayerName, int NewHPValue)
    {
        //Requests to damage players we dont know about should be ignored
        if(!RemotePlayers.ContainsKey(PlayerName))
        {
            Log.Chat("Ignoring request to damage player " + PlayerName + " we dont know about.");
            return;
        }

        //Update the remote players health value
        RemotePlayers[PlayerName].GetComponent<RemotePlayerController>().UpdateHealth(NewHPValue);
    }

    //Adds a local player character back into the game world
    public void RespawnLocalPlayer(Vector3 Position, Quaternion Rotation, float CameraZoom, float CameraX, float CameraY)
    {
        //Delete the player ragdoll and dead player camera
        GameObject.Destroy(LocalPlayerCorpse);
        GameObject.Destroy(LocalPlayerCorpseCamera);

        //Re-enable the local player character and set their values to what has been provided
        LocalPlayer.SetActive(true);
        LocalPlayer.GetComponent<PlayerCharacterController>().SetPlayer(Position, Rotation);
        LocalPlayer.transform.Find("Player Camera").GetComponent<PlayerCameraController>().SetCamera(CameraZoom, CameraX, CameraY);
    }

    //Adds some other remote player character back into the game world
    public void RespawnRemotePlayer(string CharacterName, Vector3 Position, Quaternion Rotation)
    {
        //Ignore requests to respawn unknown players
        if(!RemotePlayers.ContainsKey(CharacterName))
        {
            Log.Chat("Ignoring request to respawn unknown remote player " + CharacterName);
            return;
        }

        //Delete the remote players ragdoll object
        GameObject.Destroy(RemotePlayerCorpses[CharacterName]);
        RemotePlayerCorpses.Remove(CharacterName);

        //Reenable and reposition their player character object
        GameObject RemotePlayerCharacter = RemotePlayers[CharacterName];
        RemotePlayerCharacter.SetActive(true);
        RemotePlayerCharacter.transform.position = Position;
        RemotePlayerCharacter.transform.rotation = Rotation;
        RemotePlayerCharacter.GetComponent<RemotePlayerController>().UpdateValues(Position, Vector3.zero, Rotation, 10, 10);
    }

    //Disables all controls of the local player character and turns them into a ragdoll now that they are dead
    public void KillLocalPlayer()
    {
        //Store the player character/camera current position and rotation values
        Vector3 PlayerPosition = LocalPlayer.transform.position;
        Quaternion PlayerRotation = LocalPlayer.transform.rotation;
        Vector3 CameraPosition = LocalPlayer.transform.Find("Player Camera").transform.position;
        Quaternion CameraRotation = LocalPlayer.transform.Find("Player Camera").transform.rotation;

        //Also store all the same values for every bone that is used by the ragdoll
        Dictionary<string, Vector3> BonePositions = new Dictionary<string, Vector3>();
        Dictionary<string, Quaternion> BoneRotations = new Dictionary<string, Quaternion>();
        BonePositions.Add("Pelvis", LocalPlayer.transform.Find("Motion/B_Pelvis").transform.position);
        BoneRotations.Add("Pelvis", LocalPlayer.transform.Find("Motion/B_Pelvis").transform.rotation);
        BonePositions.Add("Left Hips", LocalPlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh").transform.position);
        BoneRotations.Add("Left Hips", LocalPlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh").transform.rotation);
        BonePositions.Add("Left Knee", LocalPlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf").transform.position);
        BoneRotations.Add("Left Knee", LocalPlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf").transform.rotation);
        BonePositions.Add("Left Foot", LocalPlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf/B_L_Foot").transform.position);
        BoneRotations.Add("Left Foot", LocalPlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf/B_L_Foot").transform.rotation);
        BonePositions.Add("Right Hips", LocalPlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh").transform.position);
        BoneRotations.Add("Right Hips", LocalPlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh").transform.rotation);
        BonePositions.Add("Right Knee", LocalPlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf").transform.position);
        BoneRotations.Add("Right Knee", LocalPlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf").transform.rotation);
        BonePositions.Add("Right Foot", LocalPlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf/B_R_Foot").transform.position);
        BoneRotations.Add("Right Foot", LocalPlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf/B_R_Foot").transform.rotation);
        BonePositions.Add("Left Arm", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm").transform.position);
        BoneRotations.Add("Left Arm", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm").transform.rotation);
        BonePositions.Add("Left Elbow", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm/B_L_Forearm").transform.position);
        BoneRotations.Add("Left Elbow", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm/B_L_Forearm").transform.rotation);
        BonePositions.Add("Right Arm", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm").transform.position);
        BoneRotations.Add("Right Arm", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm").transform.rotation);
        BonePositions.Add("Right Elbow", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm").transform.position);
        BoneRotations.Add("Right Elbow", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm").transform.rotation);
        BonePositions.Add("Middle Spine", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1").transform.position);
        BoneRotations.Add("Middle Spine", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1").transform.rotation);
        BonePositions.Add("Head", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_Neck/B_Head").transform.position);
        BoneRotations.Add("Head", LocalPlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_Neck/B_Head").transform.rotation);

        //Disable the local players character and camera
        LocalPlayer.SetActive(false);

        //Spawn in a ragdoll matching the characters position/rotaion values
        LocalPlayerCorpse = GameObject.Instantiate(PrefabManager.Instance.LocalPlayerRagdollPrefab, PlayerPosition, PlayerRotation);
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis").transform.position = BonePositions["Pelvis"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis").transform.rotation = BoneRotations["Pelvis"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh").transform.position = BonePositions["Left Hips"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh").transform.rotation = BoneRotations["Left Hips"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf").transform.position = BonePositions["Left Knee"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf").transform.rotation = BoneRotations["Left Knee"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf/B_L_Foot").transform.position = BonePositions["Left Foot"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf/B_L_Foot").transform.rotation = BoneRotations["Left Foot"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh").transform.position = BonePositions["Right Hips"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh").transform.rotation = BoneRotations["Right Hips"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf").transform.position = BonePositions["Right Knee"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf").transform.rotation = BoneRotations["Right Knee"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf/B_R_Foot").transform.position = BonePositions["Right Foot"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf/B_R_Foot").transform.rotation = BoneRotations["Right Foot"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm").transform.position = BonePositions["Left Arm"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm").transform.rotation = BoneRotations["Left Arm"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm/B_L_Forearm").transform.position = BonePositions["Left Elbow"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm/B_L_Forearm").transform.rotation = BoneRotations["Left Elbow"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm").transform.position = BonePositions["Right Arm"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm").transform.rotation = BoneRotations["Right Arm"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm").transform.position = BonePositions["Right Elbow"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm").transform.rotation = BoneRotations["Right Elbow"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1").transform.position = BonePositions["Middle Spine"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1").transform.rotation = BoneRotations["Middle Spine"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_Neck/B_Head").transform.position = BonePositions["Head"];
        LocalPlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_Neck/B_Head").transform.rotation = BoneRotations["Head"];

        //Spawn in a dead player camera at the cameras previous location, and tell it to follow around the ragdoll
        LocalPlayerCorpseCamera = GameObject.Instantiate(PrefabManager.Instance.DeadPlayerCamera, CameraPosition, CameraRotation);
        LocalPlayerCorpseCamera.GetComponent<DeadCameraController>().PivotTarget = LocalPlayerCorpse;
    }

    //Disables all controls of some other remote player character and turns them into a ragdoll
    public void KillRemotePlayer(string PlayerName)
    {
        //Ignore requests to kill remote players we dont know about
        if(!RemotePlayers.ContainsKey(PlayerName))
        {
            Log.Chat("Ignoring request to kill unknown remote player: " + PlayerName);
            return;
        }

        //Store the remote players current postion/rotation values
        GameObject RemotePlayer = RemotePlayers[PlayerName];
        Vector3 PlayerPosition = RemotePlayer.transform.position;
        Quaternion PlayerRotation = RemotePlayer.transform.rotation;

        //Also store all the same values for every bone used by the ragdoll
        Dictionary<string, Vector3> BonePositions = new Dictionary<string, Vector3>();
        Dictionary<string, Quaternion> BoneRotations = new Dictionary<string, Quaternion>();
        BonePositions.Add("Pelvis", RemotePlayer.transform.Find("Motion/B_Pelvis").transform.position);
        BoneRotations.Add("Pelvis", RemotePlayer.transform.Find("Motion/B_Pelvis").transform.rotation);
        BonePositions.Add("Left Hips", RemotePlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh").transform.position);
        BoneRotations.Add("Left Hips", RemotePlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh").transform.rotation);
        BonePositions.Add("Left Knee", RemotePlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf").transform.position);
        BoneRotations.Add("Left Knee", RemotePlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf").transform.rotation);
        BonePositions.Add("Left Foot", RemotePlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf/B_L_Foot").transform.position);
        BoneRotations.Add("Left Foot", RemotePlayer.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf/B_L_Foot").transform.rotation);
        BonePositions.Add("Right Hips", RemotePlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh").transform.position);
        BoneRotations.Add("Right Hips", RemotePlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh").transform.rotation);
        BonePositions.Add("Right Knee", RemotePlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf").transform.position);
        BoneRotations.Add("Right Knee", RemotePlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf").transform.rotation);
        BonePositions.Add("Right Foot", RemotePlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf/B_R_Foot").transform.position);
        BoneRotations.Add("Right Foot", RemotePlayer.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf/B_R_Foot").transform.rotation);
        BonePositions.Add("Left Arm", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm").transform.position);
        BoneRotations.Add("Left Arm", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm").transform.rotation);
        BonePositions.Add("Left Elbow", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm/B_L_Forearm").transform.position);
        BoneRotations.Add("Left Elbow", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm/B_L_Forearm").transform.rotation);
        BonePositions.Add("Right Arm", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm").transform.position);
        BoneRotations.Add("Right Arm", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm").transform.rotation);
        BonePositions.Add("Right Elbow", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm").transform.position);
        BoneRotations.Add("Right Elbow", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm").transform.rotation);
        BonePositions.Add("Middle Spine", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1").transform.position);
        BoneRotations.Add("Middle Spine", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1").transform.rotation);
        BonePositions.Add("Head", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_Neck/B_Head").transform.position);
        BoneRotations.Add("Head", RemotePlayer.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_Neck/B_Head").transform.rotation);

        //Disable the remote players character
        RemotePlayer.SetActive(false);

        //Spawn in a ragdoll matching the characters position/rotation/pose
        GameObject RemotePlayerCorpse = GameObject.Instantiate(PrefabManager.Instance.RemotePlayerRagdollPrefab, PlayerPosition, PlayerRotation);
        RemotePlayerCorpses.Add(PlayerName, RemotePlayerCorpse);
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis").transform.position = BonePositions["Pelvis"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis").transform.rotation = BoneRotations["Pelvis"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh").transform.position = BonePositions["Left Hips"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh").transform.rotation = BoneRotations["Left Hips"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf").transform.position = BonePositions["Left Knee"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf").transform.rotation = BoneRotations["Left Knee"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf/B_L_Foot").transform.position = BonePositions["Left Foot"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_L_Thigh/B_L_Calf/B_L_Foot").transform.rotation = BoneRotations["Left Foot"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh").transform.position = BonePositions["Right Hips"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh").transform.rotation = BoneRotations["Right Hips"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf").transform.position = BonePositions["Right Knee"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf").transform.rotation = BoneRotations["Right Knee"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf/B_R_Foot").transform.position = BonePositions["Right Foot"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_R_Thigh/B_R_Calf/B_R_Foot").transform.rotation = BoneRotations["Right Foot"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm").transform.position = BonePositions["Left Arm"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm").transform.rotation = BoneRotations["Left Arm"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm/B_L_Forearm").transform.position = BonePositions["Left Elbow"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_L_Clavicle/B_L_UpperArm/B_L_Forearm").transform.rotation = BoneRotations["Left Elbow"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm").transform.position = BonePositions["Right Arm"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm").transform.rotation = BoneRotations["Right Arm"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm").transform.position = BonePositions["Right Elbow"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm").transform.rotation = BoneRotations["Right Elbow"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1").transform.position = BonePositions["Middle Spine"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1").transform.rotation = BoneRotations["Middle Spine"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_Neck/B_Head").transform.position = BonePositions["Head"];
        RemotePlayerCorpse.transform.Find("Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_Neck/B_Head").transform.rotation = BoneRotations["Head"];
    }

    /// <summary>
    /// Spawns someone elses player character into our game world
    /// </summary>
    /// <param name="PlayerName">The name of the character being added</param>
    /// <param name="PlayerLocation">The characters initial starting location</param>
    public void AddRemotePlayer(string PlayerName, bool CharacterAlive, Vector3 PlayerLocation, Vector3 PlayerMovement, Quaternion PlayerRotation, int CurrentHP, int MaxHP)
    {
        //Requests to add players we already know about should be ignored
        if (RemotePlayers.ContainsKey(PlayerName))
        {
            Log.Chat("Ignoring request to add already known player " + PlayerName, true);
            return;
        }

        //Spawn a new remote player object into the game world using the prefab manager, make sure they are set at the right starting location
        GameObject RemotePlayer = GameObject.Instantiate(PrefabManager.Instance.RemotePlayerPrefab, PlayerLocation, PlayerRotation);
        RemotePlayerController RemoteController = RemotePlayer.GetComponent<RemotePlayerController>();
        RemoteController.UpdateValues(PlayerLocation, PlayerMovement, PlayerRotation, CurrentHP, MaxHP);

        //Update the character name displayed above the new characters head
        TextMesh PlayerNameDisplay = RemotePlayer.transform.Find("Character Name").GetComponent<TextMesh>();
        PlayerNameDisplay.text = PlayerName;
        //Map this new player into the dictionary by its player name
        RemotePlayers.Add(PlayerName, RemotePlayer);

        //If the remote player is not alive they need to have their character immediately disabled and replaced with a ragdoll
        if (!CharacterAlive)
            KillRemotePlayer(PlayerName);
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
    public void UpdateRemotePlayer(string Name, Vector3 Location, Vector3 Movement, Quaternion Rotation, int CurrentHealth, int MaxHealth)
    {
        //Make sure this player exists in our game world before we try updating them
        if (RemotePlayers.ContainsKey(Name))
            RemotePlayers[Name].GetComponent<RemotePlayerController>().UpdateValues(Location, Movement, Rotation, CurrentHealth, MaxHealth);
        //If we dont have this player in our world yet then we need to have them added now
        else
        {
            Log.Chat("Ignoring request to update unknown player: " + Name);
            return;
        }
    }

    public void RemotePlayerPlayAnimation(string CharacterName, string AnimationName)
    {
        //Make sure this player exists in our game world before we try triggering their animation playback
        if (RemotePlayers.ContainsKey(CharacterName))
            RemotePlayers[CharacterName].GetComponent<Animator>().SetTrigger(AnimationName);
    }
}
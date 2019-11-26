// ================================================================================================================================
// File:        PlayerManagementPacketHandler.cs
// Description:	Handles packets from the server with stuff like updated position values for other ingame player characters
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerManagementPacketHandler : MonoBehaviour
{
    //Singleton Instance
    public static PlayerManagementPacketHandler Instance = null;
    void Awake() { Instance = this; }

    //Functions for handling other player characters new Position/Rotation/Movement values
    public static void HandlePlayerPositionUpdate(ref NetworkPacket Packet)
    {
        //Log what we are doing
        Log.In("Remote Player Position Update");
        //Extract all the data from the network packet
        string CharacterName = Packet.ReadString();
        Vector3 CharacterPosition = Packet.ReadVector3();
        //Pass these values to the player handler for updating the remote player
        PlayerManager.Instance.UpdateRemotePlayerPosition(CharacterName, CharacterPosition);
    }
    public static void HandlePlayerRotationUpdate(ref NetworkPacket Packet)
    {
        //Log what we are doing
        Log.In("Remote Player Rotation Update");
        //Extract all the data from the network packet
        string CharacterName = Packet.ReadString();
        Quaternion CharacterRotation = Packet.ReadQuaternion();
        //Pass these values to the player handler for updating the remote player
        PlayerManager.Instance.UpdateRemotePlayerRotation(CharacterName, CharacterRotation);
    }
    public static void HandlePlayerMovementUpdate(ref NetworkPacket Packet)
    {
        //Log what we are doing
        Log.In("Remote Player Movement Update");
        //Extract all the data from the network packet
        string CharacterName = Packet.ReadString();
        Vector3 CharacterMovement = Packet.ReadVector3();
        //Pass these values to the player handler for updating the remote player
        PlayerManager.Instance.UpdateRemotePlayerMovement(CharacterName, CharacterMovement);
    }

    //Handles instructions to spawn a newly connected game clients player character into our game world
    public static void HandleSpawnOtherPlayer(ref NetworkPacket Packet)
    {
        Log.In("Spawn Remote Player");

        //Read the relevant values from the network packet
        string CharacterName = Packet.ReadString();
        Vector3 CharacterPosition = Packet.ReadVector3();
        Vector3 CharacterMovement = Packet.ReadVector3();
        Quaternion CharacterRotation = Packet.ReadQuaternion();

        //Use the remote player manager to spawn this remote player character into our game world
        PlayerManager.Instance.AddRemotePlayer(CharacterName, CharacterPosition, CharacterMovement, CharacterRotation);
    }

    //Handles instructions to despawn some other dead clients player character from our game world
    public static void HandleRemoveOtherPlayer(ref NetworkPacket Packet)
    {
        Log.In("Remove Remote Player");

        //Read the relevant data values from the network packet
        string CharacterName = Packet.ReadString();

        //Use the remote player manager to despawn this dead clients player character from our game world
        PlayerManager.Instance.RemoveRemotePlayer(CharacterName);
    }

    //Finally enters our player character into the game world once the server has let us know they've added us into the world physics simulation
    public static void HandlePlayerBegin(ref NetworkPacket Packet)
    {
        Log.In("Local Player Begin");

        //Change from the main menu UI to the ingame UI, disable the menu camera
        InterfaceManager.Instance.SetObjectActive("Message Input", true);
        InterfaceManager.Instance.SetObjectActive("Menu Background", false);
        InterfaceManager.Instance.SetObjectActive("Entering World Panel", false);
        CameraManager.Instance.ToggleMainCamera(false);

        //Get the current character index freom the GameState object, use that to fetch our position/rotation then spawn into the game world with those values
        GameState GS = GameState.Instance;
        GS.WorldEntered = true;
        int CharacterIndex = GS.SelectedCharacter - 1;
        Vector3 SpawnLocation = GS.CharacterPositions[CharacterIndex];
        Quaternion SpawnRotation = GS.CharacterRotations[CharacterIndex];
        PlayerManager.Instance.AddLocalPlayer(SpawnLocation, SpawnRotation);

        //Fetch the current zoom/rotation values of the players camera, set the camera to these values then enable camera state broadcasting
        float CameraZoom = GS.CameraZoomLevels[CharacterIndex];
        float CameraXRotation = GS.CameraXRotationValues[CharacterIndex];
        float CameraYRotation = GS.CameraYRotationValues[CharacterIndex];
        GameObject PlayerCamera = PlayerManager.Instance.LocalPlayer.transform.Find("Player Camera").gameObject;
        PlayerCamera.GetComponent<PlayerCameraController>().SetCamera(CameraZoom, CameraXRotation, CameraYRotation);
    }

    //Server telling us to force move our character to a new location
    public static void HandleForceMovePlayer(ref NetworkPacket Packet)
    {
        Log.In("Force Move Player");

        Vector3 NewLocation = Packet.ReadVector3();
        PlayerManager.Instance.UpdateLocalPlayerPosition(NewLocation);
    }

    //Server telling us to force move some other character to a new location
    public static void HandleForceMoveOtherPlayer(ref NetworkPacket Packet)
    {
        Log.In("Force Move Other Player");

        string CharacterName = Packet.ReadString();
        Vector3 NewLocation = Packet.ReadVector3();
        PlayerManager.Instance.UpdateRemotePlayerPosition(CharacterName, NewLocation);
    }
}

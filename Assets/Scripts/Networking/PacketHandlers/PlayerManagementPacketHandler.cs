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

    //Handles moving one of the other players ingame characters to a new updated location
    public static void HandlePlayerUpdate(ref NetworkPacket Packet)
    {
        //Read the relevant values from the network packet
        string CharacterName = Packet.ReadString();
        Vector3 CharacterPosition = Packet.ReadVector3();

        //Pass these values onto the player handler so it can give the remote player object its new target location
        PlayerManager.Instance.UpdateRemotePlayerLocation(CharacterName, CharacterPosition);
    }

    //Handles instructions to spawn a newly connected game clients player character into our game world
    public static void HandleSpawnPlayer(ref NetworkPacket Packet)
    {
        //Read the relevant values from the network packet
        string CharacterName = Packet.ReadString();
        Vector3 CharacterPosition = Packet.ReadVector3();

        //Use the remote player manager to spawn this remote player character into our game world
        PlayerManager.Instance.AddRemotePlayer(CharacterName, CharacterPosition);
    }

    //Handles instructions to despawn some other dead clients player character from our game world
    public static void HandleRemovePlayer(ref NetworkPacket Packet)
    {
        //Read the relevant data values from the network packet
        string CharacterName = Packet.ReadString();

        //Use the remote player manager to despawn this dead clients player character from our game world
        PlayerManager.Instance.RemoveRemotePlayer(CharacterName);
    }

    //Finally enters our player character into the game world once the server has let us know they've added us into the world physics simulation
    public static void HandlePlayerBegin(ref NetworkPacket Packet)
    {
        //Print message to show we received persmission from the server to start playing
        Log.Chat("server gave permission to start playing");

        //Change from the main menu UI to the ingame UI
        InterfaceManager.Instance.SetObjectActive("Message Input", true);
        InterfaceManager.Instance.SetObjectActive("Menu Background", false);
        InterfaceManager.Instance.SetObjectActive("Entering World Panel", false);

        //Disable the menu camera and spawn our character into the game world
        CameraManager.Instance.ToggleMainCamera(false);
        Vector3 PlayerSpawnLocation = GameState.Instance.CharacterPositions[GameState.Instance.SelectedCharacter - 1];
        PlayerManager.Instance.AddLocalPlayer(PlayerSpawnLocation);
    }
}

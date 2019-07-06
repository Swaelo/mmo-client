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

        //Pass these values onto the remote player handler and it will move the correct player to its new target location
        RemotePlayerHandler.Instance.UpdatePlayerPosition(CharacterName, CharacterPosition);
    }

    //Handles instructions to spawn a newly connected game clients player character into our game world
    public static void HandleSpawnPlayer(ref NetworkPacket Packet)
    {
        //Read the relevant values from the network packet
        string CharacterName = Packet.ReadString();
        Vector3 CharacterPosition = Packet.ReadVector3();

        //Use the remote player manager to spawn this remote player character into our game world
        RemotePlayerHandler.Instance.AddRemotePlayer(CharacterName, CharacterPosition);
    }

    //Handles instructions to despawn some other dead clients player character from our game world
    public static void HandleRemovePlayer(ref NetworkPacket Packet)
    {
        //Read the relevant data values from the network packet
        string CharacterName = Packet.ReadString();

        //Use the remote player manager to despawn this dead clients player character from our game world
        RemotePlayerHandler.Instance.RemoveRemotePlayer(CharacterName);
    }
}

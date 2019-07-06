// ================================================================================================================================
// File:        PlayerManagementPacketSender.cs
// Description:	Sends packets to the game server providing our local character controllers updated position values
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerManagementPacketSender : MonoBehaviour
{
    //Singleton Instance
    public static PlayerManagementPacketSender Instance = null;
    void Awake() { Instance = this; }

    public void SendPlayerUpdate(Vector3 CharacterPosition)
    {
        //Create a new NetworkPacket object to store the data for this player update
        NetworkPacket Packet = new NetworkPacket();

        //Write the relevant data values into the network data
        Packet.WriteType(ClientPacketType.PlayerUpdate);
        Packet.WriteString(GameState.Instance.CurrentCharacterName);
        Packet.WriteVector3(CharacterPosition);

        //Add the new NetworkPacket to the outgoing packets queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }
}

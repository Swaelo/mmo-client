// ================================================================================================================================
// File:        PlayerCommunicationPacketSender.cs
// Description:	Sends our chat messages to the game server to be displayed in everyone elses chat windows
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerCommunicationPacketSender : MonoBehaviour
{
    //Singleton Instance
    public static PlayerCommunicationPacketSender Instance = null;
    void Awake() { Instance = this; }

    //Sends a chat message to the game server to be shared with all other ingame clients
    public void SendChatMessage(string Message)
    {
        Log.Out("Chat Message");

        //Create a new NetworkPacket to store the data for this chat message
        NetworkPacket Packet = new NetworkPacket();

        //Write the relevant data values into the packet
        Packet.WriteType(ClientPacketType.PlayerChatMessage);
        Packet.WriteString(Message);

        //Add the new packet to the outgoing queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }
}

// ================================================================================================================================
// File:        MiscellaneousPacketSender.cs
// Description:	Used to send packets to the server like letting it know we are still alive so it doesnt close our connection
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class MiscellaneousPacketSender : MonoBehaviour
{
    //Singleton Instance
    public static MiscellaneousPacketSender Instance = null;
    void Awake() { Instance = this; }

    //Sends an alert to the game server letting them know we are still connected
    public void SendStillAliveAlert()
    {
        //Create a new NetworkPacket object to store the data for this alert
        NetworkPacket Packet = new NetworkPacket();

        //Write the relevant data values into the network packet
        Packet.WriteType(ClientPacketType.StillAlive);

        //Add the new packet to the outgoing packets queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }
}

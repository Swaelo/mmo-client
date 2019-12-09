// ================================================================================================================================
// File:        SystemPacketSender.cs
// Description:	Used for sending low level system messages to the game server
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class SystemPacketSender : MonoBehaviour
{
    public static SystemPacketSender Instance = null;
    void Awake() { Instance = this; }

    /// <summary>
    /// Sends an alert to the game server letting them know we have missed multiple packets and need them all sent back to us again
    /// </summary>
    /// <param name="FirstMissedPacket">Order number of the first packet we missed</param>
    /// <param name="LastMissedPacket">Order number of the last packet we missed</param>
    public void SendMissedPacketsRequest(int FirstMissedPacket)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ClientPacketType.MissedPacketsRequest);
        Packet.WriteInt(FirstMissedPacket);
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    /// <summary>
    /// Sends a reply to the server letting them know we are still connected
    /// </summary>
    public void SendStillConnectedReply()
    {
        Log.Out("Still Connected Reply");
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ClientPacketType.StillConnectedReply);
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }
}
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
    public void SendMissedPacketsRequest(int FirstMissedPacket, int LastMissedPacket)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ClientPacketType.MissedPacketsRequest);
        Packet.WriteInt(FirstMissedPacket);
        Packet.WriteInt(LastMissedPacket);
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
        Log.Chat("Asking for packets " + FirstMissedPacket + " through to " + LastMissedPacket + " to be resent.");
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

    /// <summary>
    /// Sends an alert to the server letting it know we are out of sync and need to be kicked from the game
    /// </summary>
    public void SendOutOfSyncAlert()
    {
        //Give the packet a -1 order number so its immediately processed by the server, ignoring queues and if its currently waiting for missing packets
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteInt(-1);
        Packet.WriteType(ClientPacketType.OutOfSyncAlert);
        ConnectionManager.Instance.SendPacketImmediately(Packet);
    }

    //Sends a request to the server asking to have our character respawn back at the spawn pad
    public void SendRespawnRequest()
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ClientPacketType.PlayerRespawnRequest);
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }
}
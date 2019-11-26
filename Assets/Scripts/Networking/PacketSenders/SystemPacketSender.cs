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
    /// //Sends a message to the game server letting them know we have missed some packets that they have sent us
    /// </summary>
    /// <param name="NextExpectedPacketNumber"></param>
    public void SendMissedPacketsRequest(int NextExpectedPacketNumber)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ClientPacketType.MissedPacketsRequest);
        Packet.WriteInt(NextExpectedPacketNumber);
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    /// <summary>
    /// //Sends a reply to the server letting them know we are still connected
    /// </summary>
    public void SendStillConnectedReply()
    {
        Log.Out("Still Connected Reply");
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ClientPacketType.StillConnectedReply);
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    /// <summary>
    /// //Sends an alert to the server letting it know we are out of sync and need to be kicked from the game
    /// </summary>
    public void SendOutOfSyncAlert()
    {
        //Give the packet a -1 order number so its immediately processed by the server, ignoring queues and if its currently waiting for missing packets
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteInt(-1);
        Packet.WriteType(ClientPacketType.OutOfSyncAlert);
        ConnectionManager.Instance.SendPacketImmediately(Packet);
    }
}
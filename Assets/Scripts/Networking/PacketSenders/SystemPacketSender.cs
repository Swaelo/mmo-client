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

    //Sends a message to the game server letting them know we have missed some packets that they have sent us
    public void SendMissedPacketAlert(int NextExpectedPacketNumber)
    {
        Log.Chat("Sending missed packets alert");
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ClientPacketType.MissedPackets);
        Packet.WriteInt(NextExpectedPacketNumber);
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    //Sends a reply to the server letting them know we are still connected
    public void SendStillConnectedReply()
    {
        Log.Out("Still Connected Reply");
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ClientPacketType.StillConnectedReply);
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }
}
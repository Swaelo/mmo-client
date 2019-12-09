// ================================================================================================================================
// File:        CombatPacketSender.cs
// Description:	Functions for sending instructions to the game server regarding combat actions that are performed
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class CombatPacketSender : MonoBehaviour
{
    //Singleton Class Instance
    public static CombatPacketSender Instance = null;
    void Awake() { Instance = this; }

    //Sends an alert to the game server letting it know we have performed an attack
    public void SendPlayerAttackAlert(Vector3 AttackLocation)
    {
        Log.Out("Player Attack Alert");
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ClientPacketType.PlayerAttackAlert);
        Packet.WriteVector3(AttackLocation);
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    //Sends an alert to the game server letting it know we want to respawn
    public void SendPlayerRespawnRequest()
    {
        Log.Out("Player Respawn Request");
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ClientPacketType.PlayerRespawnRequest);
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }
}
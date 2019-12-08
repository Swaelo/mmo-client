// ================================================================================================================================
// File:        CombatPacketHandler.cs
// Description:	Handles any server packets which are received regarding any combat actions performed
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class CombatPacketHandler : MonoBehaviour
{
    //Reads packet values for PlayerTakeHit
    public static NetworkPacket GetValuesLocalPlayerTakeHit(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.LocalPlayerTakeHit);
        Packet.WriteInt(ReadFrom.ReadInt());
        return Packet;
    }

    //Handles the local player taking damage, updates the HP on the UI
    public static void HandleLocalPlayerTakeHit(ref NetworkPacket Packet)
    {
        Log.In("Handle Local Player Take Hit");

        int NewHPValue = Packet.ReadInt();

        PlayerManager.Instance.DamageLocalPlayer(NewHPValue);
    }

    //Reads packet values for RemotePlayerTakeHit
    public static NetworkPacket GetValuesRemotePlayerTakeHit(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.RemotePlayerTakeHit);
        Packet.WriteString(ReadFrom.ReadString());
        Packet.WriteInt(ReadFrom.ReadInt());
        return Packet;
    }

    //Handles a remote player taking damage, update the HP above their head
    public static void HandleRemotePlayerTakeHit(ref NetworkPacket Packet)
    {
        Log.In("Handle Remote Player Take Hit");

        string PlayerName = Packet.ReadString();
        int NewHPValue = Packet.ReadInt();

        PlayerManager.Instance.DamageRemotePlayer(PlayerName, NewHPValue);
    }

    //Reads packet values for LocalPlayerDead
    public static NetworkPacket GetValuesLocalPlayerDead(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.LocalPlayerDead);
        return Packet;
    }

    //Handles the local player being killed
    public static void HandleLocalPlayerDead(ref NetworkPacket Packet)
    {
        Log.In("Handle Local Player Dead");
        PlayerManager.Instance.KillLocalPlayer();
    }

    //Reads packet values for RemotePlayerDead
    public static NetworkPacket GetValuesRemotePlayerDead(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.RemotePlayerDead);
        Packet.WriteString(ReadFrom.ReadString());
        return Packet;
    }

    //Handles some other remote player being killed
    public static void HandleRemotePlayerDead(ref NetworkPacket Packet)
    {
        Log.In("Handle Remote Player Dead");
        string CharacterName = Packet.ReadString();
        PlayerManager.Instance.KillRemotePlayer(CharacterName);
    }

    //Reads packet values for LocalPlayerRespawn
    public static NetworkPacket GetValuesLocalPlayerRespawn(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.LocalPlayerRespawn);
        Packet.WriteVector3(ReadFrom.ReadVector3());
        Packet.WriteQuaternion(ReadFrom.ReadQuaternion());
        Packet.WriteFloat(ReadFrom.ReadFloat());
        Packet.WriteFloat(ReadFrom.ReadFloat());
        Packet.WriteFloat(ReadFrom.ReadFloat());
        return Packet;
    }

    //Handles spawning our local player character back into the game world
    public static void HandleLocalPlayerRespawn(ref NetworkPacket Packet)
    {
        Vector3 SpawnPos = Packet.ReadVector3();
        Quaternion SpawnRot = Packet.ReadQuaternion();
        float CameraZoom = Packet.ReadFloat();
        float CameraX = Packet.ReadFloat();
        float CameraY = Packet.ReadFloat();
        PlayerManager.Instance.RespawnLocalPlayer(SpawnPos, SpawnRot, CameraZoom, CameraX, CameraY);
    }

    //Reads packet values for RemotePlayerRespawn
    public static NetworkPacket GetValuesRemotePlayerRespawn(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.RemotePlayerRespawn);
        Packet.WriteString(ReadFrom.ReadString());
        Packet.WriteVector3(ReadFrom.ReadVector3());
        Packet.WriteQuaternion(ReadFrom.ReadQuaternion());
        return Packet;
    }

    //Handles spawning some other remote player character back into the game world
    public static void HandleRemotePlayerRespawn(ref NetworkPacket Packet)
    {
        string CharacterName = Packet.ReadString();
        Vector3 SpawnPos = Packet.ReadVector3();
        Quaternion SpawnRot = Packet.ReadQuaternion();
        PlayerManager.Instance.RespawnRemotePlayer(CharacterName, SpawnPos, SpawnRot);
    }
}

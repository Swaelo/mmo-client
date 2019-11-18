// ================================================================================================================================
// File:        PlayerManagementPacketSender.cs
// Description:	Sends packets to the game server providing our local character controllers updated position values
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using UnityEngine;

public class PlayerManagementPacketSender : MonoBehaviour
{
    //Singleton Instance
    public static PlayerManagementPacketSender Instance = null;
    void Awake() { Instance = this; }

    //Sends the server our characters current position value
    public void SendCharacterPosition(Vector3 CharacterPosition)
    {
        //Log a message showing what packet is being sent out
        Log.Out("Character Position Update: " + CharacterPosition.ToString());

        //Create the packet were going to send
        NetworkPacket Packet = new NetworkPacket();

        //Write the data values into the packet
        Packet.WriteType(ClientPacketType.CharacterPositionUpdate);
        Packet.WriteString(GameState.Instance.CurrentCharacterName);
        Packet.WriteVector3(CharacterPosition);

        //Queue the packet for transmission
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    //Sends the server our characters current rotation value
    public void SendCharacterRotation(Quaternion CharacterRotation)
    {
        //Log a message showing what packet is being sent out
        Log.Out("Character Rotation Update: " + CharacterRotation.ToString());
        //Create the packet were going to send
        NetworkPacket Packet = new NetworkPacket();
        //Write the data values into the packet
        Packet.WriteType(ClientPacketType.CharacterRotationUpdate);
        Packet.WriteString(GameState.Instance.CurrentCharacterName);
        Packet.WriteQuaternion(CharacterRotation);
        //Queue the packet for transmission
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    //Sends the server our characters current movement value
    public void SendCharacterMovement(Vector3 CharacterMovement)
    {
        //Log a message showing what packet is being sent out
        Log.Out("Character Movement Update: " + CharacterMovement.ToString());
        //Create the packet were going to send
        NetworkPacket Packet = new NetworkPacket();
        //Write the data values into the packet
        Packet.WriteType(ClientPacketType.CharacterMovementUpdate);
        Packet.WriteString(GameState.Instance.CurrentCharacterName);
        Packet.WriteVector3(CharacterMovement);
        //Queue the packet for transmission
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    //Gives the server our player cameras current Zoom and Rotation values
    public void SendPlayerCameraUpdate(float CameraZoom, float XRotation, float YRotation)
    {
        Log.Out("Camera Update");

        //Create a new network packet
        NetworkPacket Packet = new NetworkPacket();

        //Write all the relevant values into the packet
        Packet.WriteType(ClientPacketType.CameraSettings);
        Packet.WriteString(GameState.Instance.CurrentCharacterName);
        Packet.WriteFloat(CameraZoom);
        Packet.WriteFloat(XRotation);
        Packet.WriteFloat(YRotation);

        //Add the packet to the queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    //Tells the server we are unaware about a player character which we think should exist in the game world
    public void SendUnknownCharacterAlert(string CharacterName)
    {
        //Log what we are doing
        Log.Out("Unknown Character Alert: " + CharacterName);
        //Fill a new NetworkPacket with data
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ClientPacketType.RemotePlayerUnknown);
        Packet.WriteString(CharacterName);
        //Queue the packet for transmission
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }
}

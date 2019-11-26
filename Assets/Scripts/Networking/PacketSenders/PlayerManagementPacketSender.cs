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

    /// <summary>
    /// //Sends the server our characters current position value
    /// </summary>
    /// <param name="CharacterPosition">The client characters updated position value being sent to the server</param>
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

    /// <summary>
    /// //Sends the server our characters current rotation value
    /// </summary>
    /// <param name="CharacterRotation">The client characters updated rotation value being sent to the server</param>
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

    /// <summary>
    /// //Sends the server our characters current movement value
    /// </summary>
    /// <param name="CharacterMovement">The client characters updated movement input value being sent to the server</param>
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

    /// <summary>
    /// //Gives the server our player cameras current Zoom and Rotation values
    /// </summary>
    /// <param name="CameraZoom">Client characters updated camera zoom level being sent to the server</param>
    /// <param name="XRotation">Client characters updated camera XRotation value being sent to the server</param>
    /// <param name="YRotation">Client characters updated camera YRotation value being sent to the server</param>
    public void SendPlayerCameraUpdate(float CameraZoom, float XRotation, float YRotation)
    {
        Log.Out("Camera Update");

        //Create a new network packet
        NetworkPacket Packet = new NetworkPacket();

        //Write all the relevant values into the packet
        Packet.WriteType(ClientPacketType.CharacterCameraUpdate);
        Packet.WriteString(GameState.Instance.CurrentCharacterName);
        Packet.WriteFloat(CameraZoom);
        Packet.WriteFloat(XRotation);
        Packet.WriteFloat(YRotation);

        //Add the packet to the queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }
}

// ================================================================================================================================
// File:        PacketQueue.cs
// Description:	Stores a list of outgoing network packets to be sent to the server in the next communication interval
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class PacketQueue
{
    private float CommunicationInterval = 0.1f; //How often the outgoing packets list will be emptied and transmitted to the server
    private float NextCommunication = 0.1f; //Time remaining before the next communication interval occurs
    private List<NetworkPacket> OutgoingPackets;    //Current list of packets to be transmitted in the next communication interval

    //Default constructor
    public PacketQueue()
    {
        //Initialize the outgoing packets list
        OutgoingPackets = new List<NetworkPacket>();
    }

    //Adds a network packet to the outgoing queue
    public void QueuePacket(NetworkPacket NewPacket)
    {
        //Add the new packet to the outgoing queue
        OutgoingPackets.Add(NewPacket);
    }

    //Automatically tracks the interval timer, resetting it and transmitting all queued packets every time it reaches zero
    public void UpdateQueue()
    {
        //Count down the timer until the next communication event should occur
        NextCommunication -= Time.deltaTime;

        //Transmit all the outgoing packets to the server and reset the timer whenever it reaches zero
        if(NextCommunication <= 0.0f)
            TransmitPackets();
    }

    //Transmits all outgoing packets to the game server and resets the interval timer
    private void TransmitPackets()
    {
        //Reset the communication interval timer
        NextCommunication = CommunicationInterval;

        //The data of every packet in the queue will be combined together into a single string
        string TotalData = "";

        //Before we process all the packet data, theres new more information we want to add for the server beforehand
        //If the user is currently active in the game world with one of their characters then we need to send queue its updated info before sending data to the game server
        if (GameState.Instance.WorldEntered)
        {
            //First we want to transmit to the server any of the player characters values which have changed since last transmission
            PlayerCharacterController PlayerController = GameObject.Find("Local Player(Clone)").GetComponent<PlayerCharacterController>();
            PlayerController.TransmitValues();
            //Second, we want to send the player cameras zoom/xrotation/yrotation values if they have changed since last transmission
            PlayerCameraController PlayerCamera = GameObject.Find("Player Camera").GetComponent<PlayerCameraController>();
            if (PlayerCamera.CameraSettingsChanged())
                PlayerCamera.BroadcastCameraSettings();
        }

        //Loop through all the packets in the queue, append the data of each packet onto the end of our TotalData string
        foreach(NetworkPacket Packet in OutgoingPackets)
            TotalData += Packet.PacketData;

        //If the final string actually contains some data then we can now transmit that to the game server
        if(TotalData != "")
            ConnectionManager.Instance.MessageServer(TotalData);

        //Reinitialize the outgoing packets queue as all their data has now been transmitted to the game server
        OutgoingPackets = new List<NetworkPacket>();
    }
}

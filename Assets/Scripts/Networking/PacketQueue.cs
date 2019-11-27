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
    private List<NetworkPacket> SecondaryQueue; //Secondary queue where new packets are added to while the main queue is being used for transmission
    private bool MainQueueInUse = false;    //Flag set so we know which queue to use when adding new packets to the queue

    //Default constructor
    public PacketQueue()
    {
        //Initialize the outgoing packets list
        OutgoingPackets = new List<NetworkPacket>();
        SecondaryQueue = new List<NetworkPacket>();
    }

    //Adds a network packet to the outgoing queue
    public void QueuePacket(NetworkPacket NewPacket)
    {
        //Check the value of the MainQueueInUse flag so we know where to queue new packets to
        List<NetworkPacket> CurrentQueue = MainQueueInUse ? SecondaryQueue : OutgoingPackets;

        //Add the new packet to the outgoing queue
        CurrentQueue.Add(NewPacket);
    }

    //Adds a list of packets to the main queue
    private void AddToMainQueue(List<NetworkPacket> Packets)
    {
        foreach (NetworkPacket Packet in Packets)
            AddToMainQueue(Packet);
    }

    //Adds a packet to the main queue
    private void AddToMainQueue(NetworkPacket Packet)
    {
        OutgoingPackets.Add(Packet);
    }

    //Adds a packet to the secondary queue
    private void AddToSecondaryQueue(NetworkPacket Packet)
    {
        SecondaryQueue.Add(Packet);
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
        //Reset the secondary queue, then enable the flag so any new packets are added there for the time being
        SecondaryQueue.Clear();
        MainQueueInUse = true;

        //Reset the communication interval timer
        NextCommunication = CommunicationInterval;

        //Combine the data of every packet in the main queue into a single string
        string TotalData = "";
        foreach (NetworkPacket Packet in OutgoingPackets)
            TotalData += Packet.PacketData;

        //Transmit this data to the server if it didnt end up still being empty
        if (TotalData != "")
            ConnectionManager.Instance.SendPacket(TotalData);

        //Reset the contents of the main queue, then copy anything in the secondary queue into the main queue
        OutgoingPackets.Clear();
        foreach (NetworkPacket Packet in SecondaryQueue)
            OutgoingPackets.Add(Packet);

        //Now disable the flag so new packets are again being added into the main queue
        MainQueueInUse = false;
    }
}

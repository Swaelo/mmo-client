// ================================================================================================================================
// File:        PacketQueue.cs
// Description:	Stores a list of outgoing network packets to be sent to the server in the next communication interval
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PacketQueue
{
    private float CommunicationInterval = 0.5f;   //How often the outgoing packets queue is transmitted to the server
    private float NextCommunication = 0.5f;   //Time left until we transmit queued packets to the server
    //Order number for the next packet to be sent to the server
    private int MostPreviousPacketNumber = 0;
    private int GetNextOutgoingPacketNumber() { return ++MostPreviousPacketNumber; }
    //Current set of packets waiting to be transmitted, and the total set of packets that have been sent to the server so far (maximum previous 150 packets)
    private Dictionary<int, NetworkPacket> OutgoingPacketQueue = new Dictionary<int, NetworkPacket>();
    private Dictionary<int, NetworkPacket> PacketHistory = new Dictionary<int, NetworkPacket>();
    //Set when the server has told us they are missing some packets and need them to be resent back again
    public bool PacketsToResend = false;
    public int ResendStartNumber = -1;
    //Packet order number last recieved from the server
    public int LastPacketNumberRecieved = 0;

    //Adds a NetworkPacket to the outgoing packets queue
    public void QueuePacket(NetworkPacket Packet)
    {
        //Add the order number to the front of the packet data
        int OrderNumber = GetNextOutgoingPacketNumber();
        Packet.AddPacketOrderNumber(OrderNumber);

        //Add it into the queue for transmission later, and into the total history list also
        OutgoingPacketQueue.Add(OrderNumber, Packet);
        PacketHistory.Add(OrderNumber, Packet);

        //Maintain a maximum history of 150 previous packets
        if (PacketHistory.Count > 150)
            PacketHistory.Remove(OrderNumber - 150);
    }

    //Copy all outgoing packets into a brand new array, then transmit them all to the server (or, resend all the packets since the last missing packet if they requested that)
    public void TransmitPackets()
    {
        //Copy the current outgoing packet queue into a new array, then reset it so packets can keep getting queued into it
        Dictionary<int, NetworkPacket> TransmissionQueue = new Dictionary<int, NetworkPacket>(OutgoingPacketQueue);
        OutgoingPacketQueue.Clear();

        //Create a new strig we will fill with the data of every packet in the transmission queue so its all sent at once
        string TotalData = "";

        //Append the data of each packet in the transmission queue if we dont have missing packets to resend
        if(!PacketsToResend)
        {
            foreach (KeyValuePair<int, NetworkPacket> Packet in TransmissionQueue)
                TotalData += Packet.Value.PacketData;
        }
        //Otherwise we append the data of every packet in history, starting from the first the client is missing, to the last one in the dictionary
        else
        {
            //Check thie missing packets that are being requested are still being stored in memory
            if(!PacketHistory.ContainsKey(ResendStartNumber))
            {
                //Print an error and close the connection to the server if they request packets outside of the current history
                Log.Chat("ERROR: Server requesting packets outside of history, closing the connection.");

                //Change to the disconnected from server scene
                SceneManager.LoadScene("Disconnected");

                //Exit the function
                return;
            }

            //Loop from the first missing packet number, all the way to the most previously queued packet and all all of their data into the string
            for (int i = ResendStartNumber; i < MostPreviousPacketNumber; i++)
                TotalData += PacketHistory[i].PacketData;

            PacketsToResend = false;
        }

        //Now transmit all this data to the server if theres anything to send
        if(TotalData != "")
        {
            //Convert the data into byte array then send it over to the game server
            byte[] PacketData = Encoding.UTF8.GetBytes(TotalData);
            ConnectionManager.Instance.ServerConnection.Send(PacketData);
        }
    }

    //Tracks the interval timer, transmitting all queued packets and resetting the timer whenever it reaches zero
    public void UpdateQueue()
    {
        //Count down the timer
        NextCommunication -= Time.deltaTime;

        //Transmit packets and reset timer at zero
        if (NextCommunication <= 0.0f)
        {
            TransmitPackets();
            NextCommunication = CommunicationInterval;
        }
    }
}

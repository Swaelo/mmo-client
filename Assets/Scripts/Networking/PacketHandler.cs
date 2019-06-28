// ================================================================================================================================
// File:        PacketHandler.cs
// Description:	Passes packet data on to be processed by the correct packet handler function registered for that packet type
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketHandler : MonoBehaviour
{
    //Singleton Instance
    public static PacketHandler Instance = null;

    //Packet handler functions each mapped to their packet type number
    public delegate void Packet(string PacketData);
    public Dictionary<int, Packet> PacketHandlers;

    private void Awake()
    {
        //Assign the singleton class instance
        Instance = this;

        //Register all of the packet handler functions into the dictionary
        PacketHandlers = new Dictionary<int, Packet>();
        PacketHandlers.Add((int)ServerPacketTypes.AccountLoginReply, AccountLoginReplyHandler.HandleAccountLoginReply);
        PacketHandlers.Add((int)ServerPacketTypes.AccountRegistrationReply, AccountRegisterReplyHandler.HandleAccountRegisterReply);
        PacketHandlers.Add((int)ServerPacketTypes.CharacterDataReply, CharacterDataReplyHandler.HandleCharacterDataReply);
    }

    //Reads in a packet sent from the server and passes it onto whatever handle function that packet type is mapped onto
    public void ReadClientPacket(string PacketData)
    {
        //First open up the packet and find out what type it is
        string PacketTypeString = PacketData.Substring(0, PacketData.IndexOf(' '));
        int PacketType = Int32.Parse(PacketTypeString);

        //Invoke the matching handler function
        if(PacketHandlers.TryGetValue(PacketType, out Packet Packet))
            Packet.Invoke(PacketData);
    }
}

// ================================================================================================================================
// File:        PacketHandler.cs
// Description:	Passes packet data on to be processed by the correct packet handler function registered for that packet type
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class PacketHandler : MonoBehaviour
{
    //Singleton Instance
    public static PacketHandler Instance = null;

    //Packet handler functions each mapped to their packet type number
    public delegate void Packet(ref NetworkPacket Packet);
    public Dictionary<ServerPacketType, Packet> PacketHandlers;

    private void Awake()
    {
        //Assign the singleton class instance
        Instance = this;

        //Create a new dictionary to store all the packet handler functions
        PacketHandlers = new Dictionary<ServerPacketType, Packet>();

        //Map all the account management packet handlers into the dictionary
        PacketHandlers.Add(ServerPacketType.AccountLoginReply, AccountManagementPacketHandler.HandleAccountLoginReply);
        PacketHandlers.Add(ServerPacketType.AccountRegistrationReply, AccountManagementPacketHandler.HandleAccountRegisterReply);
        PacketHandlers.Add(ServerPacketType.CharacterDataReply, AccountManagementPacketHandler.HandleCharacterDataReply);
        PacketHandlers.Add(ServerPacketType.CharacterCreationReply, AccountManagementPacketHandler.HandleCreateCharacterReply);

        //Map all the game world state packet handlers into the dictionary
        PacketHandlers.Add(ServerPacketType.ActivePlayerList, GameWorldStatePacketHandler.HandleActivePlayerList);
        PacketHandlers.Add(ServerPacketType.ActiveEntityList, GameWorldStatePacketHandler.HandleActiveEntityList);
        PacketHandlers.Add(ServerPacketType.ActiveItemList, GameWorldStatePacketHandler.HandleActiveItemList);
        PacketHandlers.Add(ServerPacketType.PlayerInventoryItems, GameWorldStatePacketHandler.HandleInventoryContents);
        PacketHandlers.Add(ServerPacketType.PlayerEquipmentItems, GameWorldStatePacketHandler.HandleEquipmentContents);
        PacketHandlers.Add(ServerPacketType.PlayerActionBarAbilities, GameWorldStatePacketHandler.HandleActionBarContents);

        //Map all the player management packet handlers into the dictionary
        PacketHandlers.Add(ServerPacketType.PlayerUpdate, PlayerManagementPacketHandler.HandlePlayerUpdate);
        PacketHandlers.Add(ServerPacketType.SpawnPlayer, PlayerManagementPacketHandler.HandleSpawnPlayer);
        PacketHandlers.Add(ServerPacketType.RemovePlayer, PlayerManagementPacketHandler.HandleRemovePlayer);

        //Map player communication packet handlers into the dictionary
        PacketHandlers.Add(ServerPacketType.PlayerChatMessage, PlayerCommunicationPacketHandler.HandleChatMessage);
        PacketHandlers.Add(ServerPacketType.PlayerBegin, PlayerManagementPacketHandler.HandlePlayerBegin);
    }

    //Reads in a packet sent from the server and passes it onto whatever handle function that packet type is mapped onto
    public void ReadServerPacket(string PacketMessage)
    {
        //Create a new NetworkPacket object to store the string value that was received from the game server
        NetworkPacket NewPacket = new NetworkPacket(PacketMessage);

        //Loop through all of the data in this net packet, passing each section of instructions on to their registered handler function
        while(!NewPacket.FinishedReading())
        {
            //Read the next packet type value from the remaining packet data
            ServerPacketType PacketType = NewPacket.ReadType();

            //Use this enum value to invoke the packet handler function that is registered to this packet type
            if(PacketHandlers.TryGetValue(PacketType, out Packet Packet))
                Packet.Invoke(ref NewPacket);
        }
    }
}

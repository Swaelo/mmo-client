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

        //Register all the packet readers and handlers
        RegisterPacketHandlers();
    }

    private void RegisterPacketHandlers()
    {
        //Create a new dictionary to store all the packet handler functions
        PacketHandlers = new Dictionary<ServerPacketType, Packet>();

        //Account Management Packet Handlers
        PacketHandlers.Add(ServerPacketType.AccountLoginReply, AccountManagementPacketHandler.HandleAccountLoginReply);
        PacketHandlers.Add(ServerPacketType.AccountRegistrationReply, AccountManagementPacketHandler.HandleAccountRegistrationReply);
        PacketHandlers.Add(ServerPacketType.CharacterDataReply, AccountManagementPacketHandler.HandleCharacterDataReply);
        PacketHandlers.Add(ServerPacketType.CreateCharacterReply, AccountManagementPacketHandler.HandleCreateCharacterReply);

        //Game World State Packet Handlers
        PacketHandlers.Add(ServerPacketType.ActivePlayerList, GameWorldStatePacketHandler.HandleActivePlayerList);
        PacketHandlers.Add(ServerPacketType.ActiveEntityList, GameWorldStatePacketHandler.HandleActiveEntityList);
        PacketHandlers.Add(ServerPacketType.ActiveItemList, GameWorldStatePacketHandler.HandleActiveItemList);
        PacketHandlers.Add(ServerPacketType.InventoryContents, GameWorldStatePacketHandler.HandleInventoryContents);
        PacketHandlers.Add(ServerPacketType.EquippedItems, GameWorldStatePacketHandler.HandleEquippedItems);
        PacketHandlers.Add(ServerPacketType.SocketedAbilities, GameWorldStatePacketHandler.HandleSocketedAbilities);

        //Player Communication Packet Handlers
        PacketHandlers.Add(ServerPacketType.PlayerChatMessage, PlayerCommunicationPacketHandler.HandleChatMessage);

        //Player Management Packet Handlers
        PacketHandlers.Add(ServerPacketType.UpdateRemotePlayer, PlayerManagementPacketHandler.HandleUpdateRemotePlayer);
        PacketHandlers.Add(ServerPacketType.AddRemotePlayer, PlayerManagementPacketHandler.HandleAddRemotePlayer);
        PacketHandlers.Add(ServerPacketType.RemoveRemotePlayer, PlayerManagementPacketHandler.HandleRemoveRemotePlayer);
        PacketHandlers.Add(ServerPacketType.AllowPlayerBegin, PlayerManagementPacketHandler.HandleAllowPlayerBegin);
        PacketHandlers.Add(ServerPacketType.RemotePlayerPlayAnimationAlert, PlayerManagementPacketHandler.HandleRemotePlayerPlayAnimation);

        //System Packet Handlers
        PacketHandlers.Add(ServerPacketType.StillConnectedCheck, SystemPacketHandler.HandleStillConnectedCheck);
        PacketHandlers.Add(ServerPacketType.MissingPacketsRequest, SystemPacketHandler.HandleMissingPacketsRequest);
        PacketHandlers.Add(ServerPacketType.KickedFromServer, SystemPacketHandler.HandleKickedFromServer);

        //Combat Packet Handlers
        PacketHandlers.Add(ServerPacketType.LocalPlayerTakeHit, CombatPacketHandler.HandleLocalPlayerTakeHit);
        PacketHandlers.Add(ServerPacketType.RemotePlayerTakeHit, CombatPacketHandler.HandleRemotePlayerTakeHit);
        PacketHandlers.Add(ServerPacketType.LocalPlayerDead, CombatPacketHandler.HandleLocalPlayerDead);
        PacketHandlers.Add(ServerPacketType.RemotePlayerDead, CombatPacketHandler.HandleRemotePlayerDead);
        PacketHandlers.Add(ServerPacketType.LocalPlayerRespawn, CombatPacketHandler.HandleLocalPlayerRespawn);
        PacketHandlers.Add(ServerPacketType.RemotePlayerRespawn, CombatPacketHandler.HandleRemotePlayerRespawn);
    }

    private NetworkPacket ReadPacketValues(ServerPacketType PacketType, NetworkPacket ReadFrom)
    {
        switch (PacketType)
        {
            //Account Management
            case (ServerPacketType.AccountLoginReply):
                return AccountManagementPacketHandler.GetValuesAccountLoginReply(ReadFrom);
            case (ServerPacketType.AccountRegistrationReply):
                return AccountManagementPacketHandler.GetValuesAccountRegistrationReply(ReadFrom);
            case (ServerPacketType.CharacterDataReply):
                return AccountManagementPacketHandler.GetValuesCharacterDataReply(ReadFrom);
            case (ServerPacketType.CreateCharacterReply):
                return AccountManagementPacketHandler.GetValuesCreateCharacterReply(ReadFrom);

            //Game World State
            case (ServerPacketType.ActivePlayerList):
                return GameWorldStatePacketHandler.GetValuesActivePlayerList(ReadFrom);
            case (ServerPacketType.ActiveEntityList):
                return GameWorldStatePacketHandler.GetValuesActiveEntityList(ReadFrom);
            case (ServerPacketType.ActiveItemList):
                return GameWorldStatePacketHandler.GetValuesActiveItemList(ReadFrom);
            case (ServerPacketType.InventoryContents):
                return GameWorldStatePacketHandler.GetValuesInventoryContents(ReadFrom);
            case (ServerPacketType.EquippedItems):
                return GameWorldStatePacketHandler.GetValuesActivePlayerList(ReadFrom);
            case (ServerPacketType.SocketedAbilities):
                return GameWorldStatePacketHandler.GetValuesSocketedAbilities(ReadFrom);

            //Player Communication
            case (ServerPacketType.PlayerChatMessage):
                return PlayerCommunicationPacketHandler.GetValuesChatMessage(ReadFrom);

            //Player Management
            case (ServerPacketType.UpdateRemotePlayer):
                return PlayerManagementPacketHandler.GetValuesUpdateRemotePlayer(ReadFrom);
            case (ServerPacketType.AddRemotePlayer):
                return PlayerManagementPacketHandler.GetValuesAddRemotePlayer(ReadFrom);
            case (ServerPacketType.RemoveRemotePlayer):
                return PlayerManagementPacketHandler.GetValuesRemoveRemotePlayer(ReadFrom);
            case (ServerPacketType.AllowPlayerBegin):
                return PlayerManagementPacketHandler.GetValuesAllowPlayerBegin(ReadFrom);
            case (ServerPacketType.RemotePlayerPlayAnimationAlert):
                return PlayerManagementPacketHandler.GetValuesRemotePlayerPlayAnimation(ReadFrom);

            //System
            case (ServerPacketType.StillConnectedCheck):
                return SystemPacketHandler.GetValuesStillConnectedCheck(ReadFrom);
            case (ServerPacketType.MissingPacketsRequest):
                return SystemPacketHandler.GetValuesMissingPacketsRequest(ReadFrom);
            case (ServerPacketType.KickedFromServer):
                return SystemPacketHandler.GetValuesKickedFromServer(ReadFrom);

            //Combat
            case (ServerPacketType.LocalPlayerTakeHit):
                return CombatPacketHandler.GetValuesLocalPlayerTakeHit(ReadFrom);
            case (ServerPacketType.RemotePlayerTakeHit):
                return CombatPacketHandler.GetValuesRemotePlayerTakeHit(ReadFrom);
            case (ServerPacketType.LocalPlayerDead):
                return CombatPacketHandler.GetValuesLocalPlayerDead(ReadFrom);
            case (ServerPacketType.RemotePlayerDead):
                return CombatPacketHandler.GetValuesRemotePlayerDead(ReadFrom);
            case (ServerPacketType.LocalPlayerRespawn):
                return CombatPacketHandler.GetValuesLocalPlayerRespawn(ReadFrom);
            case (ServerPacketType.RemotePlayerRespawn):
                return CombatPacketHandler.GetValuesRemotePlayerRespawn(ReadFrom);
        }
        return new NetworkPacket();
    }

    //Reads in a packet sent from the server and passes it onto whatever handle function that packet type is mapped onto
    public void ReadServerPacket(string PacketData)
    {
        //Store the total set of packet data into a new PacketData object for easier reading
        NetworkPacket TotalPacket = new NetworkPacket(PacketData);

        //Iterate over all the packet data until we finish reading and handling it all
        while (!TotalPacket.FinishedReading())
        {
            //Read the packets order number and packet type enum values
            int OrderNumber = TotalPacket.ReadInt();
            ServerPacketType PacketType = TotalPacket.ReadType();

            //Get the rest of the values for this set based on the packet type, then put the order number back to the front
            NetworkPacket SectionPacket = ReadPacketValues(PacketType, TotalPacket);

            //Compare this packets order number to see if its arrived in the order we were expecting
            int ExpectedOrderNumber = ConnectionManager.Instance.PacketQueue.LastPacketNumberRecieved + 1;
            bool InOrder = OrderNumber == ExpectedOrderNumber;

            //If the packet arrived in order then it gets processed normally
            if (InOrder)
            {
                //Reset the packets data before we start handling it
                SectionPacket.ResetRemainingData();

                //Read away the packet type value as its not needed when processing packets immediately
                SectionPacket.ReadType();

                //Pass the section packet on to its handler function
                if (PacketHandlers.TryGetValue(PacketType, out Packet Packet))
                    Packet.Invoke(ref SectionPacket);

                //Store this as the last packet that was processed
                ConnectionManager.Instance.PacketQueue.LastPacketNumberRecieved = OrderNumber;
            }
            //If packets arrive out of order tell the server the order number that we were expecting to recieve next so everything since that packet can be resent
            else
            {
                //Tell the server what we need resent and disregard everything else in this packet
                SystemPacketSender.Instance.SendMissedPacketsRequest(ExpectedOrderNumber);

                return;
            }
        }
    }
}
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

    //If we fall behind the server, we need to store any later packets in the dictionary until we recieve the ones that are missing
    public Dictionary<int, NetworkPacket> WaitingToProcess = new Dictionary<int, NetworkPacket>();
    private bool WaitingForMissingPackets = false;    //Set when we detect we have missed some packets, once they have been recieved we
    //can then process them, along with any else that have been added into the WaitingToProcess list in order to become caught up again
    public int FirstMissingPacketNumber; //The packet order number of the first missing packet that we are currently waiting for
    public int NewestPacketWaitingToProcess;    //The order number of the newest packet we have received that is waiting for be processed
    //once we have finished receiving all the packets missing in between

    private void Awake()
    {
        //Assign the singleton class instance
        Instance = this;

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

        //System Packet Handlers
        PacketHandlers.Add(ServerPacketType.StillConnectedCheck, SystemPacketHandler.HandleStillConnectedCheck);
        PacketHandlers.Add(ServerPacketType.MissingPacketsRequest, SystemPacketHandler.HandleMissingPacketsRequest);
        PacketHandlers.Add(ServerPacketType.KickedFromServer, SystemPacketHandler.HandleKickedFromServer);
    }

    //Reads in a packet sent from the server and passes it onto whatever handle function that packet type is mapped onto
    public void ReadServerPacket(string PacketMessage)
    {
        //Create a new NetworkPacket object to store the string value that was received from the game server
        NetworkPacket NewPacket = new NetworkPacket(PacketMessage);

        //Before reading the packets data, check the order number to make sure we didnt miss any packets in between
        int NewOrderNumber = NewPacket.ReadInt();
        int ExpectedOrderNumber = ConnectionManager.Instance.LastPacketRecieved + 1;

        //If we ever receive a packet with an order number of -1 then that order number needs to be completely ignored and that packet needs to be processed immediately
        if(NewOrderNumber == -1)
        {
            //Handle the packets data
            HandlePacket(NewPacket);
        }
        //If packets were missed then we need to let the server know, and then just wait until all the missing packets have been recieved
        else if(NewOrderNumber != ExpectedOrderNumber)
        {
            //Add this packet that we have just now received into the WaitingToProcess dictionary, and mark it as being the NewestPacketWaitingToProcess while we wait for the rest
            WaitingToProcess.Add(NewOrderNumber, NewPacket);
            NewestPacketWaitingToProcess = NewOrderNumber;
            WaitingForMissingPackets = true;

            //Send an alert to the server, letting them know which packets need to be resent to us so we can catch up to them
            SystemPacketSender.Instance.SendMissedPacketsRequest(ExpectedOrderNumber);

            //Keep track of the initial order number that we are waiting to receive back from the game server
            FirstMissingPacketNumber = ExpectedOrderNumber;

            //Nothing else needs to be done right now as we need to wait for the missing packets to be sent back to us from the server
            return;
        }
        //If we missed nothing then we process the packet data as normal
        else
        {
            //If we are waiting for missing packets to be resent, check if we have everything needed to catch back up
            if(WaitingForMissingPackets)
            {
                //Make sure we arent trying to add packets into the WaitingToProcess dictionary which are already there
                if(!WaitingToProcess.ContainsKey(NewOrderNumber))
                {
                    //Add this packet into the WaitingToProcess dictionary
                    WaitingToProcess.Add(NewOrderNumber, NewPacket);

                    //Set this as the NewestPacketWaitingToProcess if it has a higher order number than the previous
                    if (NewOrderNumber > NewestPacketWaitingToProcess)
                        NewestPacketWaitingToProcess = NewOrderNumber;

                    //Check if we now have all the missing packets that we were waiting for
                    bool HaveMissingPackets = true;
                    for (int i = FirstMissingPacketNumber; i < NewestPacketWaitingToProcess; i++)
                    {
                        if (!WaitingToProcess.ContainsKey(i))
                        {
                            HaveMissingPackets = false;
                            break;
                        }
                    }

                    //If we now have all the missing packets that we were waiting for, now they can all be processed so we are caught back up to the same state as the game server
                    if (HaveMissingPackets)
                    {
                        //Go through all the packets that need to be processed
                        for (int i = FirstMissingPacketNumber; i < NewestPacketWaitingToProcess; i++)
                        {
                            //Grab each packet from the dictionary
                            NetworkPacket PacketToProcess = WaitingToProcess[i];

                            //Handle the packets data
                            HandlePacket(PacketToProcess);
                        }

                        //All the missing packets have now been processed, reset the dictionary, disable the flag and set the new value for next expected packet number
                        WaitingToProcess = new Dictionary<int, NetworkPacket>();
                        WaitingForMissingPackets = false;
                        ConnectionManager.Instance.LastPacketRecieved = NewestPacketWaitingToProcess;
                    }
                }
            }
            //If we arent waiting for any missing packets, we just process this data as normal
            else
            {
                //Set this number as the last that was recieved from the server
                ConnectionManager.Instance.LastPacketRecieved = NewOrderNumber;

                //Handle the packets data
                HandlePacket(NewPacket);
            }
        }
    }

    //Read all the information from the given packet, passing each section on to its registered handler function
    private void HandlePacket(NetworkPacket NewPacket)
    {
        while(!NewPacket.FinishedReading())
        {
            ServerPacketType PacketType = NewPacket.ReadType();
            if (PacketHandlers.TryGetValue(PacketType, out Packet Packet))
                Packet.Invoke(ref NewPacket);
        }
    }
}
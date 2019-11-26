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

        //Map functions for handling remote players Position/Rotation/Movement updates
        PacketHandlers.Add(ServerPacketType.CharacterPositionUpdate, PlayerManagementPacketHandler.HandlePositionUpdate);
        PacketHandlers.Add(ServerPacketType.CharacterRotationUpdate, PlayerManagementPacketHandler.HandleRotationUpdate);
        PacketHandlers.Add(ServerPacketType.CharacterMovementUpdate, PlayerManagementPacketHandler.HandleMovementUpdate);

        //Map all the player management packet handlers into the dictionary
        PacketHandlers.Add(ServerPacketType.SpawnPlayer, PlayerManagementPacketHandler.HandleSpawnPlayer);
        PacketHandlers.Add(ServerPacketType.RemovePlayer, PlayerManagementPacketHandler.HandleRemovePlayer);

        //Map player communication packet handlers into the dictionary
        PacketHandlers.Add(ServerPacketType.PlayerChatMessage, PlayerCommunicationPacketHandler.HandleChatMessage);
        PacketHandlers.Add(ServerPacketType.PlayerBegin, PlayerManagementPacketHandler.HandlePlayerBegin);

        //Map functions for server telling us to force move characters into new locations
        PacketHandlers.Add(ServerPacketType.ForceCharacterMove, PlayerManagementPacketHandler.HandleForceMovePlayer);
        PacketHandlers.Add(ServerPacketType.ForceOtherCharacterMove, PlayerManagementPacketHandler.HandleForceMoveOtherPlayer);

        PacketHandlers.Add(ServerPacketType.NewNextPacketNumber, SystemPacketHandler.HandleNewNextPacketNumber);
        PacketHandlers.Add(ServerPacketType.StillConnectedCheck, SystemPacketHandler.HandleStillConnectedCheck);
        PacketHandlers.Add(ServerPacketType.ConnectionDeSync, SystemPacketHandler.HandleConnectionDeSync);
        PacketHandlers.Add(ServerPacketType.MissingPacketsReply, SystemPacketHandler.HandleMissingPacketsReply);
        PacketHandlers.Add(ServerPacketType.MissedPackets, SystemPacketHandler.HandleMissingPacketsRequest);
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

        //If packets were missed then we need to let the server know, and then just wait until all the missing packets have been recieved
        if(NewOrderNumber != ExpectedOrderNumber)
        {
            //Add this packet that we have just now received into the WaitingToProcess dictionary, and mark it as being the NewestPacketWaitingToProcess while we wait for the rest
            WaitingToProcess.Add(NewOrderNumber, NewPacket);
            NewestPacketWaitingToProcess = NewOrderNumber;
            WaitingForMissingPackets = true;

            //Send an alert to the server, letting them know which packets need to be resent to us so we can catch up to them
            SystemPacketSender.Instance.SendMissedPacketAlert(ExpectedOrderNumber);

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

                            //Loop through all the data in this packet, passing each section of instructions onto its registered handler function
                            while (!PacketToProcess.FinishedReading())
                            {
                                //Read this packets type identifier
                                ServerPacketType ProcessingPacketType = PacketToProcess.ReadType();

                                //Use this type to invoke the correct handler function
                                if (PacketHandlers.TryGetValue(ProcessingPacketType, out Packet Packet))
                                    Packet.Invoke(ref PacketToProcess);

                            }
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

                //Loop through all of the data in this net packet, passing each section of instructions on to their registered handler function
                while (!NewPacket.FinishedReading())
                {
                    //Read the next packet type value from the remaining packet data
                    ServerPacketType PacketType = NewPacket.ReadType();

                    //Use this enum value to invoke the packet handler function that is registered to this packet type
                    if (PacketHandlers.TryGetValue(PacketType, out Packet Packet))
                        Packet.Invoke(ref NewPacket);
                }
            }
        }
    }
}
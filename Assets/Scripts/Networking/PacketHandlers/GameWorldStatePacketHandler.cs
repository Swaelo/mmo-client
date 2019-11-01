// ================================================================================================================================
// File:        GameWorldStatePacketHandler.cs
// Description:	Handles network packets from the server for loading in the current state of the game world when entering into it
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class GameWorldStatePacketHandler : MonoBehaviour
{
    //Handles loading in list of all other active players before we can enter into the game world
    public static void HandleActivePlayerList(ref NetworkPacket Packet)
    {
        //Read the number of other clients from the packet data
        int OtherClients = Packet.ReadInt();

        //Loop through and read each clients information
        for(int i = 0; i < OtherClients; i++)
        {
            //Read each characters name and location values
            string CharacterName = Packet.ReadString();
            Vector3 CharacterPosition = Packet.ReadVector3();

            //Pass each characters information onto the PlayerManager so it can be spawned into the game world
            PlayerManager.Instance.AddRemotePlayer(CharacterName, CharacterPosition);
        }

        //Note that we have finished loading in the active player list
        GameState.Instance.PlayerListLoaded = true;
    }

    //Handles loading in list of all active entities before we can enter into the game world
    public static void HandleActiveEntityList(ref NetworkPacket Packet)
    {
        //Read the numer of entities from the packet data
        int EntityCount = Packet.ReadInt();

        //Loop through and read and entities information
        for(int i = 0; i < EntityCount; i++)
        {
            //Read each entities type, ID, position and health values
            string EntityType = Packet.ReadString();
            string EntityID = Packet.ReadString();
            Vector3 EntityPosition = Packet.ReadVector3();
            int EntityHealth = Packet.ReadInt();
        }

        //Note that we have finished loading in the active entity list
        GameState.Instance.EntityListLoaded = true;
    }

    //Handles loading in list of all active game item pickups before we can enter into the game world
    public static void HandleActiveItemList(ref NetworkPacket Packet)
    {
        //Read the number of item pickups from the packet data
        int ItemCount = Packet.ReadInt();

        //Loop through and read each items information
        for(int i = 0; i < ItemCount; i++)
        {
            //Read each items Number, ID and Position values
            int ItemNumber = Packet.ReadInt();
            int ItemID = Packet.ReadInt();
            Vector3 ItemPosition = Packet.ReadVector3();
        }

        //Note that we have finished loading in the item pickup list
        GameState.Instance.ItemListLoaded = true;
    }

    //Handles loading in contents of our characters inventory before we can enter into the game world
    public static void HandleInventoryContents(ref NetworkPacket Packet)
    {
        //Read the number of items in our inventory from the packet data
        int ItemCount = Packet.ReadInt();

        //Loop through and read each items information
        for(int i = 0; i < ItemCount; i++)
        {
            //Read each items Number and ID values
            int ItemNumber = Packet.ReadInt();
            int ItemID = Packet.ReadInt();
        }

        //Note that we have finished loading in the characters inventory contents
        GameState.Instance.InventoryLoaded = true;
    }

    //Handles loading in contents of our characters equipment before we can enter into the game world
    public static void HandleEquipmentContents(ref NetworkPacket Packet)
    {
        //Read the number of items equipped on our character
        int ItemCount = Packet.ReadInt();

        //Loop through and read each items information
        for(int i = 0; i < ItemCount; i++)
        {
            //Read each items equipment slot, item number and ID values
            int EquipmentSlot = Packet.ReadInt();
            int ItemNumber = Packet.ReadInt();
            int ItemID = Packet.ReadInt();
        }

        //Note that we have finished loading in the characters equipment contents
        GameState.Instance.EquipmentLoaded = true;
    }

    //Handles loading in contents of our characters action bar before we can enter into the game world
    public static void HandleActionBarContents(ref NetworkPacket Packet)
    {
        //Read the number of abilities socketed onto our action bar
        int AbilityCount = Packet.ReadInt();

        //Loop through and read each items information
        for(int i = 0; i < AbilityCount; i++)
        {
            //Read each abilities item number and ID values
            int ItemNumber = Packet.ReadInt();
            int ItemID = Packet.ReadInt();
        }

        //Note that we have finished loading in the characters action bar contents
        GameState.Instance.AbilitiesLoaded = true;
    }
}

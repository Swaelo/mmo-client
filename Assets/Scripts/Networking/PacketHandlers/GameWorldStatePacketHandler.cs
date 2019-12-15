// ================================================================================================================================
// File:        GameWorldStatePacketHandler.cs
// Description:	Handles network packets from the server for loading in the current state of the game world when entering into it
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class GameWorldStatePacketHandler : MonoBehaviour
{
    public static NetworkPacket GetValuesActivePlayerList(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.ActivePlayerList);
        int ClientCount = ReadFrom.ReadInt();
        Packet.WriteInt(ClientCount);
        for(int i = 0; i < ClientCount; i++)
        {
            Packet.WriteString(ReadFrom.ReadString());
            Packet.WriteBool(ReadFrom.ReadBool());
            Packet.WriteVector3(ReadFrom.ReadVector3());
            Packet.WriteQuaternion(ReadFrom.ReadQuaternion());
            Packet.WriteInt(ReadFrom.ReadInt());
            Packet.WriteInt(ReadFrom.ReadInt());
        }
        return Packet;
    }

    //Handles loading in list of all other active players before we can enter into the game world
    public static void HandleActivePlayerList(ref NetworkPacket Packet)
    {
        Log.In("Active Player List");

        //Read the number of other clients from the packet data
        int OtherClients = Packet.ReadInt();

        //Loop through and read each clients information
        for(int i = 0; i < OtherClients; i++)
        {
            //Read each characters name and location values
            string CharacterName = Packet.ReadString();
            bool CharacterAlive = Packet.ReadBool();
            Vector3 CharacterPosition = Packet.ReadVector3();
            Quaternion CharacterRotation = Packet.ReadQuaternion();
            int CharacterCurrentHP = Packet.ReadInt();
            int CharacterMaxHP = Packet.ReadInt();

            //Pass each characters information onto the PlayerManager so it can be spawned into the game world
            PlayerManager.Instance.AddRemotePlayer(CharacterName, CharacterAlive, CharacterPosition, CharacterRotation, CharacterCurrentHP, CharacterMaxHP);
        }

        //Note that we have finished loading in the active player list
        GameState.Instance.PlayerListLoaded = true;
    }

    public static NetworkPacket GetValuesActiveEntityList(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.ActiveEntityList);
        int EntityCount = ReadFrom.ReadInt();
        Packet.WriteInt(EntityCount);
        for(int i = 0; i < EntityCount; i++)
        {
            Packet.WriteString(ReadFrom.ReadString());
            Packet.WriteString(ReadFrom.ReadString());
            Packet.WriteVector3(ReadFrom.ReadVector3());
            Packet.WriteInt(ReadFrom.ReadInt());
        }
        return Packet;
    }

    //Handles loading in list of all active entities before we can enter into the game world
    public static void HandleActiveEntityList(ref NetworkPacket Packet)
    {
        Log.In("Active Entity List");

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

    public static NetworkPacket GetValuesActiveItemList(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.ActiveItemList);
        int ItemCount = ReadFrom.ReadInt();
        Packet.WriteInt(ItemCount);
        for (int i = 0; i < ItemCount; i++)
        {
            Packet.WriteInt(ReadFrom.ReadInt());
            Packet.WriteInt(ReadFrom.ReadInt());
            Packet.WriteVector3(ReadFrom.ReadVector3());
        }
        return Packet;
    }

    //Handles loading in list of all active game item pickups before we can enter into the game world
    public static void HandleActiveItemList(ref NetworkPacket Packet)
    {
        Log.In("Active Item List");

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

    public static NetworkPacket GetValuesInventoryContents(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.InventoryContents);
        int ItemCount = ReadFrom.ReadInt();
        Packet.WriteInt(ItemCount);
        for (int i = 0; i < ItemCount; i++)
        {
            Packet.WriteInt(ReadFrom.ReadInt());
            Packet.WriteInt(ReadFrom.ReadInt());
        }
        return Packet;
    }

    //Handles loading in contents of our characters inventory before we can enter into the game world
    public static void HandleInventoryContents(ref NetworkPacket Packet)
    {
        Log.In("Inventory Contents");

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

    public static NetworkPacket GetValuesEquippedItems(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.EquippedItems);
        int EquipmentCount = ReadFrom.ReadInt();
        Packet.WriteInt(EquipmentCount);
        for (int i = 0; i < EquipmentCount; i++)
        {
            Packet.WriteInt(ReadFrom.ReadInt());
            Packet.WriteInt(ReadFrom.ReadInt());
            Packet.WriteInt(ReadFrom.ReadInt());
        }
        return Packet;
    }

    //Handles loading in contents of our characters equipment before we can enter into the game world
    public static void HandleEquippedItems(ref NetworkPacket Packet)
    {
        Log.In("Equipment Contents");

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

    public static NetworkPacket GetValuesSocketedAbilities(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.SocketedAbilities);
        int AbilityCount = ReadFrom.ReadInt();
        Packet.WriteInt(AbilityCount);
        for (int i = 0; i < AbilityCount; i++)
        {
            Packet.WriteInt(ReadFrom.ReadInt());
            Packet.WriteInt(ReadFrom.ReadInt());
        }
        return Packet;
    }

    //Handles loading in contents of our characters action bar before we can enter into the game world
    public static void HandleSocketedAbilities(ref NetworkPacket Packet)
    {
        Log.In("Action Bar Contents");

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

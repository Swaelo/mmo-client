// ================================================================================================================================
// File:        AccountManagementPacketSender.cs
// Description:	Functions for sending instructions to the game server regarding user account management (logging in, registering etc)
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class AccountManagementPacketSender : MonoBehaviour
{
    //Singleton Class Instance
    public static AccountManagementPacketSender Instance = null;
    void Awake() { Instance = this; }

    //Sends a request to the game server to login to a user account
    public void SendLoginRequest(string Username, string Password)
    {
        Log.Out("Account Login Request");

        //Create a new NetworkPacket object to store the data for this login request
        NetworkPacket Packet = new NetworkPacket();

        //Fill it with the relevant data values
        Packet.WriteType(ClientPacketType.AccountLoginRequest);
        Packet.WriteString(Username);
        Packet.WriteString(Password);

        //Add it to the outgoing packets queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    //Sends an alert to the game server letting them know we are logging out of our account
    public void SendLogoutAlert()
    {
        Log.Out("Account Logout Alert");

        //Create a new NetworkPacket object to store the data for this logout alert
        NetworkPacket Packet = new NetworkPacket();

        //Fill it with the relevant data
        Packet.WriteType(ClientPacketType.AccountLogoutAlert);

        //Add it to the outgoing packets queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    //Sends a request to the game server to register a new user account
    public void SendRegisterRequest(string Username, string Password)
    {
        Log.Out("Account Registration Request");

        //Create a new NetworkPacket object to store the data for this account registration request
        NetworkPacket Packet = new NetworkPacket();

        //Fill it with the relevant data
        Packet.WriteType(ClientPacketType.AccountRegistrationRequest);
        Packet.WriteString(Username);
        Packet.WriteString(Password);

        //Add it to the outgoing packets queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    //Requests the game server to send us data about all characters that we own
    public void SendCharacterDataRequest()
    {
        Log.Out("Character Data Request");

        //Create a new NetworkPacket object to store the data for this character data request
        NetworkPacket Packet = new NetworkPacket();

        //Fill it with the relevant data
        Packet.WriteType(ClientPacketType.CharacterDataRequest);

        //Add it to the outgoing packets queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    //Sends a request to the game server to create a new player character registered to our user account
    public void SendCreateCharacterRequest(string CharacterName)
    {
        Log.Out("Character Creation Request");

        //Create a new NetworkPacket object to store the data for this new character creation request
        NetworkPacket Packet = new NetworkPacket();

        //Fill it with the relevant data
        Packet.WriteType(ClientPacketType.CharacterCreationRequest);
        Packet.WriteString(CharacterName);

        //Add it to the outgoing packets queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }
}

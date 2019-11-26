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

    /// <summary>
    /// //Sends a request to the game server to login to a user account
    /// </summary>
    /// <param name="Username">Account Username user is trying to log into</param>
    /// <param name="Password">Account Password using is trying to log into</param>
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

    /// <summary>
    /// //Sends a request to the game server to register a new user account
    /// </summary>
    /// <param name="Username">Account Username client is trying to register</param>
    /// <param name="Password">Account Password client is trying to register</param>
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

    /// <summary>
    /// //Requests the game server to send us data about all characters that we own
    /// </summary>
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

    /// <summary>
    /// //Sends a request to the game server to create a new player character registered to our user account
    /// </summary>
    /// <param name="CharacterName">Name of character user is trying to create</param>
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

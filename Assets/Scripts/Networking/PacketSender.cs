// ================================================================================================================================
// File:        PacketSender.cs
// Description:	Used to send messages to the game server
// Author:	Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketSender : MonoBehaviour
{
    //Singleton Class Instance
    public static PacketSender Instance = null;
    void Awake() { Instance = this; }

    //Sends a request to the game server to login to a user account
    public void SendLoginRequest(string Username, string Password)
    {
        string PacketData = ((int)ClientPacketTypes.AccountLoginRequest) + " "
            + Username + " "
            + Password;
        ConnectionManager.Instance.MessageServer(PacketData);
    }

    //Sends an alert to the game server letting them know we are logging out of our account
    public void SendLogoutAlert()
    {
        string PacketData = ((int)ClientPacketTypes.AccountLogoutAlert) + " ";
        ConnectionManager.Instance.MessageServer(PacketData);
    }

    //Sends a request to the game server to register a new user account
    public void SendRegisterRequest(string Username, string Password)
    {
        string PacketData = ((int)ClientPacketTypes.AccountRegistrationRequest) + " "
        + Username + " "
        + Password;
        ConnectionManager.Instance.MessageServer(PacketData);
    }

    //Sends a request for the server to supply all of the users existing characters data
    public void SendCharacterDataRequest()
    {
        string PacketData = ((int)ClientPacketTypes.CharacterDataRequest) + " ";
        ConnectionManager.Instance.MessageServer(PacketData);
    }

    //Sends a request to the server for creating a new player character
    public void SendCreateCharacterRequest(string CharacterName)
    {
        string PacketData = ((int)ClientPacketTypes.CharacterCreationRequest) + " "
            + CharacterName + " ";
        ConnectionManager.Instance.MessageServer(PacketData);
    }

    //Tells the server we are entering into the game world with the selected character
    public void SendEnterWorldRequest(string CharacterName)
    {
        string PacketData = ((int)ClientPacketTypes.EnterWorldRequest) + " "
            + CharacterName + " ";
        ConnectionManager.Instance.MessageServer(PacketData);
    }
}

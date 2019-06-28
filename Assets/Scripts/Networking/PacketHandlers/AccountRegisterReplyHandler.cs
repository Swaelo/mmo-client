// ================================================================================================================================
// File:        AccountRegisterReplyHandler.cs
// Description:	Handles reply message from the server when user tries to register a new account
// Author:	Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountRegisterReplyHandler : MonoBehaviour
{
    public static void HandleAccountRegisterReply(string PacketMessage)
    {
        //Trim the packet type off from the start of the string
        PacketMessage = PacketMessage.Substring(PacketMessage.IndexOf(' ') + 1);
        //Next isolate and trim the registration success flag
        string RegisterSuccessString = PacketMessage.Substring(0, PacketMessage.IndexOf(' '));
        bool RegisterSuccess = RegisterSuccessString == "1" ? true : false;
        //The reply message is what remains
        string ReplyMessage = PacketMessage.Substring(PacketMessage.IndexOf(' ') + 1);

        //Return to the account registration panel if registration was a failure
        if(!RegisterSuccess)
        {
            Log.Chat("Account Creation Failed: " + ReplyMessage);
            InterfaceManager.Instance.SetObjectActive("Registering Panel", false);
            InterfaceManager.Instance.SetObjectActive("Account Register Panel", true);
        }
        //Otherwise we send a character data request to the server, and wait for that before
        //continuing on to the character select screen
        else
        {
            Log.Chat("Account Created!");
            InterfaceManager.Instance.SetObjectActive("Registering Panel", false);
            InterfaceManager.Instance.SetObjectActive("Loading Characters Panel", true);
        }
    }
}

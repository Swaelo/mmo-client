// ================================================================================================================================
// File:        AccountLoginReplyHandler.cs
// Description:	Handles reply message from server when user tries to login to their account
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountLoginReplyHandler : MonoBehaviour
{
    public static void HandleAccountLoginReply(string PacketMessage)
    {
        //Trim the packet type off from the start of the string
        PacketMessage = PacketMessage.Substring(PacketMessage.IndexOf(' ') + 1);
        //Next isolate and trim the account login success flag
        string LoginSuccessString = PacketMessage.Substring(0, PacketMessage.IndexOf(' '));
        bool LoginSuccess = LoginSuccessString == "1" ? true : false;
        //The reply message is what remains
        string ReplyMessage = PacketMessage.Substring(PacketMessage.IndexOf(' ') + 1);

        //Return to the account login panel if the login was a failure
        if(!LoginSuccess)
        {
            Log.Chat("Account Login Failed: " + ReplyMessage);
            InterfaceManager.Instance.SetObjectActive("Logging In Panel", false);
            InterfaceManager.Instance.SetObjectActive("Account Login Panel", true);
        }
        //Otherwise send a request for all the characters data, then wait for that before continuing on to the the character select screen
        else
        {
            Log.Chat("Logged in!");
            InterfaceManager.Instance.SetObjectActive("Logging In Panel", false);
            InterfaceManager.Instance.SetObjectActive("Loading Characters Panel", true);
            //Sends a request to the server for all of our characters information
            PacketSender.Instance.SendCharacterDataRequest();
        }
    }
}

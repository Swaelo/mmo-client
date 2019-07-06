// ================================================================================================================================
// File:        CreateCharacterReplyHandler.cs
// Description:	Handles a new character creation reply from the game server
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCharacterReplyHandler : MonoBehaviour
{
    public static void HandleCreateCharacterReply(string PacketMessage)
    {
        //Trim the packet type off from the start of the string
        PacketMessage = PacketMessage.Substring(PacketMessage.IndexOf(' ') + 1);

        //Next isolate and trim the creation success flag
        string CreationSuccessString = PacketMessage.Substring(0, PacketMessage.IndexOf(' '));
        bool CreationSuccess = CreationSuccessString == "1";

        //What remains is the reply message
        string ReplyMessage = PacketMessage.Substring(PacketMessage.IndexOf(' ') + 1);

        //If the creation was a success, then we should request all the characters data before proceeding to the character select screen
        if(CreationSuccess)
        {
            Log.Chat("New Character Created!");
            InterfaceManager.Instance.SetObjectActive("Creating Character Panel", false);
            InterfaceManager.Instance.SetObjectActive("Loading Characters Panel", true);
            PacketSender.Instance.SendCharacterDataRequest();
        }
        //Otherwise we should return to the character creation screen
        else
        {
            Log.Chat("Character Creation Failed: " + ReplyMessage);
            InterfaceManager.Instance.SetObjectActive("Creating Character Panel", false);
            InterfaceManager.Instance.SetObjectActive("Character Create Panel", true);
        }
    }
}

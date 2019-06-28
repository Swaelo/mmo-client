// ================================================================================================================================
// File:        CharacterDataReplyHandler.cs
// Description:	Handles reply message from the server when its providing us with all our character data
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDataReplyHandler : MonoBehaviour
{
    public static void HandleCharacterDataReply(string PacketMessage)
    {
        //Trim the packet type off from the start of the string
        PacketMessage = PacketMessage.Substring(PacketMessage.IndexOf(' ') + 1);

        //Next isolate and trim the character count value
        string CharacterCountString = PacketMessage.Substring(0, PacketMessage.IndexOf(' '));
        int CharacterCount = Int32.Parse(CharacterCountString);
        PacketMessage = PacketMessage.Substring(PacketMessage.IndexOf(' ') + 1);

        //If there are 0 characters existing in this users account then we simply proceed to the character select screen
        if(CharacterCount == 0)
        {
            Log.Chat("No characters to load.");
            InterfaceManager.Instance.SetObjectActive("Loading Characters Panel", false);
            InterfaceManager.Instance.SetObjectActive("Character Select Panel", true);
            return;
        }

        //Otherwise we need to loop through and extract each characters information from the packet and use that to update
        //the character select screen as we progress on to it
        for(int i = 0; i < CharacterCount; i++)
        {
            Log.Chat("Loading character #" + (i+1) + " character data...");
            
        }
    }
}

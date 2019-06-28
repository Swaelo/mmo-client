// ================================================================================================================================
// File:        ChatMessageHandler.cs
// Description:	
// Author:	Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChatMessageHandler
{
    public static void HandleChatMessage(int ClientID, string PacketMessage)
    {
        //Isolate the author and message from one another
        Debug.Log("Handle Chat Message: " + PacketMessage);
    }
}

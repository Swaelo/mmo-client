// ================================================================================================================================
// File:        Log.cs
// Description: Used to easily add messages to the chat window
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using UnityEngine;

public static class Log
{
    public static void Chat(string Message, bool PrintConsole = true)
    {
        ChatWindowManager.Instance.DisplayMessage(Message);
        
        if(PrintConsole)
            Debug.Log(Message);
    }

    public static void Chat(string Message, byte[] Data)
    {
        string Time = DateTime.Now.ToString("h:mm:ss");
        string FinalMessage = Time + ": " + Message + ":";
        for (int i = 0; i < Data.Length; i++)
            FinalMessage += Data[i].ToString();
        ChatWindowManager.Instance.DisplayMessage(FinalMessage);
    }

    //Displays a message showing that a certain packet was sent to the game server
    public static void Out(string Message)
    {
        if (!DebugSettings.Instance.LogOutgoingPackets)
            return;
            
        ChatWindowManager.Instance.DisplayMessage("PacketOut: " + Message);
        Debug.Log("PacketOut: " + Message);
    }

    //Displays a message showing the a certain packet was received from the game server
    public static void In(string Message)
    {
        if (!DebugSettings.Instance.LogIncomingPackets)
            return;
            
        ChatWindowManager.Instance.DisplayMessage("PacketIn: " + Message);
        Debug.Log("PacketIn: " + Message);
    }
}

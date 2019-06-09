// ================================================================================================================================
// File:        Log.cs
// Description: Used to easily add messages to the chat window
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Log
{
    public static void Chat(string Message)
    {
        ChatWindowManager.Instance.DisplayMessage(Message);
    }

    public static void Chat(string Message, byte[] Data)
    {
        string FinalMessage = Message + ":";
        for (int i = 0; i < Data.Length; i++)
            FinalMessage += Data[i].ToString();
        ChatWindowManager.Instance.DisplayMessage(FinalMessage);
    }
}

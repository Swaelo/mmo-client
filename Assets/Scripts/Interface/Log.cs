// ================================================================================================================================
// File:        Log.cs
// Description: Used to easily add messages to the chat window
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
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
        string Time = DateTime.Now.ToString("h:mm:ss");
        string FinalMessage = Time + ": " + Message + ":";
        for (int i = 0; i < Data.Length; i++)
            FinalMessage += Data[i].ToString();
        ChatWindowManager.Instance.DisplayMessage(FinalMessage);
    }
}

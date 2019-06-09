// ================================================================================================================================
// File:        ChatWindowManager.cs
// Description: Displays messages to the chat window interface
// ================================================================================================================================

using System;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindowManager : MonoBehaviour
{
    //Singleton class for easy access throughout the entire project
    public static ChatWindowManager Instance = null;

    public GameObject[] MessageLines;
    private string[] MessageContents;

    private void Awake()
    {
        Instance = this;
        MessageContents = new string[MessageLines.Length];
        for (int i = 0; i < MessageLines.Length; i++)
        {
            MessageContents[i] = "";
            MessageLines[i].GetComponent<Text>().text = MessageContents[i];
        }
    }

    public void DisplayMessage(string Message)
    {
        //Move all the previous messages back 1 line
        for (int i = MessageLines.Length - 1; i > 0; i--)
        {
            MessageContents[i] = MessageContents[i - 1];
            MessageLines[i].GetComponent<Text>().text = MessageContents[i];
        }

        //Place the new message on the first line
        MessageContents[0] = Message;
        MessageLines[0].GetComponent<Text>().text = MessageContents[0];
    }
}
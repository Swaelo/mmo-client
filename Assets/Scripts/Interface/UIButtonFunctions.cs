using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonFunctions : MonoBehaviour
{
    public void ClickConnectButton()
    {
        string ServerAddress = GameObject.Find("Server Address Input Field").GetComponent<InputField>().text;
        Log.Chat("Connecting to: " + ServerAddress + "...");
        ConnectionManager ServerConnection = GameObject.Find("System").GetComponent<ConnectionManager>();
        ServerConnection.InitializeWebSocket(ServerAddress);
        ServerConnection.TryConnect();
    }

    public void SendServerMessage()
    {
        string ServerMessage = GameObject.Find("Server Message Input Field").GetComponent<InputField>().text;
        ConnectionManager ServerConnection = GameObject.Find("System").GetComponent<ConnectionManager>();
        byte[] ServerMessageData = Encoding.UTF8.GetBytes(ServerMessage);
        Log.Chat("To server: " + ServerMessage, ServerMessageData);
        ServerConnection.SendMessage(ServerMessage);
    }
}

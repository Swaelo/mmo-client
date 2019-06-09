// ================================================================================================================================
// File:        ConnectionManager.cs
// Description: 
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HybridWebSocket;

public class ConnectionManager : MonoBehaviour
{
    //Current connection to the game server
    public string ConnectionAddress = "ws://echo.websocket.org";
    public WebSocket ServerConnection;

    //These flags will be set by the HybridWebSocket class, its a bit weird in there so we will just monitor these values and act when they change
    public static bool Connected = false;       //Tracks whether the connection the server is open or not
    public static bool ConnectionEstablished = false;//Set once a new connection to the server has been established
    public static bool MessageReceived = false; //Set whenever a new message has been received from the game server
    public static string ServerMessage = "";    //When a message is received from the game server its stored here ready to be processed
    public static bool ConnectionError = false; //Set when some error occurs with the connection to the server, connection lost etc.
    public static string ErrorMessage = "";     //Error message whenever a connection error occurs
    public static bool ConnectionClosed = false;//Set when we are finished communicating with the server and the connection is closed
    public static string CloseCode = "";        //Set when the connection to the server is shut down


    public void InitializeWebSocket(string ServerAddress)
    {
        //Initialize the web socket and register all the events
        ConnectionAddress = ServerAddress;
        ServerConnection = WebSocketFactory.CreateInstance(ConnectionAddress);

        //Note that the connection is opened, then immediately send a message to the server saying hello
        ServerConnection.OnOpen += () =>
        {
            ConnectionEstablished = true;
            //ServerConnection.Send(Encoding.UTF8.GetBytes("Hello from Unity 3D!"));
        };

        //Store any messages received from the server and note that they need to be processed
        ServerConnection.OnMessage += (byte[] msg) =>
        {
            MessageReceived = true;
            ServerMessage = Encoding.UTF8.GetString(msg);
        };

        //Store the error message whenever connection errors occur
        ServerConnection.OnError += (string errMsg) =>
        {
            ConnectionError = true;
            ErrorMessage = errMsg;
        };

        //Store the close code once the server connection is shut down
        ServerConnection.OnClose += (WebSocketCloseCode code) =>
        {
            ConnectionClosed = true;
            CloseCode = code.ToString();
        };
    }

    public void SendMessage(string Message)
    {
        ServerConnection.Send(Encoding.UTF8.GetBytes(Message));
    }

    public void TryConnect()
    {
        ServerConnection.Connect();
    }

    //While the game is running we need to monitor all the flag values so we can react accordingly when new events occur
    private void Update()
    {
        //Announce a succesful connection once it has been initially established
        if(ConnectionEstablished && !Connected)
        {
            ChatWindowManager.Instance.DisplayMessage("Connected at " + ConnectionAddress);
            Connected = true;
        }

        //Announce any messages which are sent to us from the game server
        if(MessageReceived)
        {
            ChatWindowManager.Instance.DisplayMessage("Server: " + ServerMessage);
            MessageReceived = false;
        }

        //Announce any connection errors that may occur
        if(ConnectionError)
        {
            ChatWindowManager.Instance.DisplayMessage("Connection Error: " + ErrorMessage);
            ConnectionError = false;
            Connected = false;
        }

        //Announce once the connection to the server has been closed down
        if(ConnectionClosed)
        {
            ChatWindowManager.Instance.DisplayMessage("Connection Closed: " + CloseCode);
            ConnectionClosed = false;
            Connected = false;
        }
    }
}

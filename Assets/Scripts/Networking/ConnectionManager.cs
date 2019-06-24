// ================================================================================================================================
// File:        ConnectionManager.cs
// Description: Manages the game clients current connection to the game server, performs communication with the server at request
// ================================================================================================================================

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using HybridWebSocket;

public class ConnectionManager : MonoBehaviour
{
    public GameObject ConnectionAnimationObject;
    public ChatMessageInput ChatInput;
    private bool TryingToConnect = false;

    private float ConnectionTimeoutLimit = 3.0f;    //How long to wait before retrying new connection attempt
    private float ConnectionTimeout;

    //Give this class a singleton instance so it can easily be access anywhere in the code
    public static ConnectionManager Instance = null;

    //Current connection to the game server
    private string ConnectionAddress = "ws://203.221.43.175:5500";
    public WebSocket ServerConnection;

    //These flags will be set by the HybridWebSocket class, its a bit weird in there so we will just monitor these values and act when they change
    public static bool Connected = false;       //Tracks whether the connection the server is open or not
    public static bool ConnectionEstablished = false;//Set once a new connection to the server has been established
    public static bool MessageReceived = false; //Set whenever a new message has been received from the game server
    //public static string ServerMessage = "";    //When a message is received from the game server its stored here ready to be processed
    public static byte[] ServerMessage = null;
    public static bool ConnectionError = false; //Set when some error occurs with the connection to the server, connection lost etc.
    public static string ErrorMessage = "";     //Error message whenever a connection error occurs
    public static bool ConnectionClosed = false;//Set when we are finished communicating with the server and the connection is closed
    public static string CloseCode = "";        //Set when the connection to the server is shut down
    
    private void Awake()
    {
        Instance = this;
        ConnectionAnimationObject.SetActive(false);
    }

    public void TryConnect()
    {
        ConnectionTimeout = ConnectionTimeoutLimit;
        ConnectionAnimationObject.SetActive(true);
        TryingToConnect = true;
        InitializeWebSocket(ConnectionAddress);
    }

    //While the game is running we need to monitor all the flag values so we can react accordingly when new events occur
    private void Update()
    {
        //Countdown connection timeout while trying to establish a new connection
        if(TryingToConnect && !ConnectionEstablished && !Connected)
        {
            TryingToConnect = false;
            ConnectionTimeout -= Time.deltaTime;
            //Print an error message and retry the connection again whenever it times out
            if(ConnectionTimeout <= 0.0f)
            {
                ConnectionTimeout = ConnectionTimeoutLimit;
                InitializeWebSocket(ConnectionAddress);
            }
        }

        //Announce a succesful connection once it has been initially established
        if(ConnectionEstablished && !Connected)
        {
            //Disable the connecting animation and enable chat message input
            ConnectionAnimationObject.SetActive(false);
            ChatInput.ChatInputEnabled = true;

            ChatWindowManager.Instance.DisplayMessage("Connected at " + ConnectionAddress);
            Debug.Log("Connected at " + ConnectionAddress);
            Connected = true;
        }

        //Announce any messages which are sent to us from the game server
        if(MessageReceived)
        {
            ChatWindowManager.Instance.DisplayMessage("Server: " + Encoding.UTF8.GetString(ServerMessage));
            Debug.Log("Server: " + ServerMessage);
            MessageReceived = false;
        }

        //Announce any connection errors that may occur
        if(ConnectionError)
        {
            ChatWindowManager.Instance.DisplayMessage("Connection Error: " + ErrorMessage);
            Debug.Log("Connection Error: " + ErrorMessage);
            ConnectionError = false;
            Connected = false;
        }

        //Announce once the connection to the server has been closed down
        if(ConnectionClosed)
        {
            ChatWindowManager.Instance.DisplayMessage("Connection Closed: " + CloseCode);
            Debug.Log("Connection Closed: " + CloseCode);
            ConnectionClosed = false;
            Connected = false;
        }
    }

    //Configures the web socket connection, registers all the event callbacks then tries to connect to the game server
    public void InitializeWebSocket(string ServerAddress)
    {
        //Initialize the web socket and register all the events
        ConnectionAddress = ServerAddress;
        ServerConnection = WebSocketFactory.CreateInstance(ConnectionAddress);

        //Note that the connection is opened, then immediately send a message to the server saying hello
        ServerConnection.OnOpen += () =>
        {
            ConnectionEstablished = true;
        };

        //Store any messages received from the server and note that they need to be processed
        ServerConnection.OnMessage += (byte[] msg) =>
        {
            MessageReceived = true;
            ServerMessage = msg;
            //ServerMessage = Encoding.UTF8.GetString(msg);
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

        //Try opening a connection to the game server
        ServerConnection.Connect();
    }

    //Sends a message to the game server
    public void SendMessage(string Message)
    {
        Debug.Log("sending message to the game server: " + Message);
        if (Connected)
            ServerConnection.Send(Encoding.UTF8.GetBytes(Message));
    }
}

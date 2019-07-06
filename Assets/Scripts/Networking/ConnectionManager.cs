// ================================================================================================================================
// File:        ConnectionManager.cs
// Description: Manages the game clients current connection to the game server, performs communication with the server at request
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Text;
using UnityEngine;
using HybridWebSocket;

public class ConnectionManager : MonoBehaviour
{
    //Singleton object
    public static ConnectionManager Instance = null;
    void Awake() { Instance = this; }

    //Server connection status
    private string ServerIP = "ws://203.221.43.175:5500";
    public WebSocket ServerConnection;
    private bool TryingToConnect = false;

    //These flags are setin within the WebSocket networking events which are registered into the ServerConnection object
    static bool IsConnected = false;    //Tracks whether the connection the server is open or not
    static bool ConnectionEstablished = false;  //Set once a new connection to the server has been established
    static bool MessageReceived = false;    //Set whenever a new message has been received from the game server
    static byte[] ServerMessage = null; //When a message is received from the game server its stored here ready to be processed
    static bool ConnectionError = false;    //Set when some error occurs with the connection to the server, connection lost etc.
    static string ErrorMessage = "";    //Error message whenever a connection error occurs
    static bool ConnectionClosed = false;   //Set when we are finished communicating with the server and the connection is closed
    static string CloseCode = "";   //Set when the connection to the server is shut down    

    //Connection attempt timeout
    private float ConnectionTimeoutLimit = 3.0f;    //How long to wait before retrying the the server connection
    private float ConnectionTimeoutRemaining;       //How long left for the current server connection attempt
    private int FailedConnectionAttempts = 0;       //The client will stop trying to connect after 5 failed attempts
    private int ConnectionAttemptLimit = 3;         //How many times the client will try connecting to the server before it gives up

    //Account information once an account has been logged in to
    public string AccountName = "";
    public string FirstCharacterName = "";
    public string SecondCharacterName = "";
    public string ThirdCharacterName = "";
    public string SelectedCharacter = "";

    void Start()
    {
        //Register all the websocket networking events
        RegisterWebSocketEvents();

        //Try connecting to the game server
        Log.Chat("Connecting to the server...");
        InterfaceManager.Instance.SetObjectActive("Connecting Animation", true);
        ConnectionTimeoutRemaining = ConnectionTimeoutLimit;
        TryingToConnect = true;
        ServerConnection.Connect();
    }

    private void RegisterWebSocketEvents()
    {
        //Initialize a new instance of the websocket class
        ServerConnection = WebSocketFactory.CreateInstance(ServerIP);

        //Register new connection opened event
        ServerConnection.OnOpen += () =>
        {
            TryingToConnect = false;
            ConnectionEstablished = true;
        };

        //Register message received event
        ServerConnection.OnMessage += (byte[] Message) =>
        {
            MessageReceived = true;
            ServerMessage = Message;
        };

        //Register connection error event
        ServerConnection.OnError += (string Error) =>
        {
            ConnectionError = true;
            ErrorMessage = Error;
        };

        //Register connection closed event
        ServerConnection.OnClose += (WebSocketCloseCode Code) =>
        {
            ConnectionClosed = true;
            CloseCode = Code.ToString();
        };
    }    

    void Update()
    {
        //Keep trying to connect to the server until a connection is opened
        if(TryingToConnect)
            TryConnecting();
        
        //Check if a new connection to the server has just been opened
        if(ConnectionEstablished && !IsConnected)
        {
            //Announce the new connection to the chat and hide the connecting animation object
            Log.Chat("Connected!");
            InterfaceManager.Instance.SetObjectActive("Connecting Animation", false);
            IsConnected = true;
            //Enable the main menu panel
            InterfaceManager.Instance.SetObjectActive("Main Menu Panel", true);
        }

        //Handle new messages whenever they are received from the game server
        if(MessageReceived)
        {
            //Pass the message onto the packet handler so the correct handler function can be used
            PacketHandler.Instance.ReadClientPacket(Encoding.ASCII.GetString(ServerMessage));
            MessageReceived = false;
        }

        //Handle connection errors when they occur
        if(ConnectionError)
        {
            Debug.Log("Connection Error: " + ErrorMessage);
            ConnectionError = false;
            IsConnected = false;
        }

        //Handle when the network connection has been closed
        if(ConnectionClosed)
        {
            Debug.Log("Connection Closed: " + CloseCode);
            ConnectionClosed = false;
            IsConnected = false;
        }
    }

    //Keeps trying to establish a connection with the game server
    private void TryConnecting()
    {
        //Count down the timer until this attempt has failed
        ConnectionTimeoutRemaining -= Time.deltaTime;

        //Reset the timer once it reaches zero
        if(ConnectionTimeoutRemaining <= 0f)
        {
            //Reset the timer
            ConnectionTimeoutRemaining = ConnectionTimeoutLimit;
            //Increase the failed attempts counter
            FailedConnectionAttempts++;
            //If we have reached the connection attempt limit then we will no longer keep trying
            if(FailedConnectionAttempts == ConnectionAttemptLimit)
            {
                //Announce to the user the connection attempt has been reached
                Log.Chat("Connection timed out, limit reached. Is the server down?");
                //Stop trying to connect, hide the connecting animation object
                TryingToConnect = false;
                InterfaceManager.Instance.SetObjectActive("Connecting Animation", false);
                return;
            }
            //Otherwise we start a new connection attempt as normal
            else
            {
                //Announce and start a new connection attempt
                Log.Chat("Connection timed out, trying again...");
                ServerConnection.Connect();
            }
        }
    }
    
    //Sends a message to the game server
    public void MessageServer(string Message)
    {
        //Dont try sending anything if we arent connected to the server right now
        if(IsConnected) 
            ServerConnection.Send(Encoding.UTF8.GetBytes(Message));
    }
}

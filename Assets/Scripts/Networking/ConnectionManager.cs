// ================================================================================================================================
// File:        ConnectionManager.cs
// Description: Manages the game clients current connection to the game server, performs communication with the server at request
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour
{
    //Singleton object
    public static ConnectionManager Instance = null;

    //Store a list of packets queued up to be sent out to the server in the next communication interval
    public PacketQueue PacketQueue = new PacketQueue();

    //Server connection status
    public Boolean UseDebugServer = false;
    private string ReleaseServerIP = "ws://203.221.43.175:5500";
    private string DebugServerIP = "ws://203.221.43.175:5501";
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

    //Track how long we have been waiting to establish a connection to the server until it eventually times out
    private float ConnectionTimeout = 10.0f;    //How long to wait for the connection to go through before announcing the servers are probably down
    private bool TimedOut = false;  //Flag set once we have announced that the connection to the server has timed out

    private bool TransmitPackets = true;

    void Awake()
    {
        //Assign our singleton class instance
        Instance = this;
    }

    void Start()
    {
        //Register all the websocket networking events
        RegisterWebSocketEvents();

        //Try connecting to the game server
        Log.Chat("Connecting to the server...");
        InterfaceManager.Instance.SetObjectActive("Connecting Animation", true);
        TryingToConnect = true;
        ServerConnection.Connect();
    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.H))
        //{
        //    TransmitPackets = !TransmitPackets;
        //    Log.Chat("Packet Transmission " + (TransmitPackets ? "Enabled" : "Disabled"));
        //}

        //Wait for server connection to be established until it times out
        if (TryingToConnect)
            TryConnecting();
        //Setup the server connection when it connects successfully
        else if (ConnectionEstablished && !IsConnected)
            SetupNewConnection();
        //Handle Packets from server, and send out our packets in intervals when the connection is open
        else
        {
            //Process instructions sent from the game server
            HandleEvents();
            //Send out any queued packets each timestep that passes
            PacketQueue.UpdateQueue(TransmitPackets);
        }
    }

    private void RegisterWebSocketEvents()
    {
        //Initialize a new instance of the websocket class passing in whatever IP we are found to be using this time
        string ServerIP = UseDebugServer ? DebugServerIP : ReleaseServerIP;
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

    //Handles any WebSocket events that occur while connected to the game server
    private void HandleEvents()
    {
        if(MessageReceived)
            HandleMessage();
        if(ConnectionError)
            HandleConnectionError();
        if(ConnectionClosed)
            HandleClosedConnection();
    }

    //Handles messages received from the game server
    private void HandleMessage()
    {
        //Convert the packet data to string format
        string PacketData = Encoding.ASCII.GetString(ServerMessage);

        //Pass the data onto the packet handler so it can read each section of the data, passing each onto the correct handler function
        PacketHandler.Instance.ReadServerPacket(PacketData);

        //The message has now been handled
        ServerMessage = null;
        MessageReceived = false;
    }

    //Handles connection errors when they occur
    private void HandleConnectionError()
    {
        Debug.Log("Connection Error: " + ErrorMessage);
        ConnectionError = false;
        IsConnected = false;
        //Whenever connection errors occur we want to swap into the Disconnected scene
        SceneManager.LoadScene("Disconnected");
    }

    //Handles when the connection with the game server has been shut down
    private void HandleClosedConnection()
    {
        Debug.Log("Connection Closed: " + CloseCode);
        ConnectionClosed = false;
        IsConnected = false;
        //Whenever connection errors occur we want to swap into the Disconnected scene
        SceneManager.LoadScene("Disconnected");
    }

    //Sets up the connection to the server once it has first been opened
    private void SetupNewConnection()
    {
        //Announce the new connection to the chat and hide the connecting animation object
        Log.Chat("Connected!");
        InterfaceManager.Instance.SetObjectActive("Connecting Animation", false);
        IsConnected = true;
        //Enable the main menu panel
        InterfaceManager.Instance.SetObjectActive("Main Menu Panel", true);
    }

    //Keeps trying to establish a connection with the game server
    private void TryConnecting()
    {
        //Count down the timer until the connection attempt times out
        ConnectionTimeout -= Time.deltaTime;

        //When the connection times out announce that the servers are probably offline
        if(ConnectionTimeout < 0.0f && !TimedOut)
        {
            //Announce that the server connection has timed out and set the TimedOut flag
            Log.Chat("Having trouble connecting, are the servers down?");
            TimedOut = true;
        }
    }
}
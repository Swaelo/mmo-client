// ================================================================================================================================
// File:        ExceptionFreeHybridWebSocket.cs
// Description:	Reimplementation of Jiri Hybeks HybridWebSocket class without any exceptions being called
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

//Handler for WebSocket Open event
public delegate void WebSocketOpenEventHandler();
//Handler for message received from WebSocket
public delegate void WebSocketMessageEventHandler(byte[] Data);
//Handler for an error event received from WebSocket
public delegate void WebSocketErrorEventHandler(string ErrorMessage);
//Handler for WebSocket Close event
public delegate void WebSocketCloseEventHandler(WebSocketCloseCode CloseCode);

//Enum representing WebSocket connection state
public enum WebSocketState
{
    Connecting,
    Open,
    Closing,
    Closed
}

//WebSocket close cosed
public enum WebSocketCloseCode
{
    NotSet = 0,
    Normal = 1000,
    Away = 1001,
    ProtocolError = 1002,
    UnsupportedData = 1003,
    Undefined = 1004,
    NoStatus = 1005,
    Abnormal = 1006,
    InvalidData = 1007,
    PolicyViolation = 1008,
    TooBig = 1009,
    MandatoryExtension = 1010,
    ServerError = 1011,
    TlsHandshakeFailure = 1015
}

//WebSocket class interface shared by both native and JSLIB implementation
public interface IWebSocket
{
    //Open WebSocket connection
    void Connect();

    //Close WebSocket connection with optional status code and reason
    void Close(WebSocketCloseCode CloseCode = WebSocketCloseCode.Normal, string Reason = null);

    //Send binary data over the socket
    void Send(byte[] Data);

    //Return WebSocket conneection state
    WebSocketState GetState();

    //Occurs when the connection is opened
    event WebSocketOpenEventHandler OnOpen;
    //Occurs when a message is received
    event WebSocketMessageEventHandler OnMessage;
    //Occurs when an error was reported from WebSocket
    event WebSocketErrorEventHandler OnError;
    //Occurs when the socket was closed
    event WebSocketCloseEventHandler OnClose;
}

//Various helpers to work mainly with enums and exceptions
public static class WebSocketHelpers
{
    //Safely parse close code enum from int value
    public static WebSocketCloseCode ParseCloseCodeEnum(int CloseCode)
    {
        if(WebSocketCloseCode.IsDefined(typeof(WebSocketCloseCode), CloseCode))
            return (WebSocketCloseCode)CloseCode;
        else
            return WebSocketCloseCode.Undefined;
    }
}

#if UNITY_WEBGL && !UNITY_EDITOR
//WebSocket class bound to JSLIB
public class WebSocket : IWebSocket
{
    //WebSocket JSLIB functions
    [DllImport("__Internal")]
    public static extern int WebSocketConnect(int InstanceID);

    [DllImport("__Internal")]
    public static extern int WebSocketClose(int InstanceID, int Code, string Reason);

    [DllImport("__Internal")]
    public static extern int WebSocketSend(int InstanceID, byte[] DataPtr, int DataLength);

    [DllImport("__Internal")]
    public static extern int WebSocketGetState(int InstanceID);

    //The instance identifier
    protected int InstanceID;

    //Occurs when the connection is opened
    public event WebSocketOpenEventHandler OnOpen;
    //Occurs when a message is received
    public event WebSocketMessageEventHandler OnMessage;
    //Occurs when an error was reported from WebSocket
    public event WebSocketErrorEventHandler OnError;
    //Occurs when the socket was closed
    public event WebSocketCloseEventHandler OnClose;

    //Constructor - receive JSLIB instance ID of allocated socket
    public WebSocket(int InstanceID) { this.InstanceID = InstanceID; }

    //Destructor - notifies WebSocketFactor about it to remove JSLIB references
    //Releases unmanaged resources and performs other cleanup operations before
    //the WebSocket is reclaimed by garbage collection
    ~WebSocket() { WebSocketFactory.HandleInstanceDestroy(this.InstanceID); }

    //Return JSLIB instance ID
    public int GetIntanceID() { return this.InstanceID; }

    //Open WebSocket connection
    public void Connect() { WebSocketConnect(this.InstanceID); }

    //Close WebSocket connection with optional status code and reason
    public void Close(WebSocketCloseCode Code = WebSocketCloseCode.Normal, string Reason = null)
    { WebSocketClose(this.InstanceID, (int)Code, Reason); }

    //Send binary data over the socket
    public void Send(byte[] Data) { WebSocketSend(this.InstanceID, Data, Data.Length); }

    //Return WebSocket connection state
    public WebSocketState GetState()
    {
        int State = WebSocketGetState(this.InstanceID);

        switch(State)
        {
            case 0:
                return WebSocketState.Connecting;
            case 1:
                return WebSocketState.Open;
            case 2:
                return WebSocketState.Closing;
            case 3:
                return WebSocketState.Closed;
            default:
                return WebSocketState.Closed;
        }
    }

    //Delegates OnOpen event from JSLIB to native sharp event
    public void DelegateOnOpenEvent() { this.OnOpen?.Invoke(); }
    //Delegates OnMessage event from JSLIB to native sharp event
    public void DelegateOnMessageEvent(byte[] Data) { this.OnMessage?.Invoke(Data); }
    //Delegates OnError event from JSLIB to native sharp event
    public void DelegateOnErrorEvent(string ErrorMsg) { this.OnError?.Invoke(ErrorMsg); }
    //Delegate OnClose event from JSLIB to native sharp event
    public void DelegateOnCloseEvent(int CloseCode) { this.OnClose?.Invoke(WebSocketHelpers.ParseCloseCodeEnum(CloseCode)); }
}
#else
public class WebSocket : IWebSocket
{
    //Occurs when the connection is opened
    public event WebSocketOpenEventHandler OnOpen;
    //Occurs when a message is recieved
    public event WebSocketMessageEventHandler OnMessage;
    //Occurs when an error was reported from WebSocket
    public event WebSocketErrorEventHandler OnError;
    //Occurs when the socket was closed
    public event WebSocketCloseEventHandler OnClose;

    //The WebSocketSharp instance
    protected WebSocketSharp.WebSocket WS;

    //WebSocket constructor
    public WebSocket(string URL)
    {
        //Create WebSocket instance
        this.WS = new WebSocketSharp.WebSocket(URL);

        //Bind OnOpen event
        this.WS.OnOpen += (Sender, EV) => { this.OnOpen?.Invoke(); };
        //Bind OnMessage event
        this.WS.OnMessage += (Sender, EV) =>
        {
            if(EV.RawData != null)
                this.OnMessage?.Invoke(EV.RawData);
        };
        //Bind OnError event
        this.WS.OnError += (Sender, EV) => { this.OnError?.Invoke(EV.Message); };
        //Bind OnClose event
        this.WS.OnClose += (Sender, EV) => { this.OnClose?.Invoke(WebSocketHelpers.ParseCloseCodeEnum((int)EV.Code)); };
    }
    
    //Open WebSocket connection
    public void Connect() { this.WS.ConnectAsync(); }

    //Close WebSocket connection with optional status code and reason
    public void Close(WebSocketCloseCode Code = WebSocketCloseCode.Normal, string Reason = null)
    { this.WS.CloseAsync((ushort)Code, Reason); }

    //Send binary data over the socket
    public void Send(byte[] Data) { this.WS.Send(Data); }

    //Return WebSocket connection state
    public WebSocketState GetState()
    {
        switch(this.WS.ReadyState)
        {
            case WebSocketSharp.WebSocketState.Connecting:
                return WebSocketState.Connecting;
            case WebSocketSharp.WebSocketState.Open:
                return WebSocketState.Open;
            case WebSocketSharp.WebSocketState.Closing:
                return WebSocketState.Closing;
            case WebSocketSharp.WebSocketState.Closed:
                return WebSocketState.Closed;
            default:
                return WebSocketState.Closed;
        }
    }
}
#endif

//Class providing static access methods to work with JSLIB WebSocket or WebSocketSharp interface
public static class WebSocketFactory
{
#if UNITY_WEBGL && !UNITY_EDITOR
    //Map of WebSocket instances
    private static Dictionary<Int32, WebSocket> Instances = new Dictionary<Int32, WebSocket>();

    //Delegates
    public delegate void OnOpenCallback(int InstanceID);
    public delegate void OnMessageCallback(int InstanceID, System.IntPtr MsgPtr, int MsgSize);
    public delegate void OnErrorCallback(int InstanceID, System.IntPtr ErrorPtr);
    public delegate void OnCloseCallback(int InstanceID, int CloseCode);

    //WebSocket JSLIB callback setters and other functions
    [DllImport("__Internal")] public static extern int WebSocketAllocate(string URL);
    [DllImport("__Internal")] public static extern int WebSocketFree(int InstanceID);
    [DllImport("__Internal")] public static extern int WebSocketSetOnOpen(OnOpenCallback Callback);
    [DllImport("__Internal")] public static extern int WebSocketSetOnMessage(OnMessageCallback Callback);
    [DllImport("__Internal")] public static extern int WebSocketSetOnError(OnErrorCallback Callback);
    [DllImport("__Internal")] public static extern int WebSocketSetOnClose(OnCloseCallback Callback);

    //If callbacks have been initialized and set
    private static bool IsInitialized = false;

    //Initialize WebSocket callback to JSLIB
    private static void Initialize()
    {
        WebSocketSetOnOpen(DelegateOnOpenEvent);
        WebSocketSetOnMessage(DelegateOnMessageEvent);
        WebSocketSetOnError(DelegateOnErrorEvent);
        WebSocketSetOnClose(DelegateOnCloseEvent);

        IsInitialized = true;
    }

    //Called when instance is destroyed (by destructor)
    //Removes instance from map and free it in JSLIB implementation
    public static void HandleInstanceDestroy(int InstanceID)
    {
        Instances.Remove(InstanceID);
        WebSocketFree(InstanceID);
    }

    [MonoPInvokeCallback(typeof(OnOpenCallback))]
    public static void DelegateOnOpenEvent(int InstanceID)
    {
        WebSocket InstanceRef;
        if(Instances.TryGetValue(InstanceID, out InstanceRef))
            InstanceRef.DelegateOnOpenEvent();
    }

    [MonoPInvokeCallback(typeof(OnMessageCallback))]
    public static void DelegateOnMessageEvent(int InstanceID, System.IntPtr MsgPtr, int MsgSize)
    {
        WebSocket InstanceRef;
        if(Instances.TryGetValue(InstanceID, out InstanceRef))
        {
            byte[] Msg = new byte[MsgSize];
            Marshal.Copy(MsgPtr, Msg, 0, MsgSize);
            InstanceRef.DelegateOnMessageEvent(Msg);
        }
    }

    [MonoPInvokeCallback(typeof(OnErrorCallback))]
    public static void DelegateOnErrorEvent(int InstanceID, System.IntPtr ErrorPtr)
    {
        WebSocket InstanceRef;
        if(Instances.TryGetValue(InstanceID, out InstanceRef))
        {
            string ErrorMsg = Marshal.PtrToStringAuto(ErrorPtr);
            InstanceRef.DelegateOnErrorEvent(ErrorMsg);
        }
    }

    [MonoPInvokeCallback(typeof(OnCloseCallback))]
    public static void DelegateOnCloseEvent(int InstanceID, int CloseCode)
    {
        WebSocket InstanceRef;
        if(Instances.TryGetValue(InstanceID, out InstanceRef))
            InstanceRef.DelegateOnCloseEvent(CloseCode);
    }

#endif

    //Create WebSocket client instance
    public static WebSocket CreateInstance(string URL)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if(!IsInitialized)
            Initialize();

        int InstanceID = WebSocketAllocate(URL);
        WebSocket Wrapper = new WebSocket(InstanceID);
        Instances.Add(InstanceID, Wrapper);
        return Wrapper;
#else
        return new WebSocket(URL);
#endif
    }
}
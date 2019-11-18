// ================================================================================================================================
// File:        PlayerCommunicationPacketHandler.cs
// Description:	Recieves other clients chat messages from the server to be displayed in our chat window
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerCommunicationPacketHandler : MonoBehaviour
{
    //Singleton Instance
    public static PlayerCommunicationPacketHandler Instance = null;
    void Awake() { Instance = this; }

    //Recieves some other clients chat message and displays it in our chat log
    public static void HandleChatMessage(ref NetworkPacket Packet)
    {
        Log.In("Chat Message");

        //Read the relevant values from the network packet
        string Sender = Packet.ReadString();
        string Message = Packet.ReadString();

        //Display this message in the chat window
        Log.Chat(Sender + ": " + Message);
    }
}

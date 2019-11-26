// ================================================================================================================================
// File:        SystemPacketHandler.cs
// Description:	Handles low level system messages sent from the game server
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemPacketHandler : MonoBehaviour
{
    //If we miss some packets and they are then resent to us from the server, the will then tell us what the next packet number we can expect to recieve will be
    public static void HandleNewNextPacketNumber(ref NetworkPacket Packet)
    {
        Log.In("New Next Packet Number");
        Log.Chat("Recieved retransmitted packets that we previously missed.");

        int NextPacketNumber = Packet.ReadInt();
        ConnectionManager.Instance.LastPacketRecieved = NextPacketNumber - 1;
    }

    //Every few seconds the server will ask us if we are still connected to see that we havnt timed out
    public static void HandleStillConnectedCheck(ref NetworkPacket Packet)
    {
        Log.In("Still Connected Check");
        SystemPacketSender.Instance.SendStillConnectedReply();
    }

    //If we have fallen too far out of sync from the server, then will tell us our connection has been desync, so everything needs to be cleaned up
    //and then resynchronized once a gain to fix everything and get back in sync again
    public static void HandleConnectionDeSync(ref NetworkPacket Packet)
    {
        //Log.In("Connection DeSynced");
        //ConnectionManager.Instance.DeSynchronized = true;
    }

    //Handles recieving and processing of the set of the missing packets sent from server after we let them know we missed some of them
    public static void HandleMissingPacketsReply(ref NetworkPacket TotalPacket)
    {
        Log.Chat("Handling missing packets reply", true);

        //Loop through all of the data in the packet, passing each section of instruction on to their registered handler functions
        while (!TotalPacket.FinishedReading())
        {
            //Read the next packet type value from the remaining packet data
            ServerPacketType PacketType = TotalPacket.ReadType();

            Log.Chat("Missing Packet: " + PacketType + " handled.", true);

            //Use this to pass the packet on to have the next section of data managed by its registered handler function
            if (PacketHandler.Instance.PacketHandlers.TryGetValue(PacketType, out PacketHandler.Packet Packet))
                Packet.Invoke(ref TotalPacket);
        }

        Log.Chat("Finished reading missing packets", true);
    }

    //Handles alert from the server telling us we need to resend them some previous network packets again
    public static void HandleMissingPacketsRequest(ref NetworkPacket Packet)
    {
        //Find the number of the packet that was missed and resend it back to the server
        int MissingPacketNumber = Packet.ReadInt();
        ConnectionManager.Instance.SendMissingPacket(MissingPacketNumber);
    }

    //Handles alert from the server telling us we have been kicked from the server
    public static void HandleKickedFromServer(ref NetworkPacket Packet)
    {
        Log.Chat("Kicked from server, changing scene...", true);
        SceneManager.LoadScene("Kicked");
    }
}
// ================================================================================================================================
// File:        SystemPacketHandler.cs
// Description:	Handles low level system messages sent from the game server
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemPacketHandler : MonoBehaviour
{
    public static NetworkPacket GetValuesStillConnectedCheck(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.StillConnectedCheck);
        return Packet;
    }

    //Every few seconds the server will ask us if we are still connected to see that we havnt timed out
    public static void HandleStillConnectedCheck(ref NetworkPacket Packet)
    {
        SystemPacketSender.Instance.SendStillConnectedReply();
    }

    public static NetworkPacket GetValuesMissingPacketsRequest(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.MissingPacketsRequest);
        Packet.WriteInt(ReadFrom.ReadInt());
        return Packet;
    }

    //Handles alert from the server telling us we need to resend them some previous network packets again
    public static void HandleMissingPacketsRequest(ref NetworkPacket Packet)
    {
        //Flag the server as needing to have a bunch of missing packets resent back to it again
        ConnectionManager.Instance.PacketQueue.PacketsToResend = true;
        ConnectionManager.Instance.PacketQueue.ResendStartNumber = Packet.ReadInt();
    }

    public static NetworkPacket GetValuesKickedFromServer(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.KickedFromServer);
        Packet.WriteString(ReadFrom.ReadString());
        return Packet;
    }

    //Handles alert from the server telling us we have been kicked from the server
    public static void HandleKickedFromServer(ref NetworkPacket Packet)
    {
        //Get the reason we were kicked from the packet data
        string KickReason = Packet.ReadString();

        //Log a message showing thats happening here
        Log.Chat("Kicked from server: " + KickReason + ", changing scene...", true);

        //Instantiate a new KickReason prefab, make it remain between scene changes
        GameObject KickReasonObject = GameObject.Instantiate(PrefabManager.Instance.KickReasonPrefab, Vector3.zero, Quaternion.identity);
        DontDestroyOnLoad(KickReasonObject);

        //Get the script object on the KickReasonObject and use that to store the KickReason string inside it
        KickReason KickReasonScript = KickReasonObject.GetComponent<KickReason>();
        KickReasonScript.Reason = KickReason;

        //Now change to the scene showing the user they have been kicked from the game
        SceneManager.LoadScene("Kicked");
    }

    public static NetworkPacket GetValuesUIMessage(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.UIMessage);
        Packet.WriteString(ReadFrom.ReadString());
        return Packet;
    }

    public static void HandleUIMessage(ref NetworkPacket Packet)
    {
        string MessageContent = Packet.ReadString();
        UIServerMessageDisplay.Instance.DisplayMessage(MessageContent);
    }
}
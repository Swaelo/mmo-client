// ================================================================================================================================
// File:        SystemPacketHandler.cs
// Description:	Handles low level system messages sent from the game server
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemPacketHandler : MonoBehaviour
{
    //Every few seconds the server will ask us if we are still connected to see that we havnt timed out
    public static void HandleStillConnectedCheck(ref NetworkPacket Packet)
    {
        Log.In("Still Connected Check");
        SystemPacketSender.Instance.SendStillConnectedReply();
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
}
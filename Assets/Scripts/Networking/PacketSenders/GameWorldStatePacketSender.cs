// ================================================================================================================================
// File:        GameWorldStatePacketSender.cs
// Description:	Used to send alerts to the server when we want to start loading into the game world and to let it know when we are done
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class GameWorldStatePacketSender : MonoBehaviour
{
    //Singleton Instance
    public static GameWorldStatePacketSender Instance = null;
    void Awake() { Instance = this; }

    //Sends an alert to the game server letting them know we are now entering into the game world with a selected character
    public void SendEnterWorldAlert(string CharacterName)
    {
        //Create a new NetworkPacket to store the data for this enter world request
        NetworkPacket Packet = new NetworkPacket();

        //Write the relevant data values into the network packet
        Packet.WriteType(ClientPacketType.EnterWorldRequest);
        Packet.WriteString(CharacterName);

        //Add the new NetworkPacket to the outgoing packets queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);
    }

    //Sends an alert to the game server, letting them know we are ready and are now entering into the game world
    public void SendPlayerReadyAlert()
    {
        //Create a new NetworkPacket to store the data for this player ready alert
        NetworkPacket Packet = new NetworkPacket();

        //Write the relevant data values into the network packet
        Packet.WriteType(ClientPacketType.NewPlayerReady);

        //Add the new NetworkPacket to the outgoing packets queue
        ConnectionManager.Instance.PacketQueue.QueuePacket(Packet);

        //Disable the main scene camera and the Entering World UI animation
        GameObject.Find("Main Camera").SetActive(false);
        InterfaceManager.Instance.SetObjectActive("Entering World Panel", false);

        //Spawn in a local player prefab at our characters current world location
        GameObject.Instantiate(PlayerPrefabs.Instance.LocalPlayerPrefab, GameState.Instance.CharacterPositions[GameState.Instance.SelectedCharacter-1], Quaternion.identity);
    }
}

// ================================================================================================================================
// File:        PacketSender.cs
// Description: Functions for sending various sets of instructions and data to the game server
// ================================================================================================================================

public static class PacketSender
{
    public static void RequestNetworkID()
    {
        ConnectionManager.Instance.SendMessage("00" + ((int)ClientPacketTypes.NetworkIDRequest));
    }

    public static void SendChatMessage(string Username, string MessageContent)
    {
        ConnectionManager.Instance.SendMessage("00" + ((int)ClientPacketTypes.ChatMessage) + " " + Username + " " + MessageContent);
    }
}

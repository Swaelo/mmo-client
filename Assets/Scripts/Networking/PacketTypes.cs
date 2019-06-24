// ================================================================================================================================
// File:        PacketTypes.cs
// Description: Defines all the different types of packets which can be sent over the network
// ================================================================================================================================

using System;
using System.Collections.Generic;

public enum ClientPacketTypes
{
    NetworkIDRequest = 0,
    ChatMessage = 1
};

public enum ServerPacketTypes
{
    NetworkIDReply = 0
};
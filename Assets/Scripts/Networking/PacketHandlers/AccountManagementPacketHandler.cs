// ================================================================================================================================
// File:        AccountManagementPacketHandler.cs
// Description:	Handles any server packets which are received regarding any account management actions that have been performed
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class AccountManagementPacketHandler : MonoBehaviour
{
    //Handles reply from the server after we have requested to login to a user account
    public static void HandleAccountLoginReply(ref NetworkPacket Packet)
    {
        //Read the data values from the packet reader
        bool LoginSuccess = Packet.ReadBool();
        string ReplyMessage = Packet.ReadString();

        //Return to the account login screen if the request was denied
        if(!LoginSuccess)
        {
            Log.Chat("Account Login Failed: " + ReplyMessage);
            InterfaceManager.Instance.SetObjectActive("Logging In Panel", false);
            InterfaceManager.Instance.SetObjectActive("Account Login Panel", true);
        }
        //Otherwise we want to send a request for all our character data, and wait for that before moving to the character select screen
        else
        {
            InterfaceManager.Instance.SetObjectActive("Logging In Panel", false);
            InterfaceManager.Instance.SetObjectActive("Loading Characters Panel", true);
            AccountManagementPacketSender.Instance.SendCharacterDataRequest();
        }
    }

    public static void HandleAccountRegisterReply(ref NetworkPacket Packet)
    {
        //Read the data values from the packet reader
        bool RegisterSuccess = Packet.ReadBool();
        string ReplyMessage = Packet.ReadString();

        //Return to the account registration screen if the request was denied
        if(!RegisterSuccess)
        {
            Log.Chat("Account Creation Failed: " + ReplyMessage);
            InterfaceManager.Instance.SetObjectActive("Registering Panel", false);
            InterfaceManager.Instance.SetObjectActive("Account Register Panel", true);
        }
        //Otherwise we move on to the account login screen
        else
        {
            Log.Chat("Account Created!");
            InterfaceManager.Instance.SetObjectActive("Registering Panel", false);
            InterfaceManager.Instance.SetObjectActive("Account Login Panel", true);
        }
    }

    public static void HandleCharacterDataReply(ref NetworkPacket Packet)
    {
        //Read the number of characters existing in our account
        int CharacterCount = Packet.ReadInt();

        //If there arent any characters in our account then we proceed to the character creation screen
        if(CharacterCount == 0)
        {
            InterfaceManager.Instance.SetObjectActive("Loading Characters Panel", false);
            InterfaceManager.Instance.SetObjectActive("Character Create Panel", true);
            return;
        }

        //Otherwise we move on to the character select screen, and update it to show information of our existing characters
        InterfaceManager.Instance.SetObjectActive("Loading Characters Panel", false);
        InterfaceManager.Instance.SetObjectActive("Character Select Panel", true);

        //Loop through and extract each charactesr information from the packet data
        for(int i = 0; i < CharacterCount; i++)
        {
            //Read from the packet data each characters name and location values
            string CharacterName = Packet.ReadString();
            Vector3 CharacterPosition = Packet.ReadVector3();

            //Store the values of all of our existing characters in our connection manager object
            GameState.Instance.CharacterNames[i] = CharacterName;
            GameState.Instance.CharacterPositions[i] = CharacterPosition;

            //Update the character select screen to display this characters information
            string InfoObjectName = "Character Info " + (i+1);
            GameObject.Find(InfoObjectName).GetComponent<Text>().text = CharacterName;
        }
    }

    public static void HandleCreateCharacterReply(ref NetworkPacket Packet)
    {
        //Read the data values from the packet reader
        bool CreationSuccess = Packet.ReadBool();
        string ReplyMessage = Packet.ReadString();

        //If the creation was a success, then we should request all of our characters data before proceeding to the character select screen
        if(CreationSuccess)
        {
            Log.Chat("New character created!");
            InterfaceManager.Instance.SetObjectActive("Creating Character Panel", false);
            InterfaceManager.Instance.SetObjectActive("Loading Characters Panel", true);
            AccountManagementPacketSender.Instance.SendCharacterDataRequest();
        }
        //Otherwise we should return back to the character creation screen
        else
        {
            Log.Chat("Character Creation Failed: " + ReplyMessage);
            InterfaceManager.Instance.SetObjectActive("Creating Character Panel", false);
            InterfaceManager.Instance.SetObjectActive("Character Create Panel", true);
        }
    }
}

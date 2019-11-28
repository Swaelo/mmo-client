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
        Log.In("Account Login Reply");

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

    public static void HandleAccountRegistrationReply(ref NetworkPacket Packet)
    {
        Log.In("Account Registration Reply");

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
        //Log what we are doing here
        Log.In("Handling Character Data Reply.");

        //Read the number of characters data that has been sent to us
        int CharacterCount = Packet.ReadInt();

        //If there arent any characters in our account then we proceed straight to the character creation screen
        if(CharacterCount == 0)
        {
            InterfaceManager.Instance.SetObjectActive("Loading Characters Panel", false);
            InterfaceManager.Instance.SetObjectActive("Character Create Panel", true);
            return;
        }

        //Otherwise we move on to the character select screen
        InterfaceManager.Instance.SetObjectActive("Loading Characters Panel", false);
        InterfaceManager.Instance.SetObjectActive("Character Select Panel", true);

        //Loop through and read the data of each character provided
        for(int i = 0; i < CharacterCount; i++)
        {
            //Create and store each characters info into a new CharacterData object
            CharacterData Data = new CharacterData();
            Data.Name = Packet.ReadString();
            Data.Position = Packet.ReadVector3();
            Data.Rotation = Packet.ReadQuaternion();
            Data.CameraZoom = Packet.ReadFloat();
            Data.CameraXRotation = Packet.ReadFloat();
            Data.CameraYRotation = Packet.ReadFloat();
            Data.CurrentHealth = Packet.ReadInt();
            Data.MaxHealth = Packet.ReadInt();

            //Store each characters data to be accessed later
            if (i == 0)
                GameState.Instance.FirstCharacter = Data;
            else if (i == 1)
                GameState.Instance.SecondCharacter = Data;
            else
                GameState.Instance.ThirdCharacter = Data;

            //Set each character as selectable from the character select screen
            string InfoObjectName = "Character Info " + (i + 1);
            GameObject InfoObject = GameObject.Find(InfoObjectName);
            if (InfoObject == null)
                Log.Chat("Unable to find " + InfoObjectName);
            else
                InfoObject.GetComponent<Text>().text = Data.Name;
        }
    }

    public static void HandleCreateCharacterReply(ref NetworkPacket Packet)
    {
        Log.In("Character Creation Reply");

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

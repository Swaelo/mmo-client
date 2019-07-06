// ================================================================================================================================
// File:        ChatMessageInput.cs
// Description: Handles chat input and sending chat messages to the game server for other clients to read
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class ChatMessageInput : MonoBehaviour
{
    //There should be no way the player can interact with the chat window when the game first finishes loading
    


    // public bool ChatInputEnabled = false;
    

    // public bool ChatInputEnabled = false;
    // public bool IsTyping = false;
    // public InputField ChatWindowInput = null;  //Reference to the chat windows input field so the contents can be retrieved when finished typing a message 
    // public InputField ChatNameInput = null;    //Reference to the chat name input field so the clients username can be sent with the chat messages
    // public ConnectionManager GameServerConnection = null;  //Reference to the game server connection manager so chat messages can be sent across the network
    
    // private void Update()
    // {
    //     if(ChatInputEnabled && !IsTyping && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
    //     {
    //         ChatWindowInput.Select();
    //         ChatWindowInput.ActivateInputField();
    //         IsTyping = true;
    //     }
    // }

    // private void InputSubmitCallback()
    // {
    //     //If there name field is empty the message cannot be sent
    //     if(ChatNameInput.text == "")
    //     {
    //         Log.Chat("ERROR: You need to set a username to send chat messages.");
    //         return;
    //     }
    //     //If the name field contains any spaces the message cannot be sent
    //     if(NameContainsSpaces(ChatNameInput.text))
    //     {
    //         Log.Chat("ERROR: You are not allowed to have spaces in your chat name.");
    //         return;
    //     }
    //     //If neither the name field nor the input field are empty, display the message in chat and sent it to the game server
    //     if (ChatWindowInput.text != "")
    //     {
    //         PacketSender.SendChatMessage(ChatNameInput.text, ChatWindowInput.text);
    //         Log.Chat(ChatNameInput.text + ": " + ChatWindowInput.text);
    //         //GameServerConnection.SendMessage(ChatNameInput.text + ": " + ChatWindowInput.text);
    //     }
    //     //Reset the contents of the input field then before it is deselected
    //     ChatWindowInput.text = "";
    //     ChatWindowInput.text = "";
    // }

    // private bool NameContainsSpaces(string ChatName)
    // {
    //     for (int i = 0; i < ChatName.Length; i++)
    //         if (ChatName[i] == ' ')
    //             return true;
    //     return false;
    // }

    // void OnEnable()
    // {
    //     ChatWindowInput.onEndEdit.AddListener(delegate { InputSubmitCallback(); });
    // }

    // void OnDisable()
    // {
    //     IsTyping = false;
    //     ChatWindowInput.onEndEdit.RemoveAllListeners();
    //     ChatWindowInput.onValueChanged.RemoveAllListeners();
    // }
}
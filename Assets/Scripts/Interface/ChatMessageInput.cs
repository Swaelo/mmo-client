// ================================================================================================================================
// File:        ChatMessageInput.cs
// Description: Handles chat input and sending chat messages to the game server for other clients to read
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class ChatMessageInput : MonoBehaviour
{
    //Make into a singleton object so the player character can access it easily so it knows to ignore movement input while typing a message
    public static ChatMessageInput Instance = null;

    public bool IsTyping = false;   //Is the user currently typing a message into the input field
    private InputField ChatWindowInput = null;   //Input field used to input chat messages
    
    void Awake()
    {
        //Assign singleton
        Instance = this;
        //Assign the chat input field reference
        ChatWindowInput = GetComponent<InputField>();
    }

    void Update()
    {
        //If the input field is inactive and the client presses the enter key then it should become active so they can start typing a new message
        if(!IsTyping && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            ChatWindowInput.Select();
            ChatWindowInput.ActivateInputField();
            IsTyping = true;
        }
    }

    public void InputSubmitCallback()
    {
        //Fetch the contents of the input field and then empty it
        string ChatMessage = ChatWindowInput.text;
        ChatWindowInput.text = "";

        //If the input message is not empty then it should be sent to the game server
        if(ChatMessage != "")
        {
            PlayerCommunicationPacketSender.Instance.SendChatMessage(ChatMessage);
            //Also display the users message in their own chat log too
            Log.Chat(GameState.Instance.CurrentCharacterName + ": " + ChatMessage);
        }

        //Input field needs to be deactivated so the user is able to control their player once again
        IsTyping = false;
    }
}
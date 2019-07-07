// ================================================================================================================================
// File:        UIButtonFunctions.cs
// Description: Defines event functions triggered from UI buttons being interacted with
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class UIButtonFunctions : MonoBehaviour
{
    //Changes from the main menu panel to the account login panel
    public void MainMenuLoginButton()
    {
        //Disable the main menu panel, and enable the account login panel
        InterfaceManager.Instance.SetObjectActive("Main Menu Panel", false);
        InterfaceManager.Instance.SetObjectActive("Account Login Panel", true);
        
    }

    //Changes from the main menu panel to the account registration panel
    public void MainMenuRegisterButton()
    {
        //Disable the main menu panel, enable the account registration panel
        InterfaceManager.Instance.SetObjectActive("Main Menu Panel", false);
        InterfaceManager.Instance.SetObjectActive("Account Register Panel", true);
        
    }

    //Changes to the waiting animation panel until the account login reply has been received from the server
    public void LoginMenuLoginButton()
    {
        //Fetch the username and password credentials that have been entered into the UI
        string Username = GameObject.Find("Username Input").GetComponent<InputField>().text;
        string Password = GameObject.Find("Password Input").GetComponent<InputField>().text;

        //Validate the users input to make sure its all valid, tell them if someone was wrong
        if(Username == "" || Password == "")
        {
            Log.Chat("Error: Please fill the username and password fields.");
            return;
        }

        //If the input looks good, send it off to the game server as part of a login request
        AccountManagementPacketSender.Instance.SendLoginRequest(Username, Password);

        //Disable the account login UI and enable the logging in waiting animation panel
        InterfaceManager.Instance.SetObjectActive("Account Login Panel", false);
        InterfaceManager.Instance.SetObjectActive("Logging In Panel", true);
    }

    //Returns from the login panel back to the main menu panel
    public void LoginMenuBackButton()
    {
        //Disable the account login panel and return back to the main menu panel
        InterfaceManager.Instance.SetObjectActive("Main Menu Panel", true);
        InterfaceManager.Instance.SetObjectActive("Account Login Panel", false);
        
    }

    //Changes to the waiting animation panel until the account registration reply has been received from the server
    public void RegisterMenuRegisterButton()
    {
        //Fetch the username and password that have been entered into the UI
        string Username = GameObject.Find("New Username Input").GetComponent<InputField>().text;
        string Password = GameObject.Find("New Password Input").GetComponent<InputField>().text;
        string PasswordVerify = GameObject.Find("New Password Verify Input").GetComponent<InputField>().text;

        //If the password fields dont match then let the user know
        if(Password != PasswordVerify)
        {
            Log.Chat("The password fields did not match, registration failed.");
            return;
        }

        //Otherwise we need to move onto the registering animation and send the request to the game server
        InterfaceManager.Instance.SetObjectActive("Account Register Panel", false);
        InterfaceManager.Instance.SetObjectActive("Registering Panel", true);
        AccountManagementPacketSender.Instance.SendRegisterRequest(Username, Password);
    }

    //Returns from the account registration panel back to the main menu panel
    public void RegisterMenuBackButton()
    {
        //Disable the account registration panel, enable the main menu panel
        InterfaceManager.Instance.SetObjectActive("Main Menu Panel", true);
        InterfaceManager.Instance.SetObjectActive("Account Register Panel", false);
        
    }

    //Selects the clicked character slot from the character select screen
    public void CharacterSelectButton(int CharacterSlot)
    {
        //Store which character they tried to select
        GameState.Instance.SelectedCharacter = CharacterSlot;

        //If no character exists in this slot then go to the character creation screen
        if(GameState.Instance.CharacterNames[CharacterSlot-1] == "")
        {
            InterfaceManager.Instance.SetObjectActive("Character Select Panel", false);
            InterfaceManager.Instance.SetObjectActive("Character Create Panel", true);
        }
        //Otherwise we want to select that character and start playing the game with them
        else
        {
            InterfaceManager.Instance.SetObjectActive("Character Select Panel", false);
            InterfaceManager.Instance.SetObjectActive("Entering World Panel", true);
            GameState.Instance.CurrentCharacterName = GameState.Instance.CharacterNames[CharacterSlot-1];
            GameState.Instance.CurrentCharacterPosition = GameState.Instance.CharacterPositions[CharacterSlot-1];
            GameWorldStatePacketSender.Instance.SendEnterWorldAlert(GameState.Instance.CurrentCharacterName);
            //We will wait in the entering world panel until we have finished recieving and processing all the game world state data from the server
        }
    }

    //Logouts from the users account while in the character select screen, returning to the login screen
    public void CharacterSelectLogoutButton()
    {
        //Change the menu panels over to the account login screen
        InterfaceManager.Instance.SetObjectActive("Account Login Panel", true);
        InterfaceManager.Instance.SetObjectActive("Character Select Panel", false);
        //Let the server know we have logged out of this account
        AccountManagementPacketSender.Instance.SendLogoutAlert();
    }

    //Sends a request to the server to create a new player character
    public void CharacterCreationCreateButton()
    {
        //Fetch the name of the character that the user wants to create
        string CharacterName = GameObject.Find("Character Name Input").GetComponent<InputField>().text;

        //Change to the creating character screen and send a create character request to the game server
        InterfaceManager.Instance.SetObjectActive("Character Create Panel", false);
        InterfaceManager.Instance.SetObjectActive("Creating Character Panel", true);
        //Let the server know we want to create this new player character
        AccountManagementPacketSender.Instance.SendCreateCharacterRequest(CharacterName);
    }

    //Exits the character creation screen and returns to the character select screen
    public void CharacterCreationCancelButton()
    {
        InterfaceManager.Instance.SetObjectActive("Character Create Panel", false);
        InterfaceManager.Instance.SetObjectActive("Character Select Panel", true);
        
    }
}

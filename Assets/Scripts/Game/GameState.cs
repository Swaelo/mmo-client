// ================================================================================================================================
// File:        GameState.cs
// Description:	Keeps track of what account is currently logged in to, what character is being played with and tracks the current
//              progress of receiving and processing game world state information from the server while loading into the world
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class GameState : MonoBehaviour
{
    //Singleton Instance
    public static GameState Instance = null;
    void Awake() { Instance = this; }

    //Current user account and player character information
    public string AccountName = ""; //Which account the user is currently logged into
    public int SelectedCharacter = 0;   //Which character slot is currently being used
    public string[] CharacterNames = { "", "", "" };    //The names of characters existing under the current user account
    public Vector3[] CharacterPositions = { Vector3.zero, Vector3.zero, Vector3.zero }; //The positions of characters existing under the current user account
    public string CurrentCharacterName = "";
    public Vector3 CurrentCharacterPosition;

    //Tracks which values have been loaded in before we are reading to enter into the game world
    public bool PlayerListLoaded = false;
    public bool EntityListLoaded = false;
    public bool ItemListLoaded = false;
    public bool InventoryLoaded = false;
    public bool EquipmentLoaded = false;
    public bool AbilitiesLoaded = false;
    public bool WorldEntered = false;   //Once all the above flags have been set to true, we will be spawned into the game world

    void Update()
    {
        //If we have yet to enter into the game world, keep checking to see if we are ready to yet
        if(!WorldEntered && ReadyToEnter())
        {
            WorldEntered = true;
            Debug.Log("ready to spawn into the game world now");
            //Send a message to the game server letting them know we are now ready and are entering into the game world
            GameWorldStatePacketSender.Instance.SendPlayerReadyAlert();
        }
    }

    private bool ReadyToEnter()
    {
        return PlayerListLoaded && EntityListLoaded && ItemListLoaded && InventoryLoaded && EquipmentLoaded && AbilitiesLoaded;
    }
}

// ================================================================================================================================
// File:        GameState.cs
// Description:	Keeps track of what account is currently logged in to, what character is being played with and tracks the current
//              progress of receiving and processing game world state information from the server while loading into the world
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    //Singleton Instance
    public static GameState Instance = null;
    void Awake() { Instance = this; }

    public string AccountName = ""; //Which account the user is currently logged into

    //Store data for up to three characters in the users account
    public CharacterData FirstCharacter = new CharacterData();
    public CharacterData SecondCharacter = new CharacterData();
    public CharacterData ThirdCharacter = new CharacterData();

    //Track which character is currently selected for use
    public CharacterData SelectedCharacter = null;

    //Tracks which values have been loaded in before we are reading to enter into the game world
    public bool PlayerListLoaded = false;
    public bool EntityListLoaded = false;
    public bool ItemListLoaded = false;
    public bool InventoryLoaded = false;
    public bool EquipmentLoaded = false;
    public bool AbilitiesLoaded = false;
    public bool PlayerReady = false;    //Once all the above flags are set, we tell the server we are ready and wait for them to spawn us on its end
    public bool WorldEntered = false;   //Once the server has spawned us on their end they will let us know its time we can enter into the game world
    
    void Update()
    {
        //If we arent ready yet we need to keep checking, once ready we need to let the server know
        if(!PlayerReady && ReadyToEnter())
        {
            PlayerReady = true;
            GameWorldStatePacketSender.Instance.SendPlayerReadyAlert();
        }
    }

    private bool ReadyToEnter()
    {
        return PlayerListLoaded && EntityListLoaded && ItemListLoaded && InventoryLoaded && EquipmentLoaded && AbilitiesLoaded;
    }
}

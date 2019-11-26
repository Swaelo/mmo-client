// ================================================================================================================================
// File:        InterfaceManager.cs
// Description:	Assign a list of interface gameobjects through the inspector, they can then be toggle on and off through this class
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    //Singleton Instance
    public static InterfaceManager Instance = null;
    void Awake() { Instance = this; }

    //List of objects to be managed
    public GameObject[] InterfaceObjects;

    //List of 1 UI component from each menu state which should be set to the selected UI component whenever changing to that state
    public GameObject[] DefaultUIComponents;

    //A bunch of interface components need to be disabled as soon as the scene begins
    void Start()
    {
        //Chat Window message input needs to be disabled until the user have logged in
        SetObjectActive("Message Input", false);
        //All menu panels should be hidden until the server is connected to
        SetObjectActive("Main Menu Panel", false);
        SetObjectActive("Account Login Panel", false);
    }

    //Sets an object with the given name as either active or inactive
    public void SetObjectActive(string ObjectName, bool Active)
    {
        //Search through all the objects being managed
        for(int i = 0; i < InterfaceObjects.Length; i++)
        {
            //Check if this is the one we are looking for
            if(InterfaceObjects[i].transform.name == ObjectName)
            {
                //Set the objects to the desired value and exit from this function
                InterfaceObjects[i].SetActive(Active);
                //Reset menu's navigation whenever they are being entered into
                if(Active)
                {
                    MenuUINavigation MenuNavigation = InterfaceObjects[i].GetComponent<MenuUINavigation>();
                    if(MenuNavigation != null)
                        MenuNavigation.ResetNavigation();
                }
                return;
            }
        }
    }
}
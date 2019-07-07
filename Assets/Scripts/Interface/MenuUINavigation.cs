// ================================================================================================================================
// File:        MenuUINavigation.cs
// Description:	Overrides the built in navigation of unity UI to make it work much better, specific to each seperate menu state
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuUINavigation : MonoBehaviour
{
    //Set of items available to navigate in between while in this menu state
    public GameObject[] MenuComponents;
    //The default component that sets to active whenever entering this state
    public GameObject DefaultComponent;
    //The currently selected component in this menu
    private GameObject CurrentComponent;

    //Resets the navigation settings of this menu back to its default
    public void ResetNavigation()
    {
        //Use the EventSystem to set the default component back to the active component
        CurrentComponent = DefaultComponent;
        EventSystem.current.SetSelectedGameObject(CurrentComponent, new BaseEventData(EventSystem.current));
    }

    //Poll user input to navigate between this menus components
    void Update()
    {
        //Up arrow or Shift+Tab goes to the previous component in this menu
        bool ShiftTab = Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab);
        if(ShiftTab || Input.GetKeyDown(KeyCode.UpArrow))
        {
            //First we need to deactivate the current button component
            EventSystem.current.SetSelectedGameObject(null);

            //Now grab whatever object is previous in the menu and set it as the new current
            GameObject PreviousComponent = GetPreviousComponent(CurrentComponent);
            CurrentComponent = PreviousComponent;
            
            //If the previous component has an InputField component then we need to activate the carat/cursor in it
            InputField PreviousInput = PreviousComponent.GetComponent<InputField>();
            if(PreviousInput != null)
                PreviousInput.OnPointerClick(new PointerEventData(EventSystem.current));

            //Activate the new current component through the event system
            EventSystem.current.SetSelectedGameObject(PreviousComponent, new BaseEventData(EventSystem.current));
        }
        //Down arrow or Tab goes to the next component in this menu
        else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Tab))
        {
            //First we need to deactivate the current button component
            EventSystem.current.SetSelectedGameObject(null);

            //Now grab whatever object is next in this menu and set it as the new current
            GameObject NextComponent = GetNextComponent(CurrentComponent);
            CurrentComponent = NextComponent;

            //If the next component has an InputField component then we need to activate the carat/cursor in it
            InputField NextInput = NextComponent.GetComponent<InputField>();
            if(NextInput != null)
                NextInput.OnPointerClick(new PointerEventData(EventSystem.current));

            //Activate the new current component through the event system
            EventSystem.current.SetSelectedGameObject(NextComponent, new BaseEventData(EventSystem.current));
        }
    }

    //Returns whatever components is previous in the list compared to the given component
    private GameObject GetPreviousComponent(GameObject CurrentComponent)
    {
        //Get the current components menu index
        int CurrentIndex = GetComponentIndex(CurrentComponent);

        //Decrement this index, if it falls below zero wrap it back to the maximum index value
        int PreviousIndex = CurrentIndex - 1;
        if(PreviousIndex < 0)
            PreviousIndex = MenuComponents.Length - 1;

        //Finally use the PreviousIndex value to return the previous component object
        return MenuComponents[PreviousIndex];
    }

    //Returns whatever component is next in the list compared to the given component
    private GameObject GetNextComponent(GameObject CurrentComponent)
    {
        //Get the current components menu index
        int CurrentIndex = GetComponentIndex(CurrentComponent);

        //Increment this index, if it passes max wrap it back to 0
        int NextIndex = CurrentIndex + 1;
        if(NextIndex > MenuComponents.Length - 1)
            NextIndex = 0;

        //Use the NextIndex value to return the next component object
        return MenuComponents[NextIndex];
    }

    //Returns a components index in this menus component list
    private int GetComponentIndex(GameObject Component)
    {
        //Loop through all of this menu's components
        for(int i = 0; i < MenuComponents.Length; i++)
        {
            //Check each component, if it matches return the index value
            if(MenuComponents[i] == Component)
                return i;
        }
        //return garbage value if the component wasnt found in the list
        return -1;
    }
}

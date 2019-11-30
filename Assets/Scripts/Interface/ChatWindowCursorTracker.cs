// ================================================================================================================================
// File:        ChatWindowCursorTracker.cs
// Description:	Tracks when the mouse cursor is hovering over the chat window, used by the character camera controller to disable camera
// zooming while the mouse is over chat, to stop the chat window and the player camera from zooming/scrolling through at the same time.
// Author:	    Sjonsson's answer in this thread https://answers.unity.com/questions/967170/detect-if-pointer-is-over-any-ui-element.html
// ================================================================================================================================

using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ChatWindowCursorTracker : MonoBehaviour
{
    //Camera Controller checks this value
    public static bool IsMouseOverChat = false;

    //EventTrigger we use to track the cursors mouse over status
    private EventTrigger EventTrigger;

    //Events triggered when the mouse starts/stops hovering over the chat window
    public void CursorStartHoveringChat() { IsMouseOverChat = true; }
    public void CursorStopHoveringChat() { IsMouseOverChat = false; }

    private void Start()
    {
        //Grab the current EventTrigger component, then registered the events for tracking the status of the cursor hovering the chat
        EventTrigger = GetComponent<EventTrigger>();
        if(EventTrigger != null)
        {
            //Register an event for when the cursor starts hovering over the chat window
            EventTrigger.Entry CursorStartHoveringEntry = new EventTrigger.Entry();
            CursorStartHoveringEntry.eventID = EventTriggerType.PointerEnter;
            CursorStartHoveringEntry.callback.AddListener((eventData) => { CursorStartHoveringChat(); });
            EventTrigger.triggers.Add(CursorStartHoveringEntry);

            //Register another event for when the cursor stops hovering over the chat window
            EventTrigger.Entry CursorStopHoveringEntry = new EventTrigger.Entry();
            CursorStopHoveringEntry.eventID = EventTriggerType.PointerExit;
            CursorStopHoveringEntry.callback.AddListener((eventData) => { CursorStopHoveringChat(); });
            EventTrigger.triggers.Add(CursorStopHoveringEntry);
        }
    }
}

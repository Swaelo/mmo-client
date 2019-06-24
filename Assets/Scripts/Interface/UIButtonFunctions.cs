// ================================================================================================================================
// File:        UIButtonFunctions.cs
// Description: Defines event functions triggered from UI buttons being interacted with
// ================================================================================================================================

using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonFunctions : MonoBehaviour
{
    static ConnectionManager ServerConnection = null;
    EventSystem EventManager = null;
    InputField ChatNameInput = null;
    InputField ChatMessageInput = null;

    public void ConnectServer()
    {
        ConnectionManager.Instance.TryConnect();
    }
}

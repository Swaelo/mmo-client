// ================================================================================================================================
// File:        UIServerMessageDisplay.cs
// Description:	Takes a UI message sent from the server and displays it for a few seconds
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIServerMessageDisplay : MonoBehaviour
{
    public static UIServerMessageDisplay Instance = null;
    void Awake()
    {
        Instance = this;
        UIDisplayObject.SetActive(false);
    }

    public GameObject UIDisplayObject;
    private float DisplayTime = 3f;
    private float DisplayTimeLeft = 3f;
    private bool DisplayActive = false;

    public void DisplayMessage(string MessageContent)
    {
        UIDisplayObject.SetActive(true);
        UIDisplayObject.GetComponent<Text>().text = MessageContent;
        DisplayTimeLeft = DisplayTime;
        DisplayActive = true;
    }

    private void Update()
    {
        if(DisplayActive)
        {
            DisplayTimeLeft -= Time.deltaTime;
            if(DisplayTimeLeft <= 0.0f)
            {
                UIDisplayObject.SetActive(false);
                DisplayActive = false;
            }
        }
    }
}

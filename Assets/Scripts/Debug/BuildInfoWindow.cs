// ================================================================================================================================
// File:        BuildInfoWindow.cs
// Description:	Implements a custom editor window for managing the projects current build number
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

//Custom Editor Window code should not be included in builds
#if UNITY_EDITOR 

using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

class BuildInfoWindow : EditorWindow
{
    //Strings used to display the current build info to the window UI
    private static string BuildMajor = "Not";
    private static string BuildMinor = "Loaded";
    private static string BuildCustom = "";

    //Adds an option into the Window menu to create one of these windows
    [MenuItem ("Window/Build Info")]

    //Opens window and loads the build versions from file
    public static void ShowWindow()
    {
        //Show existing window instance or create a new one if it doesnt exist
        EditorWindow.GetWindow(typeof(BuildInfoWindow));
    }

    //Implements all of the window functionality
    private void OnGUI()
    {
        ////Display the current Major and Minor build numbers in the window UI
        GUILayout.Label(BuildMajor + ":" + BuildMinor);

        //Place a button which loads the current build info from the XML file
        if (GUILayout.Button("Load"))
            LoadBuildInfo();

        //Place a button which increments the current minor build number then resaves the XML file
        if (GUILayout.Button("Increment"))
            IncrementBuildNumber();

        //Place a text field to enter in a custom build version name and a button to apply it to the ingame UI
        BuildCustom = GUILayout.TextField(BuildCustom);
        if (GUILayout.Button("Set Custom"))
            SetCustomBuildName();
    }

    //Loads current build information from file
    private void LoadBuildInfo()
    {
        //Use the current directory to create a filepath directly to the build info document
        string CurrentDirectory = Directory.GetCurrentDirectory();
        string FileDirectory = CurrentDirectory + "\\BuildInfo.xml";

        //Open the file and fetch the build numbers from it
        XmlDocument InfoDoc = new XmlDocument();
        InfoDoc.Load(FileDirectory);

        //Read the build number values from the document and update the UI to display them
        BuildMajor = InfoDoc.DocumentElement.SelectSingleNode("/root/MajorBuild").InnerText;
        BuildMinor = InfoDoc.DocumentElement.SelectSingleNode("/root/MinorBuild").InnerText;

        //Update the window UI
        GameObject VersionDisplay = GameObject.Find("Version Number");
        VersionDisplay.GetComponent<Text>().text = BuildMajor + ":" + BuildMinor;
    }

    //Increments current minor build value and saves it into the xml document
    private void IncrementBuildNumber()
    {
        //Get the current minor build number as an integer value, then increment it
        int NewBuildNumber = Convert.ToInt32(BuildMinor) + 1;

        //Open the XML file
        string CurrentDirectory = Directory.GetCurrentDirectory();
        string FileDirectory = CurrentDirectory + "\\BuildInfo.xml";
        XmlDocument InfoDoc = new XmlDocument();
        InfoDoc.Load(FileDirectory);

        //Update its minor build number and save the file
        InfoDoc.DocumentElement.SelectSingleNode("/root/MinorBuild").InnerText = NewBuildNumber.ToString();
        InfoDoc.Save(FileDirectory);

        //Update the window UI aswell as the ingame UI to display the new build number
        BuildMinor = NewBuildNumber.ToString();
        GameObject VersionDisplay = GameObject.Find("Version Number");
        VersionDisplay.GetComponent<Text>().text = BuildMajor + ":" + BuildMinor;
    }

    //Updates the UI with the current custom build name
    private void SetCustomBuildName()
    {
        //Update the ingame UI to display the new custom build name
        GameObject VersionDisplay = GameObject.Find("Version Number");
        VersionDisplay.GetComponent<Text>().text = BuildCustom;
    }
}

#endif
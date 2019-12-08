// ================================================================================================================================
// File:        LoadSceneButton.cs
// Description: Gives UI buttons the functionality to load into a different scene
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public string SceneName = "";

    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
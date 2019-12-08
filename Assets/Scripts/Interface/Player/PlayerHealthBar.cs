// ================================================================================================================================
// File:        PlayerHealthBar.cs
// Description:	Controls the rendering of the players current health to the UI
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    //Characters Current/Max HP Values
    public int CurrentHP = 10;
    public int MaxHP = 10;

    //Whether the health bar display is enabled or not and its current length to be rendered with
    public bool HealthBarActive = false;
    private float HealthBarLength;

    //Fetch the characters initial health values from the gamestate when its spawned into the world then use
    //those to set its initial values and enable it for rendering
    void Awake()
    {
        //Fetch the initial values from the GameState
        GameState GameState = GameState.Instance;
        CurrentHP = GameState.SelectedCharacter.CurrentHealth;
        MaxHP = GameState.SelectedCharacter.MaxHealth;
        //Initialize the health bar and set it as active
        HealthBarLength = (Screen.width / 4) * (CurrentHP / (float)MaxHP);
        HealthBarActive = true;
    }

    //Draws the health bar to the UI whenever its enabled
    void OnGUI()
    {
        if(HealthBarActive)
            GUI.Box(new Rect(10, 10, HealthBarLength, 20), CurrentHP + "/" + MaxHP);
    }

    //Stores a new value for the current health and updates the UI to show the new value
    public void AdjustCurrentHealth(int NewValue)
    {
        CurrentHP = NewValue;
        HealthBarLength = (Screen.width / 4) * (CurrentHP / (float)MaxHP);
    }
}

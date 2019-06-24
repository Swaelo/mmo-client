// ================================================================================================================================
// File:        LoadingAnimation.cs
// Description: Changes colors of the letters on the UI just to show something happening while waiting for the game to load/connect
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimation : MonoBehaviour
{
    public Color DefaultColor;      //Default color of the letters
    public Color LitColor;          //Color of the letters while they are lit up
    public Text[] LoadingLetters;   //Array of UI letter objects that will be lit up

    public float LightChangeInterval = 0.1f;   //How long each letter remains lit before moving to the next
    public float LightWaitTime = 1.5f;          //Time to wait once the final letter has been lit, before starting again
    private float NextLightChange;              //How much longer to wait until changing to the next letter

    private int FinalLetterIndex = 0;
    private int CurrentLetterIndex = -1;

    private void Awake()
    {
        FinalLetterIndex = LoadingLetters.Length - 1;
        NextLightChange = LightChangeInterval;
    }

    private void Update()
    {
        NextLightChange -= Time.deltaTime;
        if (NextLightChange <= 0.0f)
        {
            //Reset the timer for the next light change
            NextLightChange = LightChangeInterval;

            //If we are at the start of the list then we light up the first letter
            if(CurrentLetterIndex == -1)
            {
                //Light up the first letter in the array
                CurrentLetterIndex = 0;
                LoadingLetters[CurrentLetterIndex].color = LitColor;
            }
            //Otherwise we set the current letter back to its default value then move onto the next
            else if(CurrentLetterIndex < FinalLetterIndex)
            {
                LoadingLetters[CurrentLetterIndex].color = DefaultColor;
                CurrentLetterIndex++;
                LoadingLetters[CurrentLetterIndex].color = LitColor;
            }
            //Otherwise we have reached the end of the list, return to the beginning with a greater wait time
            else
            {
                LoadingLetters[CurrentLetterIndex].color = DefaultColor;
                CurrentLetterIndex = -1;
                NextLightChange = LightWaitTime;
            }
        }
    }
}

// ================================================================================================================================
// File:        CharacterData.cs
// Description: Stores all the current information regarding a clients active player character currently active in the game world
// Author:      Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class CharacterData
{
    public string Account;  //Name of the account this character belongs to
    public string Name; //Characters name
    public bool NewPosition = false;
    public Vector3 Position;    //Characters position in the world
    public Vector3 Movement;    //Characters current input movement vector
    public Quaternion Rotation; //Character current rotation
    public float CameraZoom;    //How far this characters camera is zoomed out
    public float CameraXRotation;   //Character cameras current X Rotation value
    public float CameraYRotation;   //Character cameras current Y Rotation value
    public int CurrentHealth;   //Current number of Health Points
    public int MaxHealth;   //Current maximum number of Health Points
    public int Experience;  //Current EXP value
    public int ExperienceToLevel;   //Amount of EXP needed to reach the next level
    public int Level;   //Current level
    public bool IsMale; //Is the character male
}
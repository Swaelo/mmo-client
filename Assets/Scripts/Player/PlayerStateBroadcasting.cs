// ================================================================================================================================
// File:        PlayerStateBroadcasting.cs
// Description:	Keeps the game server up to date on the player characters current state during gameplay
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerStateBroadcasting : MonoBehaviour
{
    private float StateUpdateInterval = 1.0f;   //How often we should send the server our updated player status
    private float NextStateUpdate = 1.0f;   //How long until we need to send the server our updated player status
    private Vector3 LastPositionBroadcasted;    //The last position value that was broadcast to the game server

    void Update()
    {
        //Count down the update timer until it reaches zero
        NextStateUpdate -= Time.deltaTime;

        //Reset the timer and send the server our updated player state whenever the timer reaches zero
        if(NextStateUpdate <= 0.0f)
        {
            //Reset the update timer
            NextStateUpdate = StateUpdateInterval;

            //Send our updated information to the game server if we are no longer at the position that was previously broadcast
            if(transform.position != LastPositionBroadcasted)
            {
                //Send our updated position value to the game server
                PlayerManagementPacketSender.Instance.SendPlayerUpdate(transform.position);

                //Store this as the LastPositionBroadcasted so it doesnt get sent over and over when we arent moving
                LastPositionBroadcasted = transform.position;
            }
        }
    }
}

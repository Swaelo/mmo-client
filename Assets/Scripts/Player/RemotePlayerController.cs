// ================================================================================================================================
// File:        RemotePlayerController.cs
// Description:	Controls the remote player characters and moves them towards their target position sent from the server
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class RemotePlayerController : MonoBehaviour
{
    //Movement Settings
    private CharacterController Controller;
    private float MoveSpeed = 5f;
    private float TurnSpeed = 300f;

    //Animation Control
    private Animator Animator;
    private Vector3 PreviousPosition;

    //Most up to date values sent to us from the game server
    private Vector3 ServerPosition;
    private Quaternion ServerRotation;

    //Health Values
    private int Health = 10;
    private int Max = 10;
    private TextMesh HealthDisplay = null;

    //Set when forcing the character to move to a new location, performed in the RespawnRemotePlayer function to ensure they get moved to their spawn location on our end
    private bool ForceMove = false;
    private Vector3 ForceMovePos;
    private Quaternion ForceMoveRot;

    private void Awake()
    {
        ServerPosition = transform.position;
        ServerRotation = transform.rotation;

        Controller = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        PreviousPosition = transform.position;
        HealthDisplay = transform.Find("Character Health").GetComponent<TextMesh>();
    }

    private void FixedUpdate()
    {
        //Force set the values when told to
        if(ForceMove)
        {
            transform.position = ForceMovePos;
            transform.rotation = ForceMoveRot;
            ForceMove = false;

            //Send empty values to the animation controller
            Animator.SetFloat("DistanceTravelled", 0);
            Animator.SetBool("IsGrounded", true);
        }
        //Otherwise move normally
        else
        {
            //Move toward the target location if we arent there yet
            Vector3 Offset = ServerPosition - transform.position;
            if(Offset.magnitude > .1f)
            {
                Offset = Offset.normalized * MoveSpeed;
                Controller.Move(Offset * Time.fixedDeltaTime);
            }

            //Turn toward matching our rotation on the server
            transform.rotation = Quaternion.RotateTowards(transform.rotation, ServerRotation, TurnSpeed * Time.fixedDeltaTime);

            //Send movement values to the animation controller
            Animator.SetFloat("DistanceTravelled", Vector3.Distance(transform.position, PreviousPosition));
            Animator.SetBool("IsGrounded", IsGrounded());

            //Store position for distance check in next update
            PreviousPosition = transform.position;
        }
    }

    //Forces the player to move to a set location/rotation on the next LateUpdate
    public void ForceSetValues(Vector3 NewPosition, Quaternion NewRotation)
    {
        //Store the values to apply in the next LateUpdate
        ForceMove = true;
        ForceMovePos = NewPosition;
        ForceMoveRot = NewRotation;

        //Set these new values as whats on the server side also
        ServerPosition = NewPosition;
        ServerRotation = NewRotation;

        //Reset the health back to full
        Health = Max;
        HealthDisplay.text = Health + "/" + Max;
}

    public void SetValues(Vector3 Position, Quaternion Rotation, int CurrentHealth, int MaxHealth)
    {
        transform.position = Position;
        ServerPosition = Position;
        transform.rotation = Rotation;
        ServerRotation = Rotation;
        this.Health = CurrentHealth;
        this.Max = MaxHealth;
        HealthDisplay.text = this.Health + "/" + this.Max;
    }

    public void UpdateHealth(int Health)
    {
        this.Health = Health;
        HealthDisplay.text = this.Health + "/" + Max;
    }

    public void UpdatePosition(Vector3 Position)
    {
        //Unity and Bepu have reversed Z axis directions, so we flip the Z value here when we recieve it from the server before we update it
        Vector3 NewPosition = new Vector3(Position.x, Position.y, -Position.z);
        ServerPosition = NewPosition;
    }

    public void UpdateRotation(Quaternion Rotation)
    {
        ServerRotation = Rotation;
    }

    //Uses raycasting to check the distance between the players feet and whatever ground is below them to determine if they are standing or in the air
    private bool IsGrounded()
    {
        RaycastHit GroundHit;   //Where information will be stored about what object the raycast hit
        //Shoot a raycast from the characters feet directly down and see if it hits anything they are able to stand on
        if(Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out GroundHit, 1))
        {
            //If the raycast hit something, check how far away it is, if its close enough then we know the player is standing on it
            float GroundDistance = Vector3.Distance(transform.position, GroundHit.point);
            //If the ground is less than half a unit of distance away then we know the player is standing on it
            return GroundDistance <= 0.5f;
        }

        //If the raycast didnt hit anything then we know the player is definitely in the air
        return false;
    }

    //Calculates a new target rotation value to face in the direction the player is moving towards
    private Quaternion ComputeTargetRotation(Vector3 MovementVector)
    {
        //Return current rotation is movement is zero
        if (MovementVector == Vector3.zero)
            return transform.rotation;

        //Otherwise we want the player to face in the direction they are moving in
        Quaternion TargetRotation = Quaternion.LookRotation(MovementVector);
        Vector3 Eulers = TargetRotation.eulerAngles;
        Eulers.x = transform.rotation.x;
        TargetRotation.eulerAngles = Eulers;
        return TargetRotation;
    }

    //Remote player just needs some handler functions for the animation attack events, doesnt need to actually do anything with them
    public void AttackHit()
    {

    }
}
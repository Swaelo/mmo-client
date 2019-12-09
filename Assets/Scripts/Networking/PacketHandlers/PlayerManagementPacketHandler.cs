// ================================================================================================================================
// File:        PlayerManagementPacketHandler.cs
// Description:	Handles packets from the server with stuff like updated position values for other ingame player characters
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerManagementPacketHandler : MonoBehaviour
{
    //Singleton Instance
    public static PlayerManagementPacketHandler Instance = null;
    void Awake() { Instance = this; }

    public static NetworkPacket GetValuesUpdateRemotePlayer(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.UpdateRemotePlayer);
        Packet.WriteString(ReadFrom.ReadString());
        Packet.WriteVector3(ReadFrom.ReadVector3());
        Packet.WriteVector3(ReadFrom.ReadVector3());
        Packet.WriteQuaternion(ReadFrom.ReadQuaternion());
        Packet.WriteInt(ReadFrom.ReadInt());
        Packet.WriteInt(ReadFrom.ReadInt());
        return Packet;
    }

    public static void HandleUpdateRemotePlayer(ref NetworkPacket Packet)
    {
        //Log what we are doing here
        Log.In("Handle Remote Player Update");

        //Extract all the data from the network packet
        string Name = Packet.ReadString();
        Vector3 Position = Packet.ReadVector3();
        Vector3 Movement = Packet.ReadVector3();
        Quaternion Rotation = Packet.ReadQuaternion();
        int CurrentHealth = Packet.ReadInt();
        int MaxHealth = Packet.ReadInt();

        //Pass the values on to the player handler for processing
        PlayerManager.Instance.UpdateRemotePlayer(Name, Position, Movement, Rotation, CurrentHealth, MaxHealth);
    }

    public static NetworkPacket GetValuesAddRemotePlayer(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.AddRemotePlayer);
        Packet.WriteString(ReadFrom.ReadString());
        Packet.WriteBool(ReadFrom.ReadBool());
        Packet.WriteVector3(ReadFrom.ReadVector3());
        Packet.WriteVector3(ReadFrom.ReadVector3());
        Packet.WriteQuaternion(ReadFrom.ReadQuaternion());
        Packet.WriteInt(ReadFrom.ReadInt());
        Packet.WriteInt(ReadFrom.ReadInt());
        return Packet;
    }

    //Handles instructions to spawn a newly connected game clients player character into our game world
    public static void HandleAddRemotePlayer(ref NetworkPacket Packet)
    {
        Log.In("Spawn Remote Player");

        //Read the relevant values from the network packet
        string CharacterName = Packet.ReadString();
        bool CharacterAlive = Packet.ReadBool();
        Vector3 CharacterPosition = Packet.ReadVector3();
        Vector3 CharacterMovement = Packet.ReadVector3();
        Quaternion CharacterRotation = Packet.ReadQuaternion();
        int CharacterHealth = Packet.ReadInt();
        int CharacterMaxHealth = Packet.ReadInt();

        //Use the remote player manager to spawn this remote player character into our game world
        PlayerManager.Instance.AddRemotePlayer(CharacterName, CharacterAlive, CharacterPosition, CharacterMovement, CharacterRotation, CharacterHealth, CharacterMaxHealth);
    }

    public static NetworkPacket GetValuesRemoveRemotePlayer(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.RemoveRemotePlayer);
        Packet.WriteString(ReadFrom.ReadString());
        Packet.WriteBool(ReadFrom.ReadBool());
        return Packet;
    }

    //Handles instructions to despawn some other dead clients player character from our game world
    public static void HandleRemoveRemotePlayer(ref NetworkPacket Packet)
    {
        Log.In("Remove Remote Player");

        //Read the relevant data values from the network packet
        string CharacterName = Packet.ReadString();
        bool IsAlive = Packet.ReadBool();

        //Use the remote player manager to despawn this dead clients player character from our game world
        PlayerManager.Instance.RemoveRemotePlayer(CharacterName, IsAlive);
    }

    public static NetworkPacket GetValuesAllowPlayerBegin(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.AllowPlayerBegin);
        return Packet;
    }

    //Finally enters our player character into the game world once the server has let us know they've added us into the world physics simulation
    public static void HandleAllowPlayerBegin(ref NetworkPacket Packet)
    {
        Log.In("Local Player Begin");

        //Change from the main menu UI to the ingame UI, disable the menu camera
        InterfaceManager.Instance.SetObjectActive("Message Input", true);
        InterfaceManager.Instance.SetObjectActive("Menu Background", false);
        InterfaceManager.Instance.SetObjectActive("Entering World Panel", false);
        CameraManager.Instance.ToggleMainCamera(false);

        //Get the current character index from the GameState object, use that to fetch our position/rotation then spawn into the game world with those values
        GameState GameState = GameState.Instance;
        GameState.WorldEntered = true;
        PlayerManager.Instance.AddLocalPlayer(GameState.SelectedCharacter.Position, GameState.SelectedCharacter.Rotation);

        //Fetch the current zoom/rotation values of the players camera, set the camera to these values then enable camera state broadcasting
        float CameraZoom = GameState.SelectedCharacter.CameraZoom;
        float CameraXRotation = GameState.SelectedCharacter.CameraXRotation;
        float CameraYRotation = GameState.SelectedCharacter.CameraYRotation;
        GameObject PlayerCamera = PlayerManager.Instance.LocalPlayer.transform.Find("Player Camera").gameObject;
        PlayerCamera.GetComponent<PlayerCameraController>().SetCamera(CameraZoom, CameraXRotation, CameraYRotation);
    }

    public static NetworkPacket GetValuesRemotePlayerPlayAnimation(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.RemotePlayerPlayAnimationAlert);
        Packet.WriteString(ReadFrom.ReadString());
        Packet.WriteString(ReadFrom.ReadString());
        return Packet;
    }

    public static void HandleRemotePlayerPlayAnimation(ref NetworkPacket Packet)
    {
        Log.In("Remote Player Play Animation");
        string CharacterName = Packet.ReadString();
        string AnimationName = Packet.ReadString();
        PlayerManager.Instance.RemotePlayerPlayAnimation(CharacterName, AnimationName);
    }
}

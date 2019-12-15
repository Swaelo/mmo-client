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

    public static NetworkPacket GetValuesPlayerPositionUpdate(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.PlayerPositionUpdate);
        Packet.WriteString(ReadFrom.ReadString());
        Packet.WriteVector3(ReadFrom.ReadVector3());
        return Packet;
    }
    public static NetworkPacket GetValuesPlayerRotationUpdate(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.PlayerRotationUpdate);
        Packet.WriteString(ReadFrom.ReadString());
        Packet.WriteQuaternion(ReadFrom.ReadQuaternion());
        return Packet;
    }

    public static void HandlePlayerPositionUpdate(ref NetworkPacket Packet)
    {
        string Name = Packet.ReadString();
        Vector3 Position = Packet.ReadVector3();
        PlayerManager.Instance.UpdatePlayerPosition(Name, Position);
    }
    public static void HandlePlayerRotationUpdate(ref NetworkPacket Packet)
    {
        string Name = Packet.ReadString();
        Quaternion Rotation = Packet.ReadQuaternion();
        PlayerManager.Instance.UpdatePlayerRotation(Name, Rotation);
    }

    public static NetworkPacket GetValuesAddRemotePlayer(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.AddPlayer);
        Packet.WriteString(ReadFrom.ReadString());
        Packet.WriteBool(ReadFrom.ReadBool());
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
        Quaternion CharacterRotation = Packet.ReadQuaternion();
        int CharacterHealth = Packet.ReadInt();
        int CharacterMaxHealth = Packet.ReadInt();

        //Use the remote player manager to spawn this remote player character into our game world
        PlayerManager.Instance.AddRemotePlayer(CharacterName, CharacterAlive, CharacterPosition, CharacterRotation, CharacterHealth, CharacterMaxHealth);
    }

    public static NetworkPacket GetValuesRemoveRemotePlayer(NetworkPacket ReadFrom)
    {
        NetworkPacket Packet = new NetworkPacket();
        Packet.WriteType(ServerPacketType.RemovePlayer);
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
        Packet.WriteType(ServerPacketType.PlayAnimationAlert);
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

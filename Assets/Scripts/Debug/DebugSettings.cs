using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSettings : MonoBehaviour
{
    public bool LogOutgoingPackets = false;
    public bool LogIncomingPackets = false;

    public static DebugSettings Instance = null;
    void Awake() { Instance = this; }
}

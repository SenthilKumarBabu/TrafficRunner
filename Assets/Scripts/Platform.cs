using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public PlatformData data;
}

[Serializable]
public struct PlatformData
{
    public PlatformType type;
    [HideInInspector] public GameObject platform;
    public Vector3 position;
    public Vector3 scale;
    public float value;
}

public enum PlatformType
{
    Jumping,
    SpeedUp,
    SpeedDown
}

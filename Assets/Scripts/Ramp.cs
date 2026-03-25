using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    public RampData data;
}

[Serializable]
public struct RampData
{
    public RampType type;
    [HideInInspector] public GameObject ramp;
    public Vector3 position;
    public Vector3 scale;
}

public enum RampType
{
    Ramp25,
    Ramp45
}

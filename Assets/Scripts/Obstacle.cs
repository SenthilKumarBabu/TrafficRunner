using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public ObstacleData data;
}

[Serializable]
public struct ObstacleData
{
    public ObstacleType type;
    [HideInInspector] public GameObject obstacle;
    public Vector3 position;
}

public enum ObstacleType
{
    ZebraCrossing,
    Up,
    Down,
    Both
}

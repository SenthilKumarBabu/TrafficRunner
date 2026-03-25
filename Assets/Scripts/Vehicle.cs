using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public VehicleData data;
}

[Serializable]
public struct VehicleData
{
    public VehicleType type;
    [HideInInspector] public GameObject vehicle;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float moveSpeed;
}

public enum VehicleType
{
    Ambulance,
    BlueBus,
    BlueCar,
    BlueTruck,
    BrownBus,
    FireTruck,
    GarbageTruck,
    GreenCar,
    GreenVan,
    GreyBus,
    Police,
    RedCar,
    RedLoadTruck,
    RedVan,
    Taxi,
    YellowLoadTruck
}
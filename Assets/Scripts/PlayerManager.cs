using System;
using System.Collections;
using System.Collections.Generic;
using PlatformCharacterController;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static MovementCharacterController LocalPlayerInstance;

    private void Awake()
    {
    }
}

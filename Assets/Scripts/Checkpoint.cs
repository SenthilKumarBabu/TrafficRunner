using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public CheckpointData data;

    private LobbyManager lobbyManager;
    private NetworkEvents _networkEvents;

    private void Awake()
    {
        lobbyManager = ReferenceManager.Get<LobbyManager>();
        _networkEvents = ReferenceManager.Get<NetworkEvents>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player.ToString()))
        {
            if (data.type == CheckpointType.Finish)
            {
                if(other.GetComponent<NetworkObject>().IsOwner)
                    _networkEvents.OnLocalPlayerCompleteRace();
            }
        }
    }
}

[Serializable]
public struct CheckpointData
{
    public CheckpointType type;
    [HideInInspector] public GameObject checkpoint;
    public Vector3 position;
}

public enum CheckpointType
{
    Finish,
    Blocker
}
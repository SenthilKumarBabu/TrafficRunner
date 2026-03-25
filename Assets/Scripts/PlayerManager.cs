using System;
using System.Collections;
using System.Collections.Generic;
using PlatformCharacterController;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static MovementCharacterController LocalPlayerInstance;

    private GameManager _gameManager;
    private LobbyManager _lobbyManager;
    private NetworkEvents networkEvents;
    private NetworkRpc networkRpc;
    private LobbyPage _lobbyPage;

    private void Awake()
    {
        _gameManager = ReferenceManager.Get<GameManager>();
        _lobbyManager = ReferenceManager.Get<LobbyManager>();
        networkEvents = ReferenceManager.Get<NetworkEvents>();
        networkRpc = ReferenceManager.Get<NetworkRpc>();
        _lobbyPage = ReferenceManager.Get<LobbyPage>();
    }
}

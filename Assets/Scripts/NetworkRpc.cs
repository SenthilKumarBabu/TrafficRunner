using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkRpc : NetworkBehaviour
{
    private LobbyManager _lobbyManager;
    private GameManager _gameManager;
    private NetworkEvents _networkEvents;
    private NetworkData _networkData;
    private CountryManager _countryManager;
    
    private void Awake()
    {
        _lobbyManager = ReferenceManager.Get<LobbyManager>();
        _gameManager = ReferenceManager.Get<GameManager>();
        _networkEvents = ReferenceManager.Get<NetworkEvents>();
        _networkData = ReferenceManager.Get<NetworkData>();
        _countryManager = ReferenceManager.Get<CountryManager>();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerDataServerRpc(PlayerData playerData)
    {
        _networkData.SetPlayerData(playerData);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void CheckForStartRaceServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log($"CheckForStartRaceServerRpc called by {serverRpcParams.Receive.SenderClientId}");

        if (_networkData.AllPlayersReady())
        {
            StartRaceClientRpc();
        }
    }

    [ClientRpc]
    private void StartRaceClientRpc()
    {
        _networkEvents.StartRace();
    }

    [ClientRpc]
    public void CreateCountryClientRpc()
    {
        _countryManager.CreateCountry();
    }
}

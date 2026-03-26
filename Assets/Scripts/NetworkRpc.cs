using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkRpc : NetworkBehaviour
{
    private GameManager _gameManager;
    private NetworkEvents _networkEvents;
    private NetworkData _networkData;
    private CountryManager _countryManager;

    private bool _raceStarted;

    private void Awake()
    {
        _gameManager = ReferenceManager.Get<GameManager>();
        _networkEvents = ReferenceManager.Get<NetworkEvents>();
        _networkData = ReferenceManager.Get<NetworkData>();
        _countryManager = ReferenceManager.Get<CountryManager>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerDataServerRpc(PlayerData playerData)
    {
        Debug.Log($"[NetworkRpc] SetPlayerDataServerRpc: id={playerData.id} isConnected={playerData.isConnected} isReady={playerData.isReadyForGame}");
        _networkData.SetPlayerData(playerData);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CheckForStartRaceServerRpc(ServerRpcParams serverRpcParams)
    {
        if (_raceStarted) return;
        var allReady = _networkData.AllPlayersReady();
        Debug.Log($"[NetworkRpc] CheckForStartRaceServerRpc called by {serverRpcParams.Receive.SenderClientId} — AllPlayersReady={allReady}");

        if (allReady)
        {
            _raceStarted = true;
            StartRaceClientRpc();
        }
    }

    [ClientRpc]
    private void StartRaceClientRpc()
    {
        Debug.Log($"[NetworkRpc] StartRaceClientRpc received by client {NetworkManager.Singleton.LocalClientId}");
        _networkEvents.StartRace();
    }

    [ClientRpc]
    public void CreateCountryClientRpc()
    {
        Debug.Log($"[NetworkRpc] CreateCountryClientRpc received by client {NetworkManager.Singleton.LocalClientId}");
        _countryManager.CreateCountry();
    }
}

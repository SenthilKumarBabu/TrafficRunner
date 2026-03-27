using System;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Logs NetworkEvents and NetworkManager events for debugging. Attach alongside NetworkEvents.
/// Replace the networkManagerEventsLogs / inGameEventsLogs toggles that were on NetworkEvents.
/// </summary>
public class NetworkLogger : NetworkBehaviour
{
    [SerializeField] private bool networkManagerEventsLogs;
    [SerializeField] private bool inGameEventsLogs;

    private NetworkEvents _networkEvents;

    // Stored as fields so we can unsubscribe from NetworkManager (fixes Issue 6)
    private Action<ulong> _onClientConnected;
    private Action<NetworkManager, ConnectionEventData> _onConnectionEvent;
    private Action _onServerStarted;
    private Action<bool> _onServerStopped;
    private Action _onClientStarted;
    private Action<bool> _onClientStopped;

    private void Awake()
    {
        _networkEvents = ReferenceManager.Get<NetworkEvents>();
    }

    public override void OnNetworkSpawn()
    {
        if (inGameEventsLogs)
        {
            _networkEvents.OnPlayerSpawned += LogPlayerSpawned;
            _networkEvents.OnGameStarted += LogGameStarted;
            _networkEvents.OnSceneDataInitialized += LogSceneDataInitialized;
            _networkEvents.OnRaceStarted += LogRaceStarted;
            _networkEvents.LocalPlayerCompleteRace += LogLocalPlayerCompleteRace;
            _networkEvents.AllPlayersCompleteRace += LogAllPlayersCompleteRace;
        }

        if (networkManagerEventsLogs)
        {
            _onClientConnected = id =>
                Debug.Log($"Events: OnClientConnected {id} by {NetworkManager.Singleton.LocalClientId} at {Now()}");
            _onConnectionEvent = (_, data) =>
                Debug.Log($"Events: OnConnectionEvent {data.ClientId} {data.EventType} at {Now()}");
            _onServerStarted = () =>
                Debug.Log($"Events: OnServerStarted by {NetworkManager.Singleton.LocalClientId} at {Now()}");
            _onServerStopped = _ =>
                Debug.Log($"Events: OnServerStopped by {NetworkManager.Singleton.LocalClientId} at {Now()}");
            _onClientStarted = () =>
                Debug.Log($"Events: OnClientStarted by {NetworkManager.Singleton.LocalClientId} at {Now()}");
            _onClientStopped = _ =>
                Debug.Log($"Events: OnClientStopped by {NetworkManager.Singleton.LocalClientId} at {Now()}");

            NetworkManager.Singleton.OnClientConnectedCallback += _onClientConnected;
            NetworkManager.Singleton.OnConnectionEvent += _onConnectionEvent;
            NetworkManager.Singleton.OnServerStarted += _onServerStarted;
            NetworkManager.Singleton.OnServerStopped += _onServerStopped;
            NetworkManager.Singleton.OnClientStarted += _onClientStarted;
            NetworkManager.Singleton.OnClientStopped += _onClientStopped;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (inGameEventsLogs && _networkEvents != null)
        {
            _networkEvents.OnPlayerSpawned -= LogPlayerSpawned;
            _networkEvents.OnGameStarted -= LogGameStarted;
            _networkEvents.OnSceneDataInitialized -= LogSceneDataInitialized;
            _networkEvents.OnRaceStarted -= LogRaceStarted;
            _networkEvents.LocalPlayerCompleteRace -= LogLocalPlayerCompleteRace;
            _networkEvents.AllPlayersCompleteRace -= LogAllPlayersCompleteRace;
        }

        if (networkManagerEventsLogs && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= _onClientConnected;
            NetworkManager.Singleton.OnConnectionEvent -= _onConnectionEvent;
            NetworkManager.Singleton.OnServerStarted -= _onServerStarted;
            NetworkManager.Singleton.OnServerStopped -= _onServerStopped;
            NetworkManager.Singleton.OnClientStarted -= _onClientStarted;
            NetworkManager.Singleton.OnClientStopped -= _onClientStopped;
        }
    }

    private void LogPlayerSpawned(object sender, EventArgs e) =>
        Debug.Log($"Events: OnPlayerSpawned by {NetworkManager.Singleton.LocalClientId} at {Now()}");
    private void LogGameStarted(object sender, EventArgs e) =>
        Debug.Log($"Events: OnGameStarted by {NetworkManager.Singleton.LocalClientId} at {Now()}");
    private void LogSceneDataInitialized(object sender, EventArgs e) =>
        Debug.Log($"Events: OnSetupScene by {NetworkManager.Singleton.LocalClientId} at {Now()}");
    private void LogRaceStarted(object sender, EventArgs e) =>
        Debug.Log($"Events: OnRaceStarted by {NetworkManager.Singleton.LocalClientId} at {Now()}");
    private void LogLocalPlayerCompleteRace(object sender, EventArgs e) =>
        Debug.Log($"Events: LocalPlayerCompleteRace by {NetworkManager.Singleton.LocalClientId} at {Now()}");
    private void LogAllPlayersCompleteRace(object sender, EventArgs e) =>
        Debug.Log($"Events: AllPlayersCompleteRace by {NetworkManager.Singleton.LocalClientId} at {Now()}");

    private static string Now() =>
        DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using QFSW.QC;
using Unity.Netcode;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Analytics;

public class NetworkEvents : NetworkBehaviour
{
    public event EventHandler OnPlayerSpawned;
    public event EventHandler OnGameStarted;
    public event EventHandler OnSceneDataInitialized;
    public event EventHandler OnRaceStarted;
    public event EventHandler LocalPlayerCompleteRace;
    public event EventHandler AllPlayersCompleteRace;

    [SerializeField] private bool networkManagerEventsLogs;
    [SerializeField] private  bool inGameEventsLogs;
    
    private GameManager _gameManager;
    private LobbyPage _lobbyPage;
    private NetworkRpc _networkRpc;
    private NetworkData _networkData;
    
    private void Awake()
    {
        _gameManager = ReferenceManager.Get<GameManager>();
        _lobbyPage = ReferenceManager.Get<LobbyPage>();
        _networkRpc = ReferenceManager.Get<NetworkRpc>();
        _networkData = ReferenceManager.Get<NetworkData>();

        if (inGameEventsLogs)
        {
            OnPlayerSpawned += delegate
            {
                AnalyticsService.Instance.RecordEvent("OnPlayerSpawned");
                Debug.Log($"Events: OnPlayerSpawned by {NetworkManager.Singleton.LocalClientId} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };
            OnGameStarted += delegate
            {
                AnalyticsService.Instance.RecordEvent("OnGameStarted");
                Debug.Log($"Events: OnGameStarted by {NetworkManager.Singleton.LocalClientId} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };
            OnSceneDataInitialized += delegate
            {
                AnalyticsService.Instance.RecordEvent("OnSetupScene");
                Debug.Log($"Events: OnSetupScene by {NetworkManager.Singleton.LocalClientId} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };
            OnRaceStarted += delegate
            {
                AnalyticsService.Instance.RecordEvent("OnRaceStarted");
                Debug.Log($"Events: OnRaceStarted by {NetworkManager.Singleton.LocalClientId} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };
            LocalPlayerCompleteRace += delegate
            {
                AnalyticsService.Instance.RecordEvent("LocalPlayerCompleteRace");
                Debug.Log($"Events: LocalPlayerCompleteRace by {NetworkManager.Singleton.LocalClientId} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };
            AllPlayersCompleteRace += delegate
            {
                AnalyticsService.Instance.RecordEvent("AllPlayersCompleteRace");
                Debug.Log($"Events: AllPlayersCompleteRace by {NetworkManager.Singleton.LocalClientId} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };
        }
    }

    public override void OnNetworkSpawn()
    {
        if (networkManagerEventsLogs)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += delegate(ulong clientIndex)
            {
                Debug.Log($"Events: OnClientConnected {clientIndex} by {NetworkManager.Singleton.LocalClientId} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };

            NetworkManager.Singleton.OnConnectionEvent += (manager, data) =>
            {
                Debug.Log($"Events: OnConnectionEvent {data.ClientId} {data.EventType} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };
        
            NetworkManager.Singleton.OnServerStarted += delegate
            {
                Debug.Log($"Events: OnServerStarted by {NetworkManager.Singleton.LocalClientId} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };
        
            NetworkManager.Singleton.OnServerStopped += delegate
            {
                Debug.Log($"Events: OnServerStopped by {NetworkManager.Singleton.LocalClientId} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };
        
            NetworkManager.Singleton.OnClientStarted += delegate
            {
                Debug.Log($"Events: OnClientStarted by {NetworkManager.Singleton.LocalClientId} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };
            
            NetworkManager.Singleton.OnClientStopped += delegate
            {
                Debug.Log($"Events: OnClientStopped by {NetworkManager.Singleton.LocalClientId} at {DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            };
        }

        if (IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += delegate(ulong clientIndex)
            {
                if (NetworkManager.Singleton.ConnectedClientsList.Count >= _gameManager.TotalNumberOfPlayers)
                {
                    _lobbyPage.Show();
                }
            };
            
            NetworkManager.Singleton.OnConnectionEvent += delegate(NetworkManager manager, ConnectionEventData data) 
            {
                _networkRpc.SetPlayerDataServerRpc(new PlayerData()
                {
                    id = data.ClientId,
                    isConnected = true,
                    isReadyForGame = _networkData.GetLocalPlayerReadyStatus(),
                    raceCompleteTime = _networkData.GetLocalPlayerRaceEndTime()
                });
            };
        }
    }

    public void SceneSetup()
    {
        OnSceneDataInitialized?.Invoke(this,EventArgs.Empty);
    }
    
    public async void StartRace()
    {
        Debug.Log("StartRace");
        OnGameStarted?.Invoke(this, EventArgs.Empty);
                    
        await UniTask.Delay(TimeSpan.FromSeconds(_gameManager.GameStartCountDown));
            
        OnRaceStarted?.Invoke(this, EventArgs.Empty);
    }
    
    public void OnPlayerNetworkObjectSpawned()
    {
        OnPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }
    
    public void OnLocalPlayerCompleteRace()
    {
        LocalPlayerCompleteRace?.Invoke(this, EventArgs.Empty);
    }
    
    public void OnAllPlayersCompleteRace()
    {
        AllPlayersCompleteRace?.Invoke(this, EventArgs.Empty);
    }
    
    [Command("Disconnect")]
    void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
}

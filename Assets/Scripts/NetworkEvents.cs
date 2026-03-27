using System;
using Cysharp.Threading.Tasks;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;

public class NetworkEvents : NetworkBehaviour
{
    public event EventHandler OnPlayerSpawned;
    public event EventHandler OnGameStarted;
    public event EventHandler OnSceneDataInitialized;
    public event EventHandler OnRaceStarted;
    public event EventHandler LocalPlayerCompleteRace;
    public event EventHandler AllPlayersCompleteRace;

    private GameManager _gameManager;

    private void Awake()
    {
        ReferenceManager.Register(this);
        _gameManager = ReferenceManager.Get<GameManager>();
    }

    private void OnDestroy()
    {
        ReferenceManager.Unregister(this);
    }

    public void SceneSetup()
    {
        Debug.Log($"[NetworkEvents] SceneSetup called by client {NetworkManager.Singleton.LocalClientId}");
        OnSceneDataInitialized?.Invoke(this, EventArgs.Empty);
    }

    public async void StartRace()
    {
        Debug.Log($"[NetworkEvents] StartRace called by client {NetworkManager.Singleton.LocalClientId}");
        OnGameStarted?.Invoke(this, EventArgs.Empty);

        await UniTask.Delay(TimeSpan.FromSeconds(_gameManager.GameStartCountDown));

        Debug.Log($"[NetworkEvents] OnRaceStarted firing for client {NetworkManager.Singleton.LocalClientId}");
        OnRaceStarted?.Invoke(this, EventArgs.Empty);
    }

    public void OnPlayerNetworkObjectSpawned()
    {
        Debug.Log($"[NetworkEvents] OnPlayerNetworkObjectSpawned for client {NetworkManager.Singleton.LocalClientId}");
        OnPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void OnLocalPlayerCompleteRace()
    {
        Debug.Log($"[NetworkEvents] OnLocalPlayerCompleteRace for client {NetworkManager.Singleton.LocalClientId}");
        LocalPlayerCompleteRace?.Invoke(this, EventArgs.Empty);
    }

    public void OnAllPlayersCompleteRace()
    {
        Debug.Log($"[NetworkEvents] OnAllPlayersCompleteRace for client {NetworkManager.Singleton.LocalClientId}");
        AllPlayersCompleteRace?.Invoke(this, EventArgs.Empty);
    }

    [Command("Disconnect")]
    private void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
}

using System;
using Unity.Services.Analytics;
using UnityEngine;

/// <summary>
/// Listens to NetworkEvents and records analytics. Attach alongside NetworkEvents.
/// </summary>
public class AnalyticsTracker : MonoBehaviour
{
    private NetworkEvents _networkEvents;

    private void Awake()
    {
        _networkEvents = ReferenceManager.Get<NetworkEvents>();
        _networkEvents.OnPlayerSpawned += OnPlayerSpawned;
        _networkEvents.OnGameStarted += OnGameStarted;
        _networkEvents.OnSceneDataInitialized += OnSceneDataInitialized;
        _networkEvents.OnRaceStarted += OnRaceStarted;
        _networkEvents.LocalPlayerCompleteRace += OnLocalPlayerCompleteRace;
        _networkEvents.AllPlayersCompleteRace += OnAllPlayersCompleteRace;
    }

    private void OnDestroy()
    {
        if (_networkEvents == null) return;
        _networkEvents.OnPlayerSpawned -= OnPlayerSpawned;
        _networkEvents.OnGameStarted -= OnGameStarted;
        _networkEvents.OnSceneDataInitialized -= OnSceneDataInitialized;
        _networkEvents.OnRaceStarted -= OnRaceStarted;
        _networkEvents.LocalPlayerCompleteRace -= OnLocalPlayerCompleteRace;
        _networkEvents.AllPlayersCompleteRace -= OnAllPlayersCompleteRace;
    }

    private void OnPlayerSpawned(object sender, EventArgs e) =>
        AnalyticsService.Instance.RecordEvent("OnPlayerSpawned");

    private void OnGameStarted(object sender, EventArgs e) =>
        AnalyticsService.Instance.RecordEvent("OnGameStarted");

    private void OnSceneDataInitialized(object sender, EventArgs e) =>
        AnalyticsService.Instance.RecordEvent("OnSetupScene");

    private void OnRaceStarted(object sender, EventArgs e) =>
        AnalyticsService.Instance.RecordEvent("OnRaceStarted");

    private void OnLocalPlayerCompleteRace(object sender, EventArgs e) =>
        AnalyticsService.Instance.RecordEvent("LocalPlayerCompleteRace");

    private void OnAllPlayersCompleteRace(object sender, EventArgs e) =>
        AnalyticsService.Instance.RecordEvent("AllPlayersCompleteRace");
}

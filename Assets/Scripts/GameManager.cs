using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PlatformCharacterController;
using Unity.Netcode;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("GameData")]
    public SoPrefabsData prefabsDataValue;
    public int blockSize = 100;
    public float[] PlayerLaneXPosition;
    public readonly int TotalNumberOfPlayers = 2;
    public GameDifficulty Difficulty;
    
    [Header("RaceData")]
    
    public readonly int GameStartCountDown = 3;
    public bool HasRaceStarted, HasRaceEnded;
    private DateTime raceStartDateTime, raceEndDateTime;
    
    [Header("TestCase")] 
    public readonly bool TestInMobile = true;
    public bool isCloneClient = false;

    private NetworkEvents _networkEvents;
    private NetworkData _networkData;
    private NetworkRpc _networkRpc;

    private void Awake()
    {
        _networkData = ReferenceManager.Get<NetworkData>();
        _networkEvents = ReferenceManager.Get<NetworkEvents>();
        _networkRpc = ReferenceManager.Get<NetworkRpc>();

        _networkEvents.OnRaceStarted += delegate(object sender, EventArgs args)
        {
            SetRaceStartDateTime(DateTime.Now);
        };
        
        _networkEvents.LocalPlayerCompleteRace += delegate
        {
            SetRaceEndDateTime(DateTime.Now);
        };
    }

    private void SetRaceStartDateTime(DateTime dateTime)
    {
        if (HasRaceStarted)
        {
            Debug.LogError("Trying to set game start time again! This is been rejected!");
            return;
        }
            
        HasRaceStarted = true;
        raceStartDateTime = dateTime;
    }
    
    private void SetRaceEndDateTime(DateTime dateTime)
    {
        if (HasRaceEnded)
        {
            Debug.LogError("Trying to set game end time again! This is been rejected!");
            return;
        }
        HasRaceEnded = true;
        raceEndDateTime = dateTime;
        _networkRpc.SetPlayerDataServerRpc(new PlayerData()
        {
            id = NetworkManager.Singleton.LocalClientId,
            isConnected = _networkData.GetLocalPlayerConnectionStatus(),
            isReadyForGame = _networkData.GetLocalPlayerReadyStatus(),
            raceCompleteTime = GetCurrentTime()
        });
    }

    public bool CheckAllPlayersCompleteRace()
    {
        return true;
    }
    
    public string GetCurrentTime()
    {
        if (!HasRaceStarted)
            return "";

        var endTime = (HasRaceEnded) ? raceEndDateTime : DateTime.Now;
        
        TimeSpan currentTime = endTime - raceStartDateTime;

        return $"{currentTime.Minutes} : {currentTime.Seconds} : {currentTime.Milliseconds}";
    }
}

public enum GameMode
{
    RaceWithTime,
    Multiplayer
}


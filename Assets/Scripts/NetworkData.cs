using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using QFSW.QC;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class NetworkData : NetworkBehaviour
{
    
    public NetworkList<PlayerData> playersDataList;
    
    private NetworkEvents _networkEvents;
    private GameManager _gameManager;
    private NetworkRpc _networkRpc;

    private bool isSceneSetupCompleted;

    private void Awake()
    {
        _networkEvents = ReferenceManager.Get<NetworkEvents>();
        _gameManager = ReferenceManager.Get<GameManager>();
        _networkRpc = ReferenceManager.Get<NetworkRpc>();

        playersDataList = new NetworkList<PlayerData>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            var playerData = new PlayerData
            {
                id = NetworkManager.Singleton.LocalClientId,
                isConnected = false,
                isReadyForGame = false,
                raceCompleteTime = "",
            };
            playersDataList.Add(playerData);
            playersDataList.SetDirty(true);
            
            NetworkManager.Singleton.OnClientConnectedCallback += delegate(ulong clientIndex)
            {
                if (clientIndex != NetworkManager.Singleton.LocalClientId)
                {
                    _networkRpc.SetPlayerDataServerRpc(new PlayerData()
                    {
                        id = clientIndex,
                        isConnected = false,
                        isReadyForGame = false,
                        raceCompleteTime = ""
                    });
                }
            };
        }
    }

    public override void OnDestroy()
    {
        playersDataList?.Dispose();
    }

    [Command]
    private void DebugSampleData()
    {
        _networkRpc.SetPlayerDataServerRpc(new PlayerData()
        {
            id = 0,
            isConnected = true,
            isReadyForGame = false,
            raceCompleteTime = ""
        });
    }

    public void SetPlayerData(PlayerData playerData)
    {
        for (int i = 0; i < playersDataList.Count; i++)
        {
            if (playersDataList[i].id == playerData.id)
            {
                playersDataList[i] = playerData;

                if (!isSceneSetupCompleted && CheckAllPlayersConnected())
                {
                    isSceneSetupCompleted = true;   
                    _networkEvents.SceneSetup();
                }

                return;
            }
        }
        playersDataList.Add(playerData);
    }

    [Command("DebugPlayersDataList")]
    private void DebugPlayersDataList()
    {
        for (int i = 0; i < playersDataList.Count; i++)
        {
            Debug.Log($"Player{playersDataList[i].id}\t{playersDataList[i].isConnected}\t{playersDataList[i].isReadyForGame}\t{playersDataList[i].raceCompleteTime}");
        }
    }
    
    public bool GetLocalPlayerConnectionStatus()
    {
        for (int i = 0; i < playersDataList.Count; i++)
        {
            if (playersDataList[i].id == NetworkManager.Singleton.LocalClientId)
            {
                return playersDataList[i].isConnected;
            }
        }

        return false;
    }
    
    public bool GetLocalPlayerReadyStatus()
    {
        for (int i = 0; i < playersDataList.Count; i++)
        {
            if (playersDataList[i].id == NetworkManager.Singleton.LocalClientId)
            {
                return playersDataList[i].isReadyForGame;
            }
        }

        return false;
    }
    
    public string GetLocalPlayerRaceEndTime()
    {
        for (int i = 0; i < playersDataList.Count; i++)
        {
            if (playersDataList[i].id == NetworkManager.Singleton.LocalClientId)
            {
                return playersDataList[i].raceCompleteTime.ToString();
            }
        }

        return "";
    }
    
    public string GetRaceEndTime(ulong clientId)
    {
        for (int i = 0; i < playersDataList.Count; i++)
        {
            if (playersDataList[i].id == clientId)
            {
                return playersDataList[i].raceCompleteTime.ToString();
            }
        }

        return "";
    }

    public bool AllPlayersReady()
    {
        for (int i = 0; i < playersDataList.Count; i++)
        {
            if (!playersDataList[i].isReadyForGame)
                return false;
        }

        return true;
    }

    private bool CheckAllPlayersConnected()
    {
        if (playersDataList.Count != _gameManager.TotalNumberOfPlayers)
            return false;
            
        for (int i = 0; i < playersDataList.Count; i++)
        {
            if (!playersDataList[i].isConnected)
                return false;
        }

        return true;
    }
}

[Serializable]
public struct PlayerData : INetworkSerializable, System.IEquatable<PlayerData>
{
    public ulong id;
    public bool isConnected;
    public bool isReadyForGame;
    public FixedString32Bytes raceCompleteTime;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out id);
            reader.ReadValueSafe(out isConnected);
            reader.ReadValueSafe(out isReadyForGame);
            reader.ReadValueSafe(out raceCompleteTime);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(id);
            writer.WriteValueSafe(isConnected);
            writer.WriteValueSafe(isReadyForGame);
            writer.WriteValueSafe(raceCompleteTime);
        }
    }
    
    public bool Equals(PlayerData other)
    {
        return id == other.id && isConnected == other.isConnected && isReadyForGame == other.isReadyForGame && raceCompleteTime.Equals(other.raceCompleteTime) ;
    }
    
    public void SetId(ulong id)
    {
        this.id = id;
    }
    
    public void SetConnectionStatus(bool status)
    {
        this.isConnected = status;
    }
    
    public void SetReadyStatus(bool status)
    {
        this.isReadyForGame = status;
    }
    
    public void SetRaceCompleteTime(FixedString32Bytes time)
    {
        this.raceCompleteTime = time;
    }
}
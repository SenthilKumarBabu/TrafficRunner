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
    [SerializeField] private NetworkObject playerPrefab;

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
        Debug.Log($"[NetworkData] OnNetworkSpawn — IsServer={IsServer}, instanceId={GetInstanceID()}");

        if (IsServer)
        {
            // Spawn player objects and add all currently connected clients
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Debug.Log($"[NetworkData] OnNetworkSpawn (Server): spawning player and adding client {clientId} as connected");

                if (playerPrefab != null)
                {
                    var playerObj = Instantiate(playerPrefab);
                    playerObj.SpawnAsPlayerObject(clientId, false);
                }
                else
                {
                    Debug.LogError("[NetworkData] playerPrefab is not assigned!");
                }

                playersDataList.Add(new PlayerData
                {
                    id = clientId,
                    isConnected = true,
                    isReadyForGame = false,
                    raceCompleteTime = "",
                });
            }
            playersDataList.SetDirty(true);

            if (!isSceneSetupCompleted && CheckAllPlayersConnected())
            {
                isSceneSetupCompleted = true;
                _networkEvents.SceneSetup();
            }

            // Handle clients that connect after this point
            NetworkManager.Singleton.OnClientConnectedCallback += delegate(ulong clientIndex)
            {
                if (clientIndex != NetworkManager.Singleton.LocalClientId)
                {
                    Debug.Log($"[NetworkData] Client {clientIndex} connected late, adding to playersDataList");
                    _networkRpc.SetPlayerDataServerRpc(new PlayerData()
                    {
                        id = clientIndex,
                        isConnected = true,
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
        Debug.Log($"[NetworkData] SetPlayerData: id={playerData.id} isConnected={playerData.isConnected} isReady={playerData.isReadyForGame}");
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
                else if (!isSceneSetupCompleted)
                {
                    Debug.Log($"[NetworkData] Not all players connected yet — list count={playersDataList.Count}, required={_gameManager.TotalNumberOfPlayers}");
                }

                return;
            }
        }
        Debug.Log($"[NetworkData] SetPlayerData: id={playerData.id} not found, adding new entry");
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
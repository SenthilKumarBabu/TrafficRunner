using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlatformCharacterController;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CountryManager : NetworkBehaviour
{
    [SerializeField] private SoCountryData countryData;
    [SerializeField] private SoDifficultyData difficultyData;
    [Header("Country")]
    private int totalNumberOfBlocksCreated = 0;

    [Header("Sector")] 
    [SerializeField] private List<Sector> usedSectorList;

    private GameManager gameManager;
    private NetworkEvents _networkEvents;
    private NetworkData _networkData;
    private NetworkRpc _networkRpc;

    private void Awake()
    {
        ReferenceManager.Register(this);
        gameManager = ReferenceManager.Get<GameManager>();
        _networkEvents = ReferenceManager.Get<NetworkEvents>();
        _networkData = ReferenceManager.Get<NetworkData>();
        _networkRpc = ReferenceManager.Get<NetworkRpc>();

        _networkEvents.OnSceneDataInitialized += delegate
        { 
            Initialize();
        };
        
        _networkEvents.OnGameStarted += delegate
        {
            _networkRpc.CreateCountryClientRpc();
        };

        numberOfBlocks.OnValueChanged += (value, newValue) =>
        {  
            Debug.Log($"NumberOfBlocks Value Changed from {value} to {newValue}");
            CheckCountryDataAndStartGame();
        };
        
        sectorIndex.OnValueChanged += (value, newValue) =>
        {  
            Debug.Log($"SectorIndex Value Changed from {value} to {newValue}");
            CheckCountryDataAndStartGame();
        };
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            numberOfBlocks.Value = -1;
            sectorIndex.Value = -1;
        }
        else
        {
            WaitForNetworkRpcAndCheck().Forget();
        }
    }

    private async UniTaskVoid WaitForNetworkRpcAndCheck()
    {
        await UniTask.WaitUntil(() => _networkRpc != null && _networkRpc.IsSpawned);
        CheckCountryDataAndStartGame();
    }

    private void CheckCountryDataAndStartGame()
    {
        if (numberOfBlocks.Value != -1 && sectorIndex.Value != -1)
        {
            if (_networkRpc == null || !_networkRpc.IsSpawned) return;
            _networkRpc.SetPlayerDataServerRpc(new PlayerData()
            {
                id = NetworkManager.Singleton.LocalClientId,
                isConnected = _networkData.GetLocalPlayerConnectionStatus(),
                isReadyForGame = true,
                raceCompleteTime = _networkData.GetLocalPlayerRaceEndTime()
            });
            _networkRpc.CheckForStartRaceServerRpc(new ServerRpcParams());
        }
    }

    private void Initialize()
    {
        difficultyData = countryData.difficultyList[(int)gameManager.Difficulty];
        numberOfBlocks.Value = (sbyte)Random.Range(difficultyData.numberOfBlocksSpawnMinMax.x, difficultyData.numberOfBlocksSpawnMinMax.y);
        sectorIndex.Value = (sbyte)Random.Range(0, countryData.themeList.Count);
    }

    public NetworkVariable<sbyte> numberOfBlocks = new NetworkVariable<sbyte>();
    public NetworkVariable<sbyte> sectorIndex = new NetworkVariable<sbyte>();

    public void CreateCountry()
    {
        Debug.Log($"CreateCountryClientRpc called {NetworkManager.Singleton.LocalClientId} NumberOfBlocks -> {numberOfBlocks.Value}");
        var insSector = CreateSector();
        usedSectorList.Add(insSector);
    }

    private Sector CreateSector()
    {
        var sectorParent = new GameObject($"Sector{(SectorTheme)sectorIndex.Value}");
        sectorParent.transform.SetParent(this.transform);
        var insSector = sectorParent.AddComponent<Sector>();
        var sectorThemeList = new List<Theme>();
        var sectorBlockList = new List<Block>();
        
        for (int i = 0; i < (int)numberOfBlocks.Value; i++)
        {
            var themeIndex = sectorIndex;
            var insTheme = countryData.themeList[(int)themeIndex.Value].Create(gameManager, totalNumberOfBlocksCreated);
            insTheme.transform.SetParent(sectorParent.transform);
            sectorThemeList.Add(insTheme);

            var currentBlockType = (i == 0) ? BlockType.Beginning :
                (i == (int)numberOfBlocks.Value - 1) ? BlockType.Finish : BlockType.Middle;

            var currentBlockDataList = countryData.GetBlockDataList(currentBlockType);
            
            var blockIndex = Utils.GetRandomIndex(currentBlockDataList);
            var insBlock = currentBlockDataList[blockIndex].Create(gameManager, totalNumberOfBlocksCreated);
            insBlock.transform.SetParent(sectorParent.transform);
            sectorBlockList.Add(insBlock);
            
            totalNumberOfBlocksCreated++;
        }
        
        var sectorData = new SectorData()
        {
            Theme = (SectorTheme)(int)sectorIndex.Value,
            StartPosition = totalNumberOfBlocksCreated * gameManager.blockSize,
            EndPosition = (totalNumberOfBlocksCreated + (int)numberOfBlocks.Value) * gameManager.blockSize,
            totalBlocksToBeShown = (int)numberOfBlocks.Value
        };
        
        insSector.Initialize(sectorThemeList, sectorBlockList, sectorData);

        return insSector;
    }

    private void OnDestroy()
    {
        ReferenceManager.Unregister(this);
    }
}


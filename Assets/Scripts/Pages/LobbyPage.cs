using System;
using System.Collections;
using System.Collections.Generic;
using PlatformCharacterController;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPage : MonoBehaviour
{
   [SerializeField] private GameObject holder;
   
   [SerializeField] private TMP_Dropdown gameDifficultyDropdown;

   [SerializeField] private Button startGameButton;

   private PlayerManager playerManager;
   private LobbyManager lobbyManager;
   private NetworkEvents networkEvents;
   private GameManager _gameManager;
   private NetworkRpc _networkRpc;
   private UnityTransport _unityTransport;

   private void Awake()
   {
      playerManager = ReferenceManager.Get<PlayerManager>();
      lobbyManager = ReferenceManager.Get<LobbyManager>();
      networkEvents = ReferenceManager.Get<NetworkEvents>();
      _gameManager = ReferenceManager.Get<GameManager>();
      _networkRpc = ReferenceManager.Get<NetworkRpc>();
      _unityTransport = ReferenceManager.Get<UnityTransport>();
      
      startGameButton.onClick.AddListener(StartGame);
      
      lobbyManager.OnJoinedLobby += delegate
      {
         Show();
      };

      networkEvents.OnGameStarted += delegate
      {
         Hide();
      };
         
      gameDifficultyDropdown.onValueChanged.AddListener(GameDifficultyDropdownValueChanged);
   }

   private void GameDifficultyDropdownValueChanged(Int32 value)
   {
      /*PlayerManager.LocalPlayerInstance.photonView.RPC(RpcFuncName.ChangeDifficulty.ToString(), RpcTarget.All, value);*/
   }

   public void GameDifficultyChangedByServer(int value)
   {
      gameDifficultyDropdown.value = value;
   }

   private void StartGame()
   {
      if (_unityTransport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport)
      {
         startGameButton.gameObject.SetActive(false);
         lobbyManager.InitializeGameStart();
      }
   }

   public void Show()
   {
      holder.SetActive(true);
      if (_unityTransport.Protocol == UnityTransport.ProtocolType.UnityTransport)
      {
         startGameButton.gameObject.SetActive(_networkRpc.IsHost);
         gameDifficultyDropdown.interactable = _networkRpc.IsHost;
      }
      else
      {
         startGameButton.gameObject.SetActive(lobbyManager.IsLobbyHost());
         gameDifficultyDropdown.interactable = lobbyManager.IsLobbyHost();
      }
   }
   
   private void Hide()
   {
      holder.SetActive(false);
   }
}

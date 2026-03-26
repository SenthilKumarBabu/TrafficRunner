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

   private LobbyManager lobbyManager;
   private UnityTransport _unityTransport;

   private void Awake()
   {
      lobbyManager = ReferenceManager.Get<LobbyManager>();
      _unityTransport = ReferenceManager.Get<UnityTransport>();

      startGameButton.onClick.AddListener(StartGame);

      lobbyManager.OnJoinedLobby += delegate
      {
         Show();
      };

      gameDifficultyDropdown.onValueChanged.AddListener(GameDifficultyDropdownValueChanged);
   }

   private void Start()
   {
      if (NetworkManager.Singleton == null) return;
      NetworkManager.Singleton.OnClientConnectedCallback += delegate(ulong clientId)
      {
         if (_unityTransport.Protocol == UnityTransport.ProtocolType.UnityTransport)
            Show();
      };
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
      Debug.Log("[LobbyPage] StartGame clicked");
      if (_unityTransport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport)
      {
         startGameButton.gameObject.SetActive(false);
         lobbyManager.InitializeGameStart();
      }
      else
      {
         startGameButton.gameObject.SetActive(false);
         NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
      }
   }

   public void Show()
   {
      holder.SetActive(true);
      if (_unityTransport.Protocol == UnityTransport.ProtocolType.UnityTransport)
      {
         bool isHost = NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost;
         startGameButton.gameObject.SetActive(isHost);
         gameDifficultyDropdown.interactable = isHost;
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using ParrelSync;
#endif
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Analytics;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using Random = UnityEngine.Random;

public class MenuPage : MonoBehaviour
{
    [SerializeField] private GameObject holder;
    [SerializeField] private GameObject playButtonsGroup;
    [SerializeField] private TMP_InputField userNameIf;

    private LobbyManager lobbyManager;
    private GameManager _gameManager;
    private UnityTransport _unityTransport;
    
    private void Awake()
    {
        lobbyManager = ReferenceManager.Get<LobbyManager>();
        _gameManager = ReferenceManager.Get<GameManager>();
        _unityTransport = ReferenceManager.Get<UnityTransport>();
        
        userNameIf.onEndEdit.AddListener(UserNameInputFieldEndEdit);
    }

    private void Start()
    {
        Show();
    }

    private void Show()
    {
        holder.SetActive(true);
        userNameIf.text = PlayerPrefs.GetString(PlayerPrefsKey.PlayerName.ToString(),"");
    }

    private void Hide()
    {
        holder.SetActive(false);
    }

    public async void PlayButtonClicked()
    {
        if (_unityTransport.Protocol == UnityTransport.ProtocolType.UnityTransport)
        {
#if UNITY_EDITOR
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetEnvironmentName("production");

            await UnityServices.InitializeAsync(initializationOptions);
        
            Debug.Log("Initialization success");
        
            AnalyticsService.Instance.StartDataCollection();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
            Debug.Log(AnalyticsService.Instance.GetAnalyticsUserID());
            UnityServices.ExternalUserId = AnalyticsService.Instance.GetAnalyticsUserID();
            
            if (_gameManager.isCloneClient)
            {
                if (!ClonesManager.IsClone())
                    NetworkManager.Singleton.StartHost();
                else
                {
                    NetworkManager.Singleton.StartClient();
                }
            }
            else
            {
                if (!ClonesManager.IsClone())
                    NetworkManager.Singleton.StartClient();
                else
                {
                    NetworkManager.Singleton.StartHost();
                }
            }
#endif
        }
        else
        {
            lobbyManager.Authenticate();
        }
        
        Hide();
    }

    private void UserNameInputFieldEndEdit(string value)
    {
        if (value.Length < 5)
        {
            userNameIf.text = PlayerPrefs.GetString(PlayerPrefsKey.PlayerName.ToString());
            return;
        }

        lobbyManager.UpdatePlayerName(value);
        userNameIf.text = value;
    }
}

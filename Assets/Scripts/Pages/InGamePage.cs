using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using PlatformCharacterController;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGamePage : MonoBehaviour
{
    [SerializeField] private GameObject holder;
    [SerializeField] private GameObject scoreTextParent;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject pingTextParent;
    [SerializeField] private TMP_Text pingText;
    [SerializeField] private GameObject gameStartsInParent;
    [SerializeField] private TMP_Text gameStartsInText;
    [SerializeField] private GameObject mobileInputs;
    [SerializeField] private GameObject waitingForOpponent;

    [SerializeField] private Button jumpButton,dashButton,grabButton;
    [SerializeField] private EventTrigger jetPackEventTrigger, parachuteEventTrigger;

    private MovementCharacterController movementCharacterController;
    private LobbyManager lobbyManager;
    private GameManager gameManager;
    private NetworkEvents _networkEvents;

    private void Awake()
    {
        lobbyManager = ReferenceManager.Get<LobbyManager>();
        gameManager = ReferenceManager.Get<GameManager>();
        _networkEvents = ReferenceManager.Get<NetworkEvents>();

#if UNITY_EDITOR
        if (gameManager.TestInMobile)
        {
            mobileInputs.SetActive(true);
            AssignMobileInputs();
        }
        else
        {
            mobileInputs.SetActive(false);
        }
#elif UNITY_ANDROID
        mobileInputs.SetActive(true);
        AssignMobileInputs();
#else
        mobileInputs.SetActive(false);
#endif

        _networkEvents.OnPlayerSpawned += (sender, args) =>
        {
            Show();
        };
        _networkEvents.OnGameStarted += delegate
        {
            ShowRaceStartsText();
        };

        _networkEvents.LocalPlayerCompleteRace += delegate
        {
            Hide();
        };

        void ShowRaceStartsText()
        {
            Show();
            gameStartsInParent.gameObject.SetActive(true);
            scoreTextParent.gameObject.SetActive(true);
            pingTextParent.gameObject.SetActive(true);
            RefreshGameStartsInText(gameManager.GameStartCountDown);
        }
    }

    private void Update()
    {
        if(!gameManager.HasRaceStarted)
            return;
        
        scoreText.text = gameManager.GetCurrentTime();
        pingText.text = NetworkManager.Singleton.NetworkConfig.NetworkTransport
            .GetCurrentRtt(NetworkManager.ServerClientId).ToString();
    }

    private void Show()
    {
        holder.SetActive(true);
    }

    private void Hide()
    {
        holder.SetActive(false);
    }
    
    private async void RefreshGameStartsInText(int value)
    {
        gameStartsInParent.SetActive(value > 0);
        gameStartsInText.text = $"Game Starts in - {value}";
        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        var countdown = value - 1;
        if(countdown >= 0)
            RefreshGameStartsInText(countdown);
    }

    private void Restart() 
    {
        scoreText.text = 0.ToString();
        pingText.text = 0.ToString();
    }

    private void AssignMobileInputs()
    {
        _networkEvents.OnPlayerSpawned += delegate
        {
            var mobilePlayerInput = PlayerManager.LocalPlayerInstance.GetComponent<MobilePlayerInput>();
            jumpButton.onClick.AddListener(mobilePlayerInput.MakeJump);
            dashButton.onClick.AddListener(mobilePlayerInput.MakeDash);
            grabButton.onClick.AddListener(mobilePlayerInput.DropOrCarryOnButton);

            EventTrigger.Entry jetPackPointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };

            EventTrigger.Entry jetPackPointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };

            EventTrigger.Entry parachutePointerDownEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };

            EventTrigger.Entry parachutePointerUpEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };

            jetPackPointerDownEntry.callback.RemoveAllListeners();
            jetPackPointerDownEntry.callback.AddListener((data) => { mobilePlayerInput.ActiveJetPack(true); });
            jetPackEventTrigger.triggers.Add(jetPackPointerDownEntry);

            jetPackPointerUpEntry.callback.RemoveAllListeners();
            jetPackPointerUpEntry.callback.AddListener((data) => { mobilePlayerInput.ActiveJetPack(false); });
            jetPackEventTrigger.triggers.Add(jetPackPointerUpEntry);

            parachutePointerDownEntry.callback.RemoveAllListeners();
            parachutePointerDownEntry.callback.AddListener((data) => { mobilePlayerInput.ActiveParachute(true); });
            parachuteEventTrigger.triggers.Add(parachutePointerDownEntry);

            parachutePointerUpEntry.callback.RemoveAllListeners();
            parachutePointerUpEntry.callback.AddListener((data) => { mobilePlayerInput.ActiveParachute(false); });
            parachuteEventTrigger.triggers.Add(parachutePointerUpEntry);
        };
    }
}

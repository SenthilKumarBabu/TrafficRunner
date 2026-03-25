using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Android;

public class GameOverPage : MonoBehaviour
{
    [SerializeField] private GameObject holder;
    [SerializeField] private TMP_Text gameStatusText;
    [SerializeField] private TMP_Text scoreText;

    private LobbyManager lobbyManager;
    private GameManager gameManager;
    private NetworkEvents _networkEvents;

    private void Awake()
    {
        lobbyManager = ReferenceManager.Get<LobbyManager>();
        gameManager = ReferenceManager.Get<GameManager>();
        _networkEvents = ReferenceManager.Get<NetworkEvents>();
        
        _networkEvents.LocalPlayerCompleteRace += delegate 
        {
            if (!gameManager.CheckAllPlayersCompleteRace())
            {
                WaitingStateChange();
            }
            else
            {
                GameOverChange();
            }

            Show();
        };

        _networkEvents.AllPlayersCompleteRace += delegate
        {
            GameOverChange();
            Show();
        };
    }

    private void Show()
    {
        scoreText.text = $"Total Time - {gameManager.GetCurrentTime()}";
        holder.SetActive(true);
    }

    private void Hide()
    {
        holder.SetActive(false);
    }

    private void WaitingStateChange()
    {
        gameStatusText.text = $"Waiting for others to complete race";
    }

    private void GameOverChange()
    {
        gameStatusText.text = $"Race Over";
    }

    public void RestartButtonClicked()
    {
        Hide();
    }
}
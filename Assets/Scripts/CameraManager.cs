using System;
using System.Collections;
using System.Collections.Generic;
using PlatformCharacterController;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera lobbyCamera, gameCamera;

    private LobbyManager lobbyManager;
    private NetworkEvents _networkEvents;
    
    private void Awake()
    {
        lobbyManager = ReferenceManager.Get<LobbyManager>();
        _networkEvents = ReferenceManager.Get<NetworkEvents>();
        
        _networkEvents.OnGameStarted += delegate 
        {
            lobbyCamera.gameObject.SetActive(false);
            gameCamera.gameObject.SetActive(true);
            gameCamera.GetComponent<TopDownCamera>().Target = PlayerManager.LocalPlayerInstance.transform;
        };
    }

    public Camera GetMainCamera()
    {
        return gameCamera;
    }
}

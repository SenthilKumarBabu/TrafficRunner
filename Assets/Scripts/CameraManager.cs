using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlatformCharacterController;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;

    private NetworkEvents _networkEvents;

    private void Awake()
    {
        _networkEvents = ReferenceManager.Get<NetworkEvents>();

        _networkEvents.OnGameStarted += async delegate
        {
            gameCamera.gameObject.SetActive(true);
            await UniTask.WaitUntil(() => (UnityEngine.Object)PlayerManager.LocalPlayerInstance != null);
            gameCamera.GetComponent<TopDownCamera>().Target = PlayerManager.LocalPlayerInstance.transform;
        };
    }

    public Camera GetMainCamera()
    {
        return gameCamera;
    }
}

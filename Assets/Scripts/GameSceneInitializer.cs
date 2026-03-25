using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    private UnityTransport _unityTransport;
    
    private void Awake()
    {
        _unityTransport = ReferenceManager.Get<UnityTransport>();
        
        Application.runInBackground = true;

#if UNITY_EDITOR
        if (_unityTransport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport)
        {
            Debug.LogError($"Scene Initialization is incorrect. {_unityTransport.Protocol} is been used");
            return;
        }
#elif UNITY_ANDROID
        if (_unityTransport.Protocol == UnityTransport.ProtocolType.UnityTransport)
        {
            Debug.LogError($"Scene Initialization is incorrect. {_unityTransport.Protocol} is been used");
            return;
        }
#endif
    }
}

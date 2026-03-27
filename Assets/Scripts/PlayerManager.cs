using PlatformCharacterController;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static MovementCharacterController _localPlayer;

    public static MovementCharacterController LocalPlayerInstance
    {
        get => (UnityEngine.Object)_localPlayer != null ? _localPlayer : null;
        set => _localPlayer = value;
    }

    private void Awake()
    {
        ReferenceManager.Register(this);
    }

    private void OnDestroy()
    {
        ReferenceManager.Unregister(this);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
public class SoGameConfig : ScriptableObject
{
    [Header("Mobile Input")]
    public float swipeThreshold = 50f;

    [Header("Lobby Timers")]
    public float lobbyHeartbeatInterval = 15f;
    public float lobbyPollInterval = 1.1f;
}

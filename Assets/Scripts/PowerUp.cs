using System;
using System.Collections;
using System.Collections.Generic;
using PlatformCharacterController;
using UnityEngine;
using UnityEngine.Serialization;

public class PowerUp : MonoBehaviour
{
    public PowerUpData data;
    
    [Tooltip("The speed value to add at the player speed.")]
    public float SpeedPlus = 5;

    [Tooltip("This is the time that the speed plus is active in the player.")]
    public float SprintTime = 4;

    public float InvertControlTime = 3;
    
    public GameObject EnableEffect, DisableEffect;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player.ToString()))
        {
            if (data.type == PowerUpType.InvertControl)
            {
                InvertPlayerMovement(other.GetComponent<MovementCharacterController>());
            }
            else if (data.type == PowerUpType.Sprint)
            {
                SprintPlayer(other.GetComponent<MovementCharacterController>());
            }
        }
    }
    
    private void SprintPlayer(MovementCharacterController player)
    {
        if (EnableEffect)
        {
            Instantiate(EnableEffect, transform.position, transform.rotation);
        }

        player.ChangeSpeedInTime(SpeedPlus, SprintTime);

        Destroy(gameObject);
    }
    
    private void InvertPlayerMovement(MovementCharacterController player)
    {
        if (DisableEffect)
        {
            Instantiate(DisableEffect, player.transform.position, player.transform.rotation, player.transform);
        }

        player.InvertPlayerControls(InvertControlTime);

        Destroy(gameObject);
    }
}

[Serializable]
public struct PowerUpData
{
    public PowerUpType type;
    [HideInInspector] public GameObject powerUp;
    public Vector3 position;
}

public enum PowerUpType
{
    InvertControl,
    Sprint
}
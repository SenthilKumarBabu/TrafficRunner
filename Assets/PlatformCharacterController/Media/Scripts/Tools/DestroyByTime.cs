using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    public float TimeToDestroy = 5;

    public void Awake()
    {
        Invoke("DestroyEffect", TimeToDestroy);
    }
    
    void DestroyEffect()
    {
        Destroy(this.gameObject);
    }
}
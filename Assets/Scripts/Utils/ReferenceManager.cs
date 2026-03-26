using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReferenceManager
{
    public static T Get<T>() where T : MonoBehaviour
    {
        return Object.FindObjectOfType(typeof(T), true) as T;
    }
}

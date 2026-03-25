using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReferenceManager
{
    private static Dictionary<System.Type, MonoBehaviour> _dict;

    public static T Get<T>() where T : MonoBehaviour
    {
        _dict ??= new Dictionary<System.Type, MonoBehaviour>();

        if (_dict.ContainsKey(typeof(T)))
        {
            var temp = _dict[typeof(T)];
            if (temp == null)
            {
                _dict.Remove(typeof(T));
            }
            else
            {
                return temp as T;
            }
        }

        var value = Object.FindObjectOfType(typeof(T), true) as MonoBehaviour;
        if (value != null)
        {
            _dict[typeof(T)] = value;
            return value as T;
        }
        return null;
    }
}

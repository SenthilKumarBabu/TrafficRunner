using System;
using System.Collections.Generic;
using UnityEngine;

public static class ReferenceManager
{
    private static readonly Dictionary<Type, MonoBehaviour> _cache = new Dictionary<Type, MonoBehaviour>();

    public static void Register<T>(T instance) where T : MonoBehaviour
    {
        _cache[typeof(T)] = instance;
    }

    public static void Unregister<T>(T instance) where T : MonoBehaviour
    {
        if (_cache.TryGetValue(typeof(T), out var cached) && cached == instance)
            _cache.Remove(typeof(T));
    }

    public static T Get<T>() where T : MonoBehaviour
    {
        if (_cache.TryGetValue(typeof(T), out var cached) && (UnityEngine.Object)cached != null)
            return cached as T;
        // Fallback for types that don't call Register (e.g. external package classes like UnityTransport)
        var found = Object.FindObjectOfType(typeof(T), true) as T;
        if (found != null) _cache[typeof(T)] = found;
        return found;
    }
}

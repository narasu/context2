using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static Dictionary<string, object> directory = new();
    
    public static void Provide(string _name, object _service)
    {
        directory.TryAdd(_name, _service);
    }

    public static bool TryLocate(string _name, out object _service)
    {
        bool found = directory.TryGetValue(_name, out _service);
        return found;
    }

    public static void Remove(string _name)
    {
        directory.Remove(_name);
    }
}
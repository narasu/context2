using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    private Dictionary<string, object> dictionary = new();

    public T GetVariable<T>(string name)
    {
        if (dictionary.TryGetValue(name, out object value))
        {
            return (T)value;
        }
        return default;
    }

    public void SetVariable<T>(string name, T variable)
    {
        dictionary[name] = variable;
    }
}

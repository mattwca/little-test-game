using System;
using System.Collections.Generic;

namespace Engine.ECS;

public class StateManager
{
    private readonly Dictionary<string, string> _state;

    public StateManager()
    {
        _state = new Dictionary<string, string>();
    }

    public void SetBool(string name, bool value)
    {
        _state[name] = value.ToString();
    }

    public bool GetBool(string name, bool defaultVal = false)
    {
        if (_state.TryGetValue(name, out var value))
        {
            return Convert.ToBoolean(value);
        }

        return defaultVal;
    }
}
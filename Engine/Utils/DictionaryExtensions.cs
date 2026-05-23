using System;
using System.Collections.Generic;

namespace Engine.Utils;

public static class DictionaryExtensions
{
    public static TValue TryGetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> adder
    )
    {
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }

        var newValue = adder();
        dictionary.Add(key, newValue);

        return newValue;
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> CreateFromLists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, List<TKey> keys, List<TValue> values)
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();

            // Ensure both lists are of the same length
            if (keys.Count != values.Count)
            {
                Debug.LogError("Lists have different lengths!");
                return result; // Return an empty dictionary
            }

            // Fill the dictionary
            for (int i = 0; i < keys.Count; i++)
            {
                result.Add(keys[i], values[i]);
            }

            return result;
        }
    }
}
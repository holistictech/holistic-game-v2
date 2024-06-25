using System;
using UnityEngine;

namespace Utilities
{
    public static class PlayerSaveManager
    {
        public static void SavePlayerAttribute<T>(T value, string key)
        {
            if (typeof(T) == typeof(int))
            {
                PlayerPrefs.SetInt(key, Convert.ToInt32(value));
            }
            else if (typeof(T) == typeof(float))
            {
                PlayerPrefs.SetFloat(key, Convert.ToSingle(value));
            }
            else if (typeof(T) == typeof(string))
            {
                PlayerPrefs.SetString(key, Convert.ToString(value));
            }
            else if (typeof(T) == typeof(DateTime))
            {
                PlayerPrefs.SetString(key, ((DateTime)(object)value).ToString("o"));
            }
            else if (typeof(T).IsEnum)
            {
                PlayerPrefs.SetInt(key, Convert.ToInt32(value));
            }
            else
            {
                throw new ArgumentException("Unsupported parameter type");
            }
        }
        
        public static T GetPlayerAttribute<T>(string key, T defaultValue = default(T))
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
            }
            
            if (typeof(T) == typeof(float))
            {
                return (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
            }
            
            if (typeof(T) == typeof(string))
            {
                return (T)(object)PlayerPrefs.GetString(key, (string)(object)defaultValue);
            }

            if (typeof(T) == typeof(DateTime))
            {
                string dateTimeString = PlayerPrefs.GetString(key, defaultValue.ToString());
                return (T)(object)DateTime.Parse(dateTimeString, null, System.Globalization.DateTimeStyles.RoundtripKind);
            }
            
            if (typeof(T).IsEnum)
            {
                int intValue = PlayerPrefs.GetInt(key, Convert.ToInt32(defaultValue));
                return (T)(object)intValue;
            }
            
            throw new ArgumentException("Unsupported parameter type");
        }

    }
}

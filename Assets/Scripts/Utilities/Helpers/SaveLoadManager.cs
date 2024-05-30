using System.IO;
using GridSystem;
using Interactables;
using UnityEngine;
using Newtonsoft.Json;

namespace Utilities
{
    public static class SaveLoadManager
    {
        public static void SaveFarm(FarmData data)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText("HolyFarm.json", json);
        }

        public static FarmData TryLoadFarm()
        {
            string filePath = "HolyFarm.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                FarmData data = JsonConvert.DeserializeObject<FarmData>(jsonData);
                return data;
            }
            else
            {
                Debug.LogWarning("Save file not found: " + filePath);
                return new FarmData();
            }
        }

    }
}

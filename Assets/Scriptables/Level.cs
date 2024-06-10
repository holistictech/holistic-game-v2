using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using static Utilities.Helpers.CommonFields;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Tasks/Level")]
    public class Level : ScriptableObject
    {
        public List<TaskConfig> LevelTasks;
        public List<GameObject> dailySpans;
        public int LevelId;

        public List<TaskConfig> GetAvailableTasks()
        {
            return LevelTasks.Where((item, index) => !item.GetHasCompleted()).ToList();
        }

        public GameObject GetSpanByIndex(int index)
        {
            if(index == dailySpans.Count -1)
                PlayerSaveManager.SavePlayerAttribute(DateTime.Now,$"Day{LevelId}");
            return dailySpans[index];
        }

        public bool HasDailySpansCompleted(int stage)
        {
            return stage >= dailySpans.Count;
        }
    }
}

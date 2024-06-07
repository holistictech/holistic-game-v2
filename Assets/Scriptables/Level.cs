using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Utilities.Helpers.CommonFields;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Tasks/Level")]
    public class Level : ScriptableObject
    {
        public List<TaskConfig> LevelTasks;
        public List<GameObject> dailySpans;
        public int LevelId;
        public int CurrentStage;

        public int GetActivityId()
        {
            return LevelId * DAILY_ACTIVITY_COUNT + CurrentStage;
        }

        public List<TaskConfig> GetAvailableTasks()
        {
            return LevelTasks.Where((item, index) => !item.GetHasCompleted()).ToList();
        }

        public GameObject GetSpanByIndex(int index)
        {
            if (index >= dailySpans.Count)
            {
                return dailySpans[0];
            }
            return dailySpans[index];
        }
    }
}

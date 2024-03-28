using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Utilities.CommonFields;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Tasks/Level")]
    public class Level : ScriptableObject
    {
        public List<TaskConfig> LevelTasks;
        public int LevelId;
        public int CurrentStage;

        public int GetActivityId()
        {
            return LevelId * DAILY_ACTIVITY_COUNT + CurrentStage;
        }

        public List<TaskConfig> GetAvailableTasks()
        {
            return LevelTasks.Where((item, index) => index >= CurrentStage).ToList();
        }
    }
}

using System.Collections.Generic;
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
    }
}

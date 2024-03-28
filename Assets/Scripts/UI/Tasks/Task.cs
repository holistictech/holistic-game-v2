using Scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tasks
{
    public class Task : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI taskField;
        [SerializeField] private TextMeshProUGUI targetField;
        [SerializeField] private Button playButton;

        private TaskConfig _taskConfig;
        public void ConfigureUI(TaskConfig config)
        {
            _taskConfig = config;
            taskField.text = config.Mission;
            targetField.text = $"{config.Cost}";
        }
    }
}

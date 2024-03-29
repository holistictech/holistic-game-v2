using System;
using Scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace UI.Tasks
{
    public class Task : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI taskField;
        [SerializeField] private TextMeshProUGUI targetField;
        [SerializeField] private Button goButton;
        [SerializeField] private TextMeshProUGUI buttonTextField;

        [SerializeField] private TasksPanel taskPanelManager;

        private TaskConfig _taskConfig;

        public static event Action<InteractableConfig> OnTaskCompleted;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void ConfigureUI(TaskConfig config)
        {
            _taskConfig = config;
            taskField.text = config.Mission;
            targetField.text = $"{config.Cost}";
            ConfigureButtonState();
        }

        private void ConfigureButtonState()
        {
            if (PlayerInventory.Instance.Currency >= _taskConfig.Cost)
            {
                MakeButtonGo();
            }
            else
            {
                MakeButtonPlay();
            }
        }

        private void MakeButtonGo()
        {
            buttonTextField.text = "Kur";
            goButton.onClick.AddListener(TryCompleteTask);
        }

        private void MakeButtonPlay()
        {
            buttonTextField.text = "Oyna";
            goButton.onClick.AddListener(RedirectToSpan);
        }

        private void TryCompleteTask()
        {
            OnTaskCompleted?.Invoke(_taskConfig.RewardInteractable);
            taskPanelManager.DisableTaskPopup();
        }

        private void RedirectToSpan()
        {
            taskPanelManager.DisableTaskPopup();
        }

        private void AddListeners()
        {
            
        }

        private void RemoveListeners()
        {
            goButton.onClick.RemoveAllListeners();
        }
    }
}

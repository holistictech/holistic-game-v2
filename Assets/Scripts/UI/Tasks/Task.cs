using System;
using Scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;
using Utilities.Helpers;

namespace UI.Tasks
{
    public class Task : MonoBehaviour
    {
        [SerializeField] private Image taskImage;
        [SerializeField] private TextMeshProUGUI taskField;
        [SerializeField] private TextMeshProUGUI targetField;
        [SerializeField] private Image currencyField;
        [SerializeField] private Sprite[] currencies;
        [SerializeField] private Button goButton;
        [SerializeField] private TextMeshProUGUI buttonTextField;
        
        private TasksPanel _taskPanelManager;

        private TaskConfig _taskConfig;

        public static event Action<TaskConfig> OnTaskCompleted;
        //public static event Action OnSpanRequested;

        private void OnEnable()
        {
            AddListeners(); 
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void ConfigureUI(TaskConfig config, TasksPanel parent)
        {
            _taskPanelManager = parent;
            _taskConfig = config;
            taskField.text = config.Mission;
            targetField.text = $"{config.Cost}";
            currencyField.sprite =
                config.CurrencyType == CommonFields.CurrencyType.Energy ? currencies[0] : currencies[1];
            if (config.TaskSprite != null)
            {
                taskImage.sprite = config.TaskSprite;
                taskImage.gameObject.SetActive(true);
            }
            ConfigureButtonState();
        }

        private void ConfigureButtonState()
        {
            if (PlayerInventory.Instance.Energy >= _taskConfig.Cost)
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
            _taskPanelManager.DisableTaskPopup();
            if (!_taskConfig.CanBeCompleted())
            {
                _taskPanelManager.TriggerWarningHelper(_taskConfig);
            }
            else
            {
                OnTaskCompleted?.Invoke(_taskConfig);
            }
        }

        private void RedirectToSpan()
        {
            _taskPanelManager.RequestSpan();
            //OnSpanRequested?.Invoke();
            //_taskPanelManager.DisableTaskPopup();
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

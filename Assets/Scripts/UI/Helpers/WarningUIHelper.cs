using System;
using Scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utilities.Helpers.CommonFields;

namespace UI.Helpers
{
    public class WarningUIHelper : MonoBehaviour
    {
        [SerializeField] private Image warningPopup;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button redirectButton;
        [SerializeField] private TextMeshProUGUI warningField;

        private TaskConfig _currentConfig;

        public static event Action<bool> OnRedirectToTask;
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void ConfigurePopup(TaskConfig config)
        {
            _currentConfig = config;
            ConfigureWarningField();
            warningPopup.gameObject.SetActive(true);
        }

        private void ConfigureWarningField()
        {
            _currentConfig.SetWarningString();
            warningField.text = _currentConfig.GetWarningString();
        }

        private void DisablePopup()
        {
            ResetFields();
            warningPopup.gameObject.SetActive(false);
        }

        private void ResetFields()
        {
            warningField.text = "";
            _currentConfig = null;
        }

        private void RedirectUser()
        {
            DisablePopup();
            switch (_currentConfig.WarningType)
            {
                case WarningType.TaskDependency:
                    OnRedirectToTask?.Invoke(true);
                    break;
            }
        }

        private void AddListeners()
        {
            closeButton.onClick.AddListener(DisablePopup);
            redirectButton.onClick.AddListener(RedirectUser);
        }

        private void RemoveListeners()
        {
            closeButton.onClick.RemoveListener(DisablePopup);
            redirectButton.onClick.RemoveListener(RedirectUser);
        }
    }
}

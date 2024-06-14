using System;
using DG.Tweening;
using Scriptables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utilities.Helpers.CommonFields;

namespace UI.Helpers
{
    public class WarningUIHelper : MonoBehaviour
    {
        [SerializeField] private Image blackishPanel;
        [SerializeField] private Image warningImage;
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
            blackishPanel.gameObject.SetActive(true);
            warningPopup.transform.DOScale(new Vector3(1, 1, 1), 0.55f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                warningImage.transform.DOShakeScale(0.8f, 0.2f).SetEase(Ease.OutCirc);
            });
        }

        private void ConfigureWarningField()
        {
            _currentConfig.SetWarningString();
            warningField.text = _currentConfig.GetWarningString();
        }

        private void DisablePopup()
        {
            ResetFields();
            blackishPanel.gameObject.SetActive(false);
            warningPopup.transform.localScale = new Vector3(0, 0, 0);
        }

        private void ResetFields()
        {
            warningField.text = "";
            _currentConfig = null;
        }

        private void RedirectUser()
        {
            switch (_currentConfig.WarningType)
            {
                case WarningType.TaskDependency:
                    OnRedirectToTask?.Invoke(true);
                    break;
            }
            DisablePopup();
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

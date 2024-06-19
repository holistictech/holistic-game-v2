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
            _currentConfig.SetWarningString();
            ConfigureWarningField(_currentConfig.GetWarningString());
            AnimatePanel();
        }

        public void ConfigurePopupForUsedSpace()
        {
            string warning = "Bu alanda inşaa edilmiş bina mevcut. Başka bir alan seçmelisin";
            ConfigureWarningField(warning);
            AnimatePanel();
        }

        private void AnimatePanel()
        {
            blackishPanel.gameObject.SetActive(true);
            warningPopup.transform.DOScale(new Vector3(1, 1, 1), 0.55f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                warningImage.transform.DOShakeScale(0.8f, 0.2f).SetEase(Ease.OutCirc);
            });
        }

        private void ConfigureWarningField(string field)
        {
            warningField.text = field;
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
            DisablePopup();
            
            if (_currentConfig != null)
            {
                switch (_currentConfig.WarningType)
                {
                    case WarningType.TaskDependency:
                        OnRedirectToTask?.Invoke(true);
                        break;
                }
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

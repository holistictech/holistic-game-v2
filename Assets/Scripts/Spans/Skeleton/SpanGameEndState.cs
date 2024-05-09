using System.Collections.Generic;
using DG.Tweening;
using Scriptables.Tutorial;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.Skeleton
{
    public class SpanGameEndState : MonoBehaviour, ISpanState
    {
        [SerializeField] private ParticleSystem confettiShower;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Image timeFinishedBanner;
        [SerializeField] private Image finalPopup;
        [SerializeField] private TextMeshProUGUI feedbackField;
        private SpanController _spanController;
        public void Enter(SpanController controller)
        {
            if (_spanController == null)
            {
                _spanController = controller;
                AddListeners();
                EnableUIElements();
                ConfigureUI();
            }
        }

        private void ConfigureUI()
        {
            confettiShower.Play();
            var trueCount = StatisticsHelper.GetTrueCount();
            var total = StatisticsHelper.GetTotalQuestionCount();

            feedbackField.text = $"{total} soru arasından {trueCount} kadarını başarıyla doğru cevapladık! tebrikler!";
            
            AnimateConfirmButton();
        }

        private void AnimateConfirmButton()
        {
            confirmButton.transform.DOScale(new Vector3(1, 1, 1), 1.4f).SetEase(Ease.OutBounce);
        }

        public void Exit()
        {
        }

        public void SwitchNextState()
        {
            DisableUIElements();
            RemoveListeners();
            _spanController.EndSpan();
        }

        public void TryShowStateTutorial()
        {
        }

        public void EnableUIElements()
        {
            confettiShower.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(true);
            timeFinishedBanner.gameObject.SetActive(true);
            finalPopup.gameObject.SetActive(true);
            feedbackField.gameObject.SetActive(true);
        }

        public void DisableUIElements()
        {
            confettiShower.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);
            timeFinishedBanner.gameObject.SetActive(false);
            finalPopup.gameObject.SetActive(false);
            feedbackField.gameObject.SetActive(false);
            confettiShower.Stop();
        }

        private void AddListeners()
        {
            confirmButton.onClick.AddListener(SwitchNextState);
        }
        
        private void RemoveListeners()
        {
            confirmButton.onClick.RemoveListener(SwitchNextState);   
        }
    }
}

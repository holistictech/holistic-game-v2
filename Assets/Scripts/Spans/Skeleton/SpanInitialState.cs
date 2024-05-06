using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.Tutorial;
using TMPro;
using Tutorial;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Spans.Skeleton
{
    public class SpanInitialState : MonoBehaviour, ISpanState
    {
        [SerializeField] private List<TutorialStep> steps;
        [SerializeField] private GameObject getStartedBanner;
        [SerializeField] private TextMeshProUGUI getStarted;
        [SerializeField] private GameObject getReadyPopup;
        [SerializeField] private TextMeshProUGUI countdownField;
        [SerializeField] private TextMeshProUGUI getReady;

        private SpanController _spanController;
        private Coroutine _countdown;
        private bool _isInitial = true;
        public void Enter(SpanController spanController)
        {
            if (_spanController == null)
            {
                _spanController = spanController;
                if (_spanController.GetTutorialStatus())
                {
                    TryShowStateTutorial();
                }
                else
                {
                    SetStateConfig();
                }
            }
            else
            {
                SetStateConfig();
            }
        }

        private void SetStateConfig()
        {
            EnableUIElements();
            if (_isInitial)
            {
                FadeGetReady();
                _isInitial = false;
            }
            else
            {
                ConfigureUI();
                if (_spanController.GetTutorialStatus())
                {
                    _spanController.TriggerTutorialField("Şimdi sıra sende!");
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        _spanController.SetTutorialCompleted();
                    });
                }
            }
        }

        public void Exit()
        {
            if (_countdown != null)
            {
                StopCoroutine(_countdown);
            }
            ResetFields();
            DisableUIElements();
        }

        public void SwitchNextState()
        {
            _spanController.SwitchState();
        }

        public void TryShowStateTutorial()
        {
            var targets = new List<GameObject>()
            {
                getReadyPopup.gameObject
            };
            var dictionary = new Dictionary<GameObject, TutorialStep>().CreateFromLists(targets, steps);
            SetTutorialField();
            _spanController.TriggerStateTutorial(dictionary, false, SwitchNextState);
        }

        public void EnableUIElements()
        {
            getReadyPopup.gameObject.SetActive(true);
        }

        public void DisableUIElements()
        {
            getStarted.text = "";
            getReadyPopup.gameObject.SetActive(false);
            getStartedBanner.gameObject.SetActive(false);
        }

        private void SetTutorialField()
        {
            getReadyPopup.gameObject.SetActive(true);
            getReady.text = "Hadi başlayalım";
            getReady.DOFade(1, .25f).SetEase(Ease.OutBack);
        }

        private void FadeGetReady()
        {
            getStartedBanner.gameObject.SetActive(true);
            getStarted.text = "HADİ BAŞLAYALIM";
            getStarted.DOFade(1, 0.5f).SetEase(Ease.OutBounce).OnComplete(ConfigureUI);
        }
        
        private void ConfigureUI()
        {
            getReady.text = "HAZIR OL";
            _countdown = StartCoroutine(PlayCountdown());
            getReady.DOFade(1, .5f).SetEase(Ease.OutBounce);
        }

        private IEnumerator PlayCountdown()
        {
            for (int i = 2; i >= 0; i--)
            {
                countdownField.text = $"{i}";
                yield return new WaitForSeconds(1f);
            }
            SwitchNextState();
        }

        private void ResetFields()
        {
            countdownField.text = "";
            Color fieldColor = getReady.color;
            getReady.color = new Color(fieldColor.r, fieldColor.g, fieldColor.b, 0);
        }
    }
}

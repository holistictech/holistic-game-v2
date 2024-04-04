using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spans.Skeleton
{
    public class SpanInitialState : MonoBehaviour, ISpanState
    {
        [SerializeField] private TextMeshProUGUI getStarted;
        [SerializeField] private TextMeshProUGUI countdownField;
        [SerializeField] private TextMeshProUGUI getReady;

        private SpanController _spanController;
        private Coroutine _countdown;
        public void Enter(SpanController spanController)
        {
            EnableUIElements();
            if (_spanController == null)
            {
                _spanController = spanController;
                FadeGetReady();
            }
            ConfigureUI();
            _countdown = StartCoroutine(PlayCountdown());
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

        public void EnableUIElements()
        {
            getReady.gameObject.SetActive(true);
            getStarted.gameObject.SetActive(true);
            countdownField.gameObject.SetActive(true);
        }

        public void DisableUIElements()
        {
            getStarted.text = "";
            getReady.gameObject.SetActive(false);
            getStarted.gameObject.SetActive(false);
            countdownField.gameObject.SetActive(false);
        }

        private void FadeGetReady()
        {
            getStarted.text = "HADİ BAŞLAYALIM";
            getStarted.DOFade(1, 0.5f).SetEase(Ease.OutBack);
        }
        
        private void ConfigureUI()
        {
            getReady.text = "HAZIR OL";
            getReady.DOFade(1, 2f).SetEase(Ease.Linear).OnComplete(() =>
            {
            });
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

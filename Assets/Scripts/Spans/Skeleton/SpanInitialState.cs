using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spans.Skeleton
{
    public class SpanInitialState : MonoBehaviour, ISpanState
    {
        [SerializeField] private GameObject getStartedBanner;
        [SerializeField] private TextMeshProUGUI getStarted;
        [SerializeField] private GameObject getReadyPopup;
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
            else
            {
                ConfigureUI();
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

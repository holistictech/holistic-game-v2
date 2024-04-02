using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Spans.Skeleton
{
    public class SpanInitialState : MonoBehaviour, ISpanState
    {
        [SerializeField] private TextMeshProUGUI getReady;
        [SerializeField] private TextMeshProUGUI countdownField;

        private SpanController _spanController;
        private Coroutine _countdown;
        public void Enter(SpanController spanController)
        {
            FadeGetReady();
            _countdown = StartCoroutine(PlayCountdown());
            SwitchNextState();
        }

        public void Exit()
        {
            if (_countdown != null)
            {
                StopCoroutine(_countdown);
            }
            ResetFields();
        }

        public void SwitchNextState()
        {
            _spanController.SwitchState();
        }

        private void FadeGetReady()
        {
            getReady.text = "Hazırlan";
            getReady.DOFade(1, 0.5f).SetEase(Ease.OutBack);
        }

        private IEnumerator PlayCountdown()
        {
            for (int i = 3; i > 0; i--)
            {
                countdownField.text = $"{i}";
                yield return new WaitForSeconds(1f);
            }
        }

        private void ResetFields()
        {
            getReady.gameObject.SetActive(false);
            countdownField.gameObject.SetActive(false);
        }
    }
}

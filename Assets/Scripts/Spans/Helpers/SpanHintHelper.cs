using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Helpers
{
    public class SpanHintHelper : MonoBehaviour
    {
        [SerializeField] private Image combineBanner;
        [SerializeField] private TextMeshProUGUI combineField;

        public void SetFieldText(string text)
        {
            combineField.text = text;
        }

        public void AnimateBanner(Action onComplete)
        {
            Sequence combineSequence = DOTween.Sequence();
            combineSequence.Append(combineBanner.transform.DOScaleX(1f, 0.5f).SetEase(Ease.OutCirc).OnComplete(() =>
            {
                combineField.gameObject.SetActive(true);
            }));
            combineSequence.Append(combineField.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.5f).OnComplete(
                () =>
                {
                    combineField.gameObject.SetActive(false);
                }));
            combineSequence.Append(combineBanner.transform.DOScaleX(0f, 0.5f).SetEase(Ease.InCirc).OnComplete(() =>
            {
                onComplete?.Invoke();
            }));
        }
    }
}

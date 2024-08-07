using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.QuestionSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace Spans.Helpers
{
    public class SpanHintHelper : MonoBehaviour
    {
        [SerializeField] private Image hintBanner;
        [SerializeField] private TextMeshProUGUI hintField;
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private Image imagePrefab;

        private List<Image> _pooledImages = new List<Image>();
        public void SetFieldText(string text)
        {
            hintField.text = text;
        }

        public void AnimateBanner(Action onComplete)
        {
            Sequence combineSequence = DOTween.Sequence();
            combineSequence.Append(hintBanner.transform.DOScaleX(1f, .7f).SetEase(Ease.OutCirc).OnComplete(() =>
            {
                hintField.gameObject.SetActive(true);
            }));
            combineSequence.Append(hintField.transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0.02f), 0.5f).OnComplete(
                () =>
                {
                    DOVirtual.DelayedCall(1.5f, () =>
                    {
                        hintField.gameObject.SetActive(false);
                    });
                }));
            combineSequence.Append(hintBanner.transform.DOScaleX(0f, .7f).SetEase(Ease.InCirc).OnComplete(() =>
            {
                onComplete?.Invoke();
            }));
        }

        private void EnableHintBanner(int delay, Action onComplete)
        {
            hintBanner.gameObject.SetActive(true);
            DOVirtual.DelayedCall(delay, () =>
            {
                ResetFields();
                hintBanner.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        private Coroutine _displayingRules;
        public void PopulateHintGrid(Dictionary<Question, Question> classes, Action onComplete)
        {
            /*foreach (KeyValuePair<Question, Question> tuple in classes)
            {
                var tempImage = GetAvailableImage();
                tempImage.sprite = tuple.Key.GetCorrectSprite();
                tempImage.gameObject.SetActive(true);
                tempImage = GetAvailableImage();
                tempImage.sprite = (Sprite)tuple.Value.GetQuestionItem();
                tempImage.gameObject.SetActive(true);
            }*/

            _displayingRules = StartCoroutine(DisplaySpanRule(classes, () =>
            {
                onComplete?.Invoke();
            }));
        }

        private IEnumerator DisplaySpanRule(Dictionary<Question, Question> classes, Action onComplete)
        {
            hintBanner.gameObject.SetActive(true);
            foreach (KeyValuePair<Question, Question> tuple in classes)
            {
                AudioManager.Instance.PlayAudioClip((AudioClip)tuple.Key.GetQuestionItem());
                var tempImage = GetAvailableImage();
                tempImage.sprite = (Sprite)tuple.Value.GetQuestionItem();
                tempImage.gameObject.SetActive(true);

                yield return new WaitForSeconds(1.5f);
                tempImage.gameObject.SetActive(false);
            }
            
            ResetFields();
            hintBanner.gameObject.SetActive(false);
            onComplete?.Invoke();
        }

        private Image GetAvailableImage()
        {
            foreach (var element in _pooledImages)
            {
                if (!element.gameObject.activeSelf)
                    return element;
            }

            throw new Exception("No suitable pooled hint image found");
        }

        private void ResetFields()
        {
            foreach (var element in _pooledImages)
            {
                element.sprite = null;
                element.gameObject.SetActive(false);
            }
        }
        
        public void PoolImagesOnNeed()
        {
            for (int i = 0; i < 5; i++)
            {
                var temp = Instantiate(imagePrefab, gridLayout.transform);
                _pooledImages.Add(temp);
                temp.gameObject.SetActive(false);
            }
        }
    }
}

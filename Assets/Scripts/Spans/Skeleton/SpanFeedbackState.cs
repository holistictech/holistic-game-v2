using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.Tutorial;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Random = UnityEngine.Random;

namespace Spans.Skeleton
{
    public class SpanFeedbackState : MonoBehaviour, ISpanState
    {
        [SerializeField] private AudioClip tutorialSuccessFeedback;
        [SerializeField] private List<TutorialStep> steps;
        [SerializeField] private ParticleSystem levelUpEffect;
        [SerializeField] private ParticleSystem successEffect;
        [SerializeField] private ParticleSystem failEffect;
        [SerializeField] private Image feedbackLabel;
        [SerializeField] private TextMeshProUGUI feedbackField;
        [SerializeField] private Sprite wrongSprite;
        [SerializeField] private Sprite correctSprite;
        [SerializeField] private Slider progressBar;
        private SpanController _spanController;

        private Coroutine _progressBar;

        private readonly string[] _successFeedbacks = new string[]
        {
            "Başardın!",
            "Harika!",
            "Çok iyi!",
            "Aferin sana!",
            "Süper!",
            "Tam isabet!",
            "Mükemmel!",
            "Doğru cevap!",
            "İşte bu!",
            "Çok güzel!",
            "Bravo!",
            "Çok akıllıca!",
            "Ne kadar zekisin!",
            //"Çok başarılı!",
            //"Harikasın!",
        };
        private readonly string[] _failFeedbacks = new string[]
        {
            "Bir daha deneyelim!",
            "Çok yaklaştın, tekrar dene!",
            "Az kaldı, bir kez daha dene!",
            "Hiç sorun değil, birlikte doğru cevabı bulalım.",
            "Olur böyle, önemli olan denemek!",
            "Bu sefer olmadı ama doğru yoldasın!",
            "İyi bir denemeydi, şimdi düzeltebiliriz!",
            "Herkes hata yapabilir, önemli olan tekrar denemektir.",
            "Hatalardan öğreniyoruz, bir daha deneyelim",
            "Biraz daha çalışırsak yapabiliriz.",
            "Her zaman ilk seferde olmaz, tekrar deneyelim",
            "Doğru cevaba bir adım daha yaklaştın, devam et!",
            "Bu sefer olmadı ama bir dahaki sefere daha iyi olacak!",
        };
        public void Enter(SpanController controller)
        {
            if (_spanController == null)
            {
                _spanController = controller;
            }

            EnableUIElements();
            PlayEffects();
            if(_spanController.GetTutorialStatus())
                TryShowStateTutorial();
        }


        private float speed = 5f;
        private void PlayEffects()
        {
            if (_spanController.IsAnswerCorrect())
            {
                successEffect.Play();
                progressBar.maxValue = _spanController.GetRoundIndex();
                float maxValue = progressBar.maxValue;
                float target = maxValue / 4f + progressBar.value;
                _progressBar = StartCoroutine(AnimateProgressBar(target, .3f));


                ConfigureFeedbackField(true);
            }
            else
            {
                failEffect.Play();
                _progressBar = StartCoroutine(AnimateProgressBar(0, .3f));
                ConfigureFeedbackField(false);
            }
        }
        
        private IEnumerator AnimateProgressBar(float targetValue, float duration)
        {
            if (targetValue >= progressBar.maxValue)
            {
                StartCoroutine(LerpSlider(targetValue, duration, () =>
                {
                    levelUpEffect.Play();
                    progressBar.value = 0f;
                    _spanController.ResetLevelChangedStatus();
                }));
            }
            else
            {
                StartCoroutine(LerpSlider(targetValue, duration));
            }

            yield return null;

        }

        private IEnumerator LerpSlider(float targetValue, float duration, Action onComplete = null)
        {
            float elapsedTime = 0f;
            float startValue = progressBar.value;
    
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                float newValue = Mathf.Lerp(startValue, targetValue, t);
                
                progressBar.value = newValue;
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            progressBar.value = targetValue;
            onComplete?.Invoke();
        }


        private void ConfigureFeedbackField(bool correct)
        {
            feedbackLabel.sprite = correct ? correctSprite : wrongSprite;
            
            feedbackField.text = GetRandomFeedback(correct);

            feedbackLabel.transform.DOScale(1f, 1.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    _spanController.SwitchState();
                });
            });
        }

        private string GetRandomFeedback(bool isCorrect)
        {
            if (_spanController.GetTutorialStatus())
            {
                AudioManager.Instance.PlayAudioClip(tutorialSuccessFeedback);
                _spanController.SetTutorialCompleted();
                return _successFeedbacks[0];
            }
            else
            {
                var index = Random.Range(0, _successFeedbacks.Length -1);
                return isCorrect ? _successFeedbacks[index] : _failFeedbacks[index];
            }
        }

        public void Exit()
        {
            DisableUIElements();
            if (_progressBar != null)
            {
                StopCoroutine(_progressBar);
            }
        }

        public void SwitchNextState()
        {
            _spanController.SwitchState();
        }

        public void TryShowStateTutorial()
        {
        }

        public void EnableUIElements()
        {
            successEffect.gameObject.SetActive(true);
            failEffect.gameObject.SetActive(true);
            progressBar.gameObject.SetActive(true);
            feedbackLabel.gameObject.SetActive(true);
            feedbackField.gameObject.SetActive(true);
            levelUpEffect.gameObject.SetActive(true);
        }

        public void DisableUIElements()
        {
            successEffect.gameObject.SetActive(false);
            failEffect.gameObject.SetActive(false);
            feedbackLabel.gameObject.SetActive(false);
            levelUpEffect.gameObject.SetActive(false);
            feedbackLabel.transform.localScale = new Vector3(0,0,0);
        }
    }
}

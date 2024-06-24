using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.Tutorial;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;
using Random = UnityEngine.Random;

namespace Spans.Skeleton
{
    public class SpanFeedbackState : MonoBehaviour, ISpanState
    {
        [SerializeField] private AudioClip tutorialSuccessFeedback;
        [SerializeField] private AudioClip yourTurnClip;
        [SerializeField] private List<TutorialStep> steps;
        [SerializeField] private ParticleSystem levelUpEffect;
        [SerializeField] private ParticleSystem[] confettis;
        [SerializeField] private Image feedbackLabel;
        [SerializeField] private TextMeshProUGUI feedbackField;
        [SerializeField] private Sprite wrongSprite;
        [SerializeField] private Sprite correctSprite;
        [SerializeField] private Slider progressBarSlider;
        [SerializeField] private TextMeshProUGUI levelField;
        protected SpanController spanController;

        protected Coroutine progressBar;

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
        public virtual void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                spanController = controller;
                progressBarSlider.gameObject.SetActive(true);
            }

            EnableUIElements();
            PlayEffects();
        }
        
        protected virtual void PlayEffects()
        {
            if (spanController.IsAnswerCorrect())
            {
                PlayConfetti();
                ConfigureProgressBar();
                ConfigureFeedbackField(true);
            }
            else
            {
                progressBar = StartCoroutine(AnimateProgressBar(0, .3f));
                ConfigureFeedbackField(false);
            }
        }

        public void ConfigureProgressBar()
        {
            var roundIndex = spanController.GetRoundIndex();
            progressBarSlider.maxValue = roundIndex;
            levelField.text = $"{roundIndex}";
            float maxValue = progressBarSlider.maxValue;
            float target = maxValue / 4f + progressBarSlider.value;
            progressBar = StartCoroutine(AnimateProgressBar(target, .3f));
        }

        private void PlayConfetti()
        {
            foreach (var confetti in confettis)
            {
                confetti.gameObject.SetActive(true);
                confetti.Play();
            }
        }
        
        protected IEnumerator AnimateProgressBar(float targetValue, float duration)
        {
            levelField.text = $"{spanController.GetRoundIndex()}";
            if (targetValue >= progressBarSlider.maxValue)
            {
                StartCoroutine(LerpSlider(targetValue, duration, () =>
                {
                    levelUpEffect.Play();
                    progressBarSlider.value = 0f;
                    levelField.text = $"{spanController.GetRoundIndex()}";
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
            float startValue = progressBarSlider.value;
    
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                float newValue = Mathf.Lerp(startValue, targetValue, t);
                
                progressBarSlider.value = newValue;
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            progressBarSlider.value = targetValue;
            onComplete?.Invoke();
        }


        private void ConfigureFeedbackField(bool correct)
        {
            feedbackLabel.sprite = correct ? correctSprite : wrongSprite;
            
            feedbackField.text = GetRandomFeedback(correct);

            feedbackLabel.transform.DOScale(1f, 1.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                if (spanController.GetTutorialStatus())
                {
                    feedbackLabel.transform.localScale = new Vector3(0,0,0);
                    TryShowStateTutorial();
                }
                else
                {
                    SwitchNextState();
                }
                /*DOVirtual.DelayedCall(1f, () =>
                {
                    //_spanController.SwitchState();
                });*/
            });
        }

        private string GetRandomFeedback(bool isCorrect)
        {
            if (spanController.GetTutorialStatus())
            {
                AudioManager.Instance.PlayAudioClip(tutorialSuccessFeedback);
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
            if (progressBar != null)
            {
                StopCoroutine(progressBar);
            }
        }

        public void SwitchNextState()
        {
            spanController.SwitchState();
        }

        public void TryShowStateTutorial()
        {
            List<GameObject> objects = new List<GameObject>(spanController.GetTutorialHelperObjects());
            var dictionary = new Dictionary<GameObject, TutorialStep>().CreateFromLists(objects, steps);
            DOVirtual.DelayedCall(1.5f, () =>
            {
                spanController.TriggerStateTutorial(dictionary, true, () =>
                {
                    spanController.TriggerFinalTutorialField("Şimdi sıra sende", yourTurnClip);
                    spanController.SetTutorialCompleted();
                    spanController.SetHelperTutorialCompleted();
                    SwitchNextState();
                });
            });
        }

        public void EnableUIElements()
        {
            progressBarSlider.gameObject.SetActive(true);
            feedbackLabel.gameObject.SetActive(true);
            feedbackField.gameObject.SetActive(true);
            levelUpEffect.gameObject.SetActive(true);
        }

        public void DisableUIElements()
        {
            feedbackLabel.gameObject.SetActive(false);
            levelUpEffect.gameObject.SetActive(false);
            feedbackLabel.transform.localScale = new Vector3(0,0,0);
            DisableConfetti();
        }
        
        private void DisableConfetti()
        {
            foreach (var confetti in confettis)
            {
                confetti.gameObject.SetActive(false);
            }
        }
    }
}

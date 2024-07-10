using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using Scriptables.QuestionSystem.Images;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.ComplexSpan
{
    public class Questioner : MonoBehaviour
    {
        [SerializeField] private GameObject parent;
        [SerializeField] private Image questionBox;

        private Coroutine _displayingQuestions;

        public void PlayCoroutine(List<Question> questions, IComplexSpanStrategy strategy)
        {
            _displayingQuestions = StartCoroutine(ShowQuestionsByType(questions, strategy.HandleOnComplete));
        }
        
        private IEnumerator ShowQuestionsByType(List<Question> questions, Action onComplete)
        {
            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                if (question is ImageQuestion)
                {
                    ConfigureImageField(question);
                }
                else if (question is ClipQuestion)
                {
                    ConfigureClipField(question);
                }
                yield return new WaitForSeconds(1f);
                questionBox.enabled = false;
            }
            
            onComplete?.Invoke();
        }
        
        public void ShowQuestion(Question question, Action onComplete = null)
        {
            parent.SetActive(true);
            if (question is ImageQuestion)
            {
                ConfigureImageField(question);
            }
            else if (question is ClipQuestion)
            {
                ConfigureClipField(question);
            }
            
            DOVirtual.DelayedCall(1f, () =>
            {
                questionBox.enabled = false;
                parent.SetActive(false);
                onComplete?.Invoke();
            });
        }

        private void ConfigureImageField(Question question)
        {
            questionBox.sprite = (Sprite)question.GetQuestionItem();
            questionBox.enabled = true;
        }

        private void ConfigureClipField(Question question)
        {
            AudioClip clip = (AudioClip)question.GetQuestionItem();
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{question.GetCorrectText()}";
            AudioManager.Instance.PlayAudioClip(clip);
        }
    }
}
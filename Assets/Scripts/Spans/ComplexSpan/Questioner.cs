using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using Scriptables.QuestionSystem.Images;
using Scriptables.QuestionSystem.Numbers;
using Spans.Skeleton;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.ComplexSpan
{
    public class Questioner : MonoBehaviour
    {
        [SerializeField] private SpanQuestionState _questionState;
        [SerializeField] private GameObject parent;
        [SerializeField] private Image questionBox;
        private Coroutine _displayingQuestions;

        public void PlayCoroutine(List<Question> questions, IComplexSpanStrategy strategy, SpanQuestionState questionState)
        {
            _questionState = questionState;
            _displayingQuestions = StartCoroutine(ShowQuestionsByType(questions, strategy.HandleOnComplete));
        }
        
        private IEnumerator ShowQuestionsByType(List<Question> questions, Action onComplete)
        {
            parent.gameObject.SetActive(true);
            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                _questionState.ActivateCircle(i, 1f);
                if (question is ImageQuestion)
                {
                    ConfigureImageField(question);
                }
                else if (question is ClipQuestion)
                {
                    ConfigureClipField(question);
                }
                else if (question is NumberQuestion)
                {
                    ConfigureNumberField(question);
                }
                
                yield return new WaitForSeconds(1f);
                questionBox.sprite = null;
                questionBox.enabled = false;
                questionBox.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
            
            onComplete?.Invoke();
        }
        
        public void ShowQuestion(Question question, Action onComplete = null)
        {
            parent.gameObject.SetActive(true);
            if (question is ImageQuestion)
            {
                ConfigureImageField(question);
            }
            else if (question is ClipQuestion)
            {
                ConfigureClipField(question);
            }
            else if (question is NumberQuestion)
            {
                ConfigureNumberField(question);
            }
            
            DOVirtual.DelayedCall(1f, () =>
            {
                questionBox.enabled = false;
                parent.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        private void ConfigureImageField(Question question)
        {
            questionBox.sprite = (Sprite)question.GetQuestionItem();
            questionBox.enabled = true;
        }

        private void ConfigureNumberField(Question question)
        {
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{question.GetQuestionItem()}";
            questionBox.enabled = false;
        }

        private void ConfigureClipField(Question question)
        {
            AudioClip clip = (AudioClip)question.GetQuestionItem();
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{question.GetCorrectText()}";
            AudioManager.Instance.PlayAudioClip(clip);
        }
    }
}
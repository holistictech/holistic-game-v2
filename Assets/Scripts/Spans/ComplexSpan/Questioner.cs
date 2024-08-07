using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using Scriptables.QuestionSystem.Images;
using Scriptables.QuestionSystem.Numbers;
using Spans.Skeleton;
using Spans.Skeleton.QuestionStates;
using TMPro;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.ComplexSpan
{
    public class Questioner : MonoBehaviour
    {
        private SpanQuestionState _questionState;
        private CorsiBlockUIHelper _gridHelper;
        [SerializeField] private GameObject parent;
        [SerializeField] private Image questionBox;
        private Coroutine _displayingQuestions;

        public void InjectQuestionState(ComplexQuestionState questionState)
        {
            _questionState = questionState; 
        }

        public void PlayCoroutine(List<Question> questions, IComplexSpanStrategy strategy, SpanQuestionState questionState)
        {
            _displayingQuestions = StartCoroutine(ShowQuestionsByType(questions, strategy.HandleOnComplete));
        }

        public void PlayComplexShapeCoroutine(List<ComplexShapeQuestion> questions, IComplexSpanStrategy strategy, SpanQuestionState questionState)
        {
            _displayingQuestions = StartCoroutine(ShowComplexQuestion(questions, strategy.HandleOnComplete));
        }

        public void PlayBlockSpanRoutine(List<Question> questions, IComplexSpanStrategy strategy, CorsiBlockUIHelper grid)
        {
            parent.gameObject.SetActive(false);
            grid.gameObject.SetActive(true);
            _gridHelper = grid;
            _displayingQuestions = StartCoroutine(ShowBlockSpanQuestions(questions, strategy.HandleOnComplete));
        }

        private IEnumerator ShowBlockSpanQuestions(List<Question> questions, Action onComplete)
        {
            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                _gridHelper.HighlightTargetBlock(question);
                _questionState.ActivateCircle(i, 1f);
                yield return new WaitForSeconds(1f);
            }
            
            onComplete?.Invoke();
            parent.gameObject.SetActive(false);
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
            parent.gameObject.SetActive(false);
        }

        private IEnumerator ShowComplexQuestion(List<ComplexShapeQuestion> questions, Action onComplete)
        {
            parent.gameObject.SetActive(true);
            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                _questionState.ActivateCircle(i, 1f);
                ConfigureComplexNumberField(question);
                yield return new WaitForSeconds(1f);
                ConfigureComplexShapeField(question);
                yield return new WaitForSeconds(1f);
                ConfigureComplexColorField(question);
                yield return new WaitForSeconds(1f);
            }
            
            parent.gameObject.SetActive(false);
            questionBox.sprite = null;
            questionBox.enabled = false;
            questionBox.color = Color.white;
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = "";
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

        private void ConfigureComplexColorField(ComplexShapeQuestion question)
        {
            questionBox.color = question.QuestionColor;
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = "";
            questionBox.enabled = true;
        }
        
        private void ConfigureComplexShapeField(ComplexShapeQuestion question)
        {
            questionBox.sprite = question.Shape;
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = "";
            questionBox.enabled = true;
        }
        
        private void ConfigureComplexNumberField(ComplexShapeQuestion question)
        {
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{question.Index}";
            questionBox.enabled = false;
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
            questionBox.sprite = question.GetCorrectSprite();
            AudioManager.Instance.PlayAudioClip(clip);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.QuestionSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Spans.Skeleton
{
    public class SpanQuestionState : MonoBehaviour, ISpanState
    {
        [SerializeField] private Image questionBox;
        [SerializeField] private AudioSource audioSource; 
        private SpanController _spanController;
        private List<Question> _spanObjects;

        private int _currentQuestionIndex;
        private List<string> _answers = new List<string>();
        private Coroutine _displayingQuestions;
        public void Enter(SpanController spanController)
        {
            if (_spanController == null) _spanController = spanController;
            _spanObjects = _spanController.GetSpanObjects();
            EnableUIElements();
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            _answers = new List<string>();
            if (_currentQuestionIndex + _spanController.GetRoundIndex() >= _spanObjects.Count)
            {
                _spanObjects = _spanController.GetSpanObjects();
                _currentQuestionIndex = 0;
            }
            
            var question = _spanObjects[_currentQuestionIndex];
            if (question is NumberQuestion)
            {
                _displayingQuestions = StartCoroutine(ShowNumbers());
            } else if (question is ImageQuestion)
            {
                _displayingQuestions = StartCoroutine(ShowImages());
            } else if (question is ClipQuestion)
            {
                _displayingQuestions = StartCoroutine(PlayClips());
            }
        }

        private IEnumerator ShowNumbers()
        {
            for (int i = 0; i < _spanController.GetRoundIndex(); i++)
            {
                var question = _spanObjects[_currentQuestionIndex];
                questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{question.GetQuestionItem()}";
                questionBox.enabled = false;
                _answers.Add(question.CorrectAnswer);
                _currentQuestionIndex++;
                yield return new WaitForSeconds(1f);
            }
            
            SwitchNextState();
        }

        private IEnumerator ShowImages()
        {
            for (int i = 0; i < _spanController.GetRoundIndex(); i++)
            {
                var question = _spanObjects[_currentQuestionIndex];
                questionBox.sprite = (Sprite)question.GetQuestionItem();
                _answers.Add(question.CorrectAnswer);
                _currentQuestionIndex++;
                yield return new WaitForSeconds(1f);
            }
            
            SwitchNextState();
        }

        private IEnumerator PlayClips()
        {
            for (int i = 0; i < _spanController.GetRoundIndex(); i++)
            {
                var question = _spanObjects[_currentQuestionIndex];
                audioSource.Play((ulong)question.GetQuestionItem());
                _answers.Add(question.CorrectAnswer);
                _currentQuestionIndex++;
                yield return new WaitForSeconds(1f);
            }
            
            SwitchNextState();
        }

        public void Exit()
        {
            if (_displayingQuestions != null)
            {
                StopCoroutine(_displayingQuestions);
            }
            
            DisableUIElements();
        }

        public void SwitchNextState()
        {
            _spanController.SetCorrectAnswers(_answers);
            _spanController.SwitchState();
        }

        public void EnableUIElements()
        {
            questionBox.gameObject.SetActive(true);
        }

        public void DisableUIElements()
        {
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = "";
            questionBox.gameObject.SetActive(false);
        }
    }
}

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
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            _answers = new List<string>();
            if (_currentQuestionIndex >= _spanObjects.Count)
            {
                _spanObjects = _spanController.GetSpanObjects();
                _currentQuestionIndex = 0;
            }
            
            var type = _spanObjects[0].GetType();
            if (type == typeof(int))
            {
                _displayingQuestions = StartCoroutine(ShowNumbers());
            } else if (type == typeof(Sprite))
            {
                _displayingQuestions = StartCoroutine(ShowImages());
            } else if (type == typeof(AudioClip))
            {
                _displayingQuestions = StartCoroutine(PlayClips());
            }
            
            SwitchNextState();
        }

        private IEnumerator ShowNumbers()
        {
            for (int i = 0; i < _spanController.GetRoundIndex(); i++)
            {
                var question = _spanObjects[_currentQuestionIndex];
                questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{question.GetQuestionItem()}";
                _answers.Add(question.CorrectAnswer);
                _currentQuestionIndex++;
                yield return new WaitForSeconds(1f);
            }
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
        }

        public void Exit()
        {
            if (_displayingQuestions != null)
            {
                StopCoroutine(_displayingQuestions);
            }
        }

        public void SwitchNextState()
        {
            _spanController.SwitchState();
        }
    }
}

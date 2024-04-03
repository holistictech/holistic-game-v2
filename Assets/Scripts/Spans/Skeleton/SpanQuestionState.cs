using System;
using System.Collections.Generic;
using DG.Tweening;
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
        private List<object> _spanObjects;

        private int _currentQuestionIndex;
        public void Enter(SpanController spanController)
        {
            if (_spanController == null) _spanController = spanController;
            _spanObjects = _spanController.GetSpanObjects();
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            if (_currentQuestionIndex >= _spanObjects.Count)
            {
                _spanObjects = _spanController.GetSpanObjects();
                _currentQuestionIndex = 0;
            }
            
            var type = _spanObjects[0].GetType();
            if (type == typeof(int))
            {
                ShowNumber();
            } else if (type == typeof(Sprite))
            {
                ShowImage();
                
            } else if (type == typeof(AudioClip))
            {
                PlayClip();
            }
            
            _currentQuestionIndex++;
            SwitchNextState();
        }

        private void ShowNumber()
        {
            //@todo: update these function such that 
            //they show questions appropriate with currentRoundIndex
            questionBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{_spanObjects[_currentQuestionIndex]}";
        }

        private void ShowImage()
        {
            questionBox.sprite = (Sprite)_spanObjects[_currentQuestionIndex];
        }

        private void PlayClip()
        {
            audioSource.Play((ulong)_spanObjects[_currentQuestionIndex]);
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void SwitchNextState()
        {
            _spanController.SwitchState();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Samples.Whisper;
using Scriptables.Tutorial;
using TMPro;
using UI.Helpers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Spans.Skeleton
{
    public class SpanAnswerState : MonoBehaviour, ISpanState
    {
        [SerializeField] private List<TutorialStep> _steps;
        [SerializeField] protected TimerHelper timer;
        [SerializeField] protected Button confirmButton;
        [SerializeField] protected Button revertButton;
        private SpanController _spanController;
        
        
        public virtual void Enter(SpanController spanController)
        {
            AddListeners();
        }

        public virtual void Exit()
        {
            RemoveListeners();
            timer.StopTimer();
        }
        
        public virtual void PlayTimer(float maxTime)
        {
            timer.StartTimer(maxTime, SwitchNextState);
        }

        public virtual void SwitchNextState()
        {
        }

        public virtual void TryShowStateTutorial()
        {
        }

        public virtual void EnableUIElements()
        {
            confirmButton.gameObject.SetActive(true);
            revertButton.gameObject.SetActive(true);
        }

        public virtual void DisableUIElements()
        {
            confirmButton.gameObject.SetActive(false);
            revertButton.gameObject.SetActive(false);
        }
        
        public virtual void RevertLastAnswer()
        {
        }

        protected List<TutorialStep> GetTutorialSteps()
        {
            return _steps;
        }

        public virtual void AddListeners()
        {
            confirmButton.onClick.AddListener(SwitchNextState);
            revertButton.onClick.AddListener(RevertLastAnswer);
        }

        public virtual void RemoveListeners()
        {
            confirmButton.onClick.RemoveListener(SwitchNextState);
            revertButton.onClick.RemoveListener(RevertLastAnswer);
        }
    }
}

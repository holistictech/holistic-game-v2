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
        private SpanController _spanController;
        
        
        public virtual void Enter(SpanController spanController)
        {
        }

        public virtual void Exit()
        {
        }
        
        public virtual void PlayTimer(float maxTime)
        {
            timer.StartTimer(maxTime, SwitchNextState);
            /*timerBar.maxValue = maxTime;
            float currentTime = maxTime;

            while (currentTime > 0)
            {
                timerBar.value = Mathf.Lerp(timerBar.value, currentTime, Time.deltaTime * 10);
                currentTime -= Time.deltaTime;
                yield return null;
            }

            timerBar.value = 0f;*/
        }

        public virtual void SwitchNextState()
        {
        }

        public virtual void TryShowStateTutorial()
        {
        }

        public virtual void EnableUIElements()
        {
        }

        public virtual void DisableUIElements()
        {
        }

        protected List<TutorialStep> GetTutorialSteps()
        {
            return _steps;
        }
    }
}

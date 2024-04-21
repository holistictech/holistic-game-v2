using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Samples.Whisper;
using Scriptables.Tutorial;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Spans.Skeleton
{
    public class SpanAnswerState : MonoBehaviour, ISpanState
    {
        [SerializeField] private List<TutorialStep> _steps;
        public Slider timerBar;
        private SpanController _spanController;
        
        
        public virtual void Enter(SpanController spanController)
        {
        }

        public virtual void Exit()
        {
        }
        
        private IEnumerator PlayTimer(float maxTime)
        {
            timerBar.maxValue = maxTime;
            float currentTime = maxTime;

            while (currentTime > 0)
            {
                timerBar.value = Mathf.Lerp(timerBar.value, currentTime, Time.deltaTime * 10);
                currentTime -= Time.deltaTime;
                yield return null;
            }

            timerBar.value = 0f;
        }

        public virtual void SwitchNextState()
        {
        }

        public virtual void TryShowStateTutorial()
        {
        }

        public virtual void EnableUIElements()
        {
            timerBar.gameObject.SetActive(true);
        }

        public virtual void DisableUIElements()
        {
            timerBar.gameObject.SetActive(false);
        }

        protected List<TutorialStep> GetTutorialSteps()
        {
            return _steps;
        }
    }
}

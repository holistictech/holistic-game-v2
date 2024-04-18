using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Samples.Whisper;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Spans.Skeleton
{
    public class SpanAnswerState : MonoBehaviour, ISpanState
    {
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

        public virtual List<GameObject> GetTutorialObjects()
        {
            return new List<GameObject>()
            {
                timerBar.gameObject
            };
        }


        public virtual void EnableUIElements()
        {
            timerBar.gameObject.SetActive(true);
        }

        public virtual void DisableUIElements()
        {
            timerBar.gameObject.SetActive(false);
        }
    }
}

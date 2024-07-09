using System.Collections;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Scriptables.Tutorial;
using Spans.CumulativeSpan;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.Skeleton.AnswerStates
{
    public class ComplexAnswerState : MultipleChoiceAnswerState
    {
        private ComplexSpan.ComplexSpan _complexSpan;
        
        private Coroutine _timer;
        private Coroutine _tutorialHighlight;
        private float _maxTime;
        
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                _complexSpan = controller.GetComponent<ComplexSpan.ComplexSpan>();
                spanController = controller;
                //base.Enter(controller);
            }

            _maxTime = spanController.GetRoundTime();
            AddListeners();
            EnableUIElements();
            SetChoiceUI();
            
            if (spanController.GetTutorialStatus())
            {
                timer.EnableSelf();
                TryShowStateTutorial();
            }
            else
            {
                if (_complexSpan.GetIsMainSpanNeeded())
                {
                    hintHelper.SetFieldText("Hangi hayvan seslerini duymuştun?");
                    hintHelper.AnimateBanner(() =>
                    {
                        PlayTimer(_maxTime);
                    });
                }
                else
                {
                    PlayTimer(_maxTime);
                }
            }
        }
        
        protected override void SetChoiceUI()
        {
            if (_complexSpan.GetIsMainSpanNeeded())
            {
                Debug.Log("Needed");
            }
            base.SetChoiceUI();
        }
    }
}

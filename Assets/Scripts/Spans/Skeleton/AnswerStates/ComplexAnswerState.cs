using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using Scriptables.Tutorial;
using Spans.CumulativeSpan;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Helpers;

namespace Spans.Skeleton.AnswerStates
{
    public class ComplexAnswerState : MultipleChoiceAnswerState
    {
        [SerializeField] private List<Button> modeButtons;
        private ComplexSpan.ComplexSpan _complexSpan;
        private IComplexSpanStrategy _currentStrategy;
        
        private Coroutine _timer;
        private Coroutine _tutorialHighlight;
        
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                _complexSpan = controller.GetComponent<ComplexSpan.ComplexSpan>();
                spanController = controller;
                //base.Enter(controller);
            }

            _currentStrategy = _complexSpan.GetCurrentStrategy();

            maxTime = spanController.GetRoundTime();
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
                /*if (_complexSpan.GetIsMainSpanNeeded())
                {
                    hintHelper.SetFieldText("Sırasıyla hangi hayvan seslerini duymuştun?");
                    hintHelper.AnimateBanner(() =>
                    {
                        PlayTimer(maxTime);
                    });
                }
                else
                {
                    PlayTimer(maxTime);
                }*/
                
                PlayTimer(maxTime);
            }
        }
        
        protected override void SetChoiceUI()
        {
            base.SetChoiceUI();
            if (_complexSpan.GetIsMainSpanNeeded())
            {
                Debug.Log("Needed");
            }
        }

        private void OnButtonClick(int index)
        {
            var chosenType = (CommonFields.ButtonType)index;
            _currentStrategy.AppendChoice(chosenType);
        }

        public List<Button> GetButtons()
        {
            return modeButtons;
        }
        
        public override void EnableUIElements()
        {
            base.EnableUIElements();
            gridLayoutGroup.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(true);
            revertButton.gameObject.SetActive(true);
            timer.EnableSelf();
        }
    }
}

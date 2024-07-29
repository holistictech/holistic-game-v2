using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using Scriptables.Tutorial;
using Spans.ComplexSpan;
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
        [SerializeField] private Questioner questioner; 
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
                AddListeners();
            }

            _currentStrategy = _complexSpan.GetCurrentStrategy();
            maxTime = spanController.GetRoundTime();
            EnableUIElements();
            
            _currentStrategy.InjectAnswerState(this);
            _currentStrategy.ShowAnswerStateQuestion(questioner, () =>
            {
               PlayTimer(maxTime); 
            });
        }

        public void TriggerHintHelper(string hint, Action onComplete)
        {
            hintHelper.SetFieldText(hint);
            hintHelper.AnimateBanner(() =>
            {
                onComplete?.Invoke();
            });
        }
        
        public void OnButtonClick(int index)
        {
            modeButtons.ForEach(x => x.gameObject.SetActive(false));
            var chosenType = (CommonFields.ButtonType)index;
            _currentStrategy.AppendChoice(chosenType);
            SwitchNextState();
        }

        public List<Button> GetButtons()
        {
            return modeButtons;
        }
        
        public override void EnableUIElements()
        {
            timer.EnableSelf();
        }

        public void EnableGridField()
        {
            gridLayoutGroup.gameObject.SetActive(true);
        }

        public void EnableButtons()
        {
            confirmButton.gameObject.SetActive(true);
            revertButton.gameObject.SetActive(true);
        }
    }
}

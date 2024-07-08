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
        [SerializeField] private List<TutorialStep> gridStep;
        [SerializeField] private GridUIHelper gridHelper;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        
        private Coroutine _timer;
        private Coroutine _tutorialHighlight;
        private float _maxTime;
        
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                spanController = controller;
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
                PlayTimer(_maxTime);
            }
        }

        private void SetChoiceUI()
        {
            gridHelper.SetConstraintCount(spanController.GetRoundIndex(), spanController.GetCumulativeStatus());
            var choices = spanController.GetChoices();
            var unitCircles = spanController.GetActiveCircles();
            gridHelper.SetActiveCircles(unitCircles);
            gridHelper.SetStartingIndex(spanController.GetStartingUnitIndex());
            gridHelper.ConfigureChoices(choices, this);
        }

        protected override void PlayTimer(float duration)
        {
            timer.StartTimer(duration, SwitchNextState);
        }

        public override void SwitchNextState()
        {
            if (spanController.GetTutorialStatus())
            {
                spanController.ClearTutorialHighlights();
            }
            spanController.SetSelectedAnswers(gridHelper.GetGivenAnswers());
            spanController.SwitchState();
        }

        public override void Exit()
        {
            DisableUIElements();
            RemoveListeners();
            if (_tutorialHighlight != null)
            {
                StopCoroutine(_tutorialHighlight);
            }
        }
        
        public override void EnableUIElements()
        {
            base.EnableUIElements();
            gridLayoutGroup.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(true);
            revertButton.gameObject.SetActive(true);
            timer.EnableSelf();
        }

        public override void DisableUIElements()
        {
            base.DisableUIElements();
            gridLayoutGroup.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);
            revertButton.gameObject.SetActive(spanController.GetTutorialStatus());
            if (spanController.GetTutorialStatus())
            {
                timer.EnableSelf();
                spanController.SetTutorialHelperObjects(new List<GameObject>
                {
                    timer.gameObject,
                    revertButton.gameObject,
                });
            }
            else
            {
                timer.StopTimer();
            }
            DisableSpawnedChoices();
        }

        private void DisableSpawnedChoices()
        {
            gridHelper.DisableSpawnedChoices();
        }

        public override void RevertLastAnswer()
        {
            gridHelper.RevokeLastSelection();
        }
    }
}

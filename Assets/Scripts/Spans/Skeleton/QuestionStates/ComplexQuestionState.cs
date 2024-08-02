using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using Scriptables.Tutorial;
using Spans.ComplexSpan;
using UI.Helpers;
using UnityEngine;
using Utilities.Helpers;

namespace Spans.Skeleton.QuestionStates
{
    public class ComplexQuestionState : SpanQuestionState
    {
        [SerializeField] private List<TutorialStep> steps;
        [SerializeField] private Questioner questioner;
        [SerializeField] private CorsiBlockUIHelper blockUIHelper;

        private List<Question> _spanObjects;

        private ComplexSpan.ComplexSpan _complexSpan;
        private IComplexSpanStrategy _currentStrategy;

        public override void Enter(SpanController controller)
        {
            Debug.Log("Got in complex question state");
            if (spanController == null)
            {
                _complexSpan = controller.GetComponent<ComplexSpan.ComplexSpan>();
                _spanObjects = controller.GetSpanObjects();
                base.Enter(controller);
            }

            _currentStrategy = _complexSpan.GetCurrentStrategy();
            _currentStrategy.InjectQuestionState(this);
            EnableUIElements();
            ResetPreviousCircles();
            SetCircleUI(_currentStrategy.GetCircleCount());
            if (spanController.GetTutorialStatus())
            {
                TryShowStateTutorial();
            }
            else
            {
                ShowQuestion();
            }
            

            StatisticsHelper.IncrementDisplayedQuestionCount();
        }

        private bool _hasMainPlayed;
        public override void ShowQuestion()
        {
            if (_currentStrategy is PerceptionRecognitionStrategy)
            {
                _spanObjects = spanController.GetSpanObjects();
                _currentStrategy.ShowQuestionStateQuestion(questioner);
            }else
            {
                _currentStrategy.ShowQuestionStateQuestion(questioner);
            }
        }

        public override void Exit()
        {
            if (displayingQuestions != null)
            {
                StopCoroutine(displayingQuestions);
            }

            ResetPreviousCircles();
        }

        public CorsiBlockUIHelper GetGridHelper()
        {
            return blockUIHelper;
        }

        public override void SwitchNextState()
        {
            DisableUIElements();
            spanController.SwitchState();
        }

        public override void EnableUIElements()
        {
            unitParent.gameObject.SetActive(true);
        }

        public override void DisableUIElements()
        {
            
        }
    }
}
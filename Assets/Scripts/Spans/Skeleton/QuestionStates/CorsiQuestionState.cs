using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.QuestionSystem;
using UI;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.Skeleton.QuestionStates
{
    public class CorsiQuestionState : SpanQuestionState
    {
        [SerializeField] private CorsiBlockUIHelper blockUIHelper;
        private List<Question> _spanObjects = new List<Question>();
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                spanController = controller;
                base.Enter(spanController);
                blockUIHelper.SpawnCorsiBlocks();
                blockUIHelper.InjectQuestionState(this);
                spanController.SetHelperObject(blockUIHelper.gameObject);
            }
            SetCircleUI(spanController.GetRoundIndex());
            EnableUIElements();
            _spanObjects = spanController.GetSpanObjects();
            ShowQuestion();
            StatisticsHelper.IncrementDisplayedQuestionCount();
        }
        
        public override void ShowQuestion()
        {
            DistributeQuestions();
        }

        private void DistributeQuestions()
        {
            blockUIHelper.AssignQuestions(_spanObjects);
            displayingQuestions = StartCoroutine(IterateQuestions());
        }

        private IEnumerator IterateQuestions()
        {
            var spanQuestions = spanController.GetCurrentSpanQuestions();
            for (int i = 0; i < spanQuestions.Count; i++)
            {
                ActivateCircle(i);
                blockUIHelper.HighlightTargetBlock(spanQuestions[i]);
                yield return new WaitForSeconds(2f);
            }
            
            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        public override void SwitchNextState()
        {
            if (spanController.GetBackwardStatus())
            {
                RotateCircles(() =>
                {
                    spanController.SwitchState();
                });
            }
            else
            {
                spanController.SwitchState();
            }
        }

        public override void TryShowStateTutorial()
        {
        }

        public override void EnableUIElements()
        {
            blockUIHelper.gameObject.SetActive(true);
            unitParent.gameObject.SetActive(true);
        }

        public override void DisableUIElements()
        {
        }
    }
}

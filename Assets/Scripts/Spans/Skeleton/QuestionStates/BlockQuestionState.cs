using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.QuestionSystem;
using UI.Helpers;
using UnityEngine;

namespace Spans.Skeleton.QuestionStates
{
    public class BlockQuestionState : SpanQuestionState
    {
        [SerializeField] private BlockSpanUIHelper blockUIHelper;
        private List<Question> _spanObjects = new List<Question>();
        
        public override void Enter(SpanController controller)
        {
            base.Enter(controller);
            if (spanController == null)
            {
                spanController = controller;
            }

            _spanObjects = spanController.GetSpanObjects();
        }

        public override void ShowQuestion()
        {
            DistributeQuestions();
            displayingQuestions = StartCoroutine(IterateQuestions());
        }

        private void DistributeQuestions()
        {
            blockUIHelper.AssignQuestions(_spanObjects);
        }

        private IEnumerator IterateQuestions()
        {
            var spanQuestions = spanController.GetCurrentSpanQuestions();
            for (int i = 0; i < spanQuestions.Count; i++)
            {
                if (currentQuestionIndex >= spanQuestions.Count)
                {
                    break;
                }
                ActivateCircle(currentQuestionIndex);
                blockUIHelper.HighlightTargetBlock(spanQuestions[currentQuestionIndex]);
                currentQuestionIndex++;
                yield return new WaitForSeconds(2f);
            }
            
            DOVirtual.DelayedCall(1f, SwitchNextState);
        }



        public override void EnableUIElements()
        {
            base.EnableUIElements();
        }

        public override void DisableUIElements()
        {
            base.DisableUIElements();
        }

        private void AddListeners()
        {
            
        }

        private void RemoveListeners()
        {
            
        }
    }
}

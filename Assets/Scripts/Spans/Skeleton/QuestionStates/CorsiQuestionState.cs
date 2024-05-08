using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.QuestionSystem;
using UI;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton.QuestionStates
{
    public class CorsiQuestionState : SpanQuestionState
    {
        [SerializeField] private CorsiBlockUIHelper blockUIHelper;

        private SpanController _spanController;
        private List<Question> _spanObjects = new List<Question>();
        public override void Enter(SpanController spanController)
        {
            if (_spanController == null)
            {
                _spanController = spanController;
                base.Enter(_spanController);
                blockUIHelper.SpawnCorsiBlocks();
                blockUIHelper.InjectQuestionState(this);
                _spanController.SetHelperObject(blockUIHelper.gameObject);
            }
            SetCircleUI(_spanController.GetRoundIndex());
            EnableUIElements();
            _spanObjects = _spanController.GetSpanObjects();
            ShowQuestion();
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
            var spanQuestions = _spanController.GetCurrentSpanQuestions();
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
            _spanController.SwitchState();
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

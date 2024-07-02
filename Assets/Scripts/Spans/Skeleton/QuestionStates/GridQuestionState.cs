using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.QuestionSystem;
using UI.Helpers;
using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Spans.Skeleton.QuestionStates
{
    public class GridQuestionState : SpanQuestionState
    {
        [SerializeField] private CorsiBlockUIHelper blockUIHelper;
        private List<Question> _spanObjects = new List<Question>();
        private List<Question> _currentQuestions = new List<Question>();
        
        public override void Enter(SpanController controller)
        {
            blockUIHelper.GetCorsiBlocks();
            blockUIHelper.ResetCorsiBlocks();
            EnableUIElements();
            if (spanController == null)
            {
                base.Enter(controller);
                spanController.SetHelperObject(blockUIHelper.gameObject);
                currentQuestionIndex = 0;
            }
            
            _spanObjects = spanController.GetSpanObjects();
            blockUIHelper.ConfigureInput(false);
            StatisticsHelper.IncrementDisplayedQuestionCount();
            SetCircleUI(spanController.GetRoundIndex());
            ShowQuestion();
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
            _currentQuestions = new List<Question>();
            for (int i = 0; i < spanQuestions.Count; i++)
            {
                ConfigureQuestionField(currentQuestionIndex, spanQuestions[i]);
                yield return new WaitForSeconds(1f);
            }
            
            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        private void ConfigureQuestionField(int index, Question question)
        {
            AudioManager.Instance.PlayAudioClip((AudioClip)question.GetQuestionItem());
            ActivateCircle(index, 1f);
            blockUIHelper.HighlightTargetBlock(question);
            currentQuestionIndex++;
            _currentQuestions.Add(question);
        }
        
        public override void SwitchNextState()
        {
            if (displayingQuestions != null)
            {
                StopCoroutine(displayingQuestions);
            }
            DisableUIElements();
            ConfigureDisplayedQuestions();
            
            if (spanController.GetBackwardStatus())
            {
                RotateCircles(-180, () =>
                {
                    base.SwitchNextState();
                });
            }
            else
            {
                base.SwitchNextState();
            }
        }
        
        private void ConfigureDisplayedQuestions()
        {
            spanController.SetCurrentDisplayedQuestions(_currentQuestions);
        }
        
        public override void EnableUIElements()
        {
            base.EnableUIElements();
            blockUIHelper.gameObject.SetActive(true);
        }
    }
}

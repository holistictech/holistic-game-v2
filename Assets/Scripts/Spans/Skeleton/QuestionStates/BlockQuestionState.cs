using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.QuestionSystem;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton.QuestionStates
{
    public class BlockQuestionState : SpanQuestionState
    {
        [SerializeField] private BlockSpanUIHelper blockUIHelper;
        private List<Question> _spanObjects = new List<Question>();
        private List<Question> _currentQuestions = new List<Question>();

        private int _indexHelper = 2;

        public override void Enter(SpanController controller)
        {
            base.Enter(controller);
            EnableUIElements();
            if (spanController == null)
            {
                spanController = controller;
            }

            _spanObjects = spanController.GetSpanObjects();
            spanEventBus.Register<BlockSpanGridSizeEvent>(UpdateHelperIndex);
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
            blockUIHelper.SetConstraintsCount(_indexHelper);
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
                _currentQuestions.Add(spanQuestions[i]);
                yield return new WaitForSeconds(2f);
            }
            
            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        public override void SwitchNextState()
        {
            if (displayingQuestions != null)
            {
                StopCoroutine(displayingQuestions);
            }
            DisableUIElements();
            ConfigureDisplayedQuestions();
            base.SwitchNextState();
        }
        
        private void ConfigureDisplayedQuestions()
        {
            spanController.SetCurrentDisplayedQuestions(_currentQuestions);
        }

        private void UpdateHelperIndex(BlockSpanGridSizeEvent update)
        {
            _indexHelper = update.NewGrid.y;
        }
        
        public override void EnableUIElements()
        {
            base.EnableUIElements();
            blockUIHelper.EnableGrid();
        }

        public override void DisableUIElements()
        {
            base.DisableUIElements();
            blockUIHelper.DisableGrid();
        }

        private void AddListeners()
        {
            
        }

        private void RemoveListeners()
        {
            
        }
        
        public override void OnDestroy()
        {
            spanEventBus.Unregister<BlockSpanGridSizeEvent>(UpdateHelperIndex);
        }
    }
}

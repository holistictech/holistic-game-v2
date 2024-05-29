using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables.QuestionSystem;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.Skeleton.QuestionStates
{
    public class BlockQuestionState : SpanQuestionState
    {
        [SerializeField] private BlockSpanUIHelper blockUIHelper;
        private List<Question> _spanObjects = new List<Question>();
        private List<Question> _currentQuestions = new List<Question>();

        private int _indexHelper = 2;
        private int _circleCount = 1;
        
        public override void Enter(SpanController controller)
        {
            blockUIHelper.DisableCorsiBlocks();
            EnableUIElements();
            if (spanController == null)
            {
                base.Enter(controller);
                spanController.SetHelperObject(blockUIHelper.gameObject);
                spanEventBus.Register<BlockSpanGridSizeEvent>(UpdateHelperIndex);
            }
            _spanObjects = spanController.GetSpanObjects();
            blockUIHelper.ConfigureInput(false);
            StatisticsHelper.IncrementDisplayedQuestionCount();
            SetCircleUI(_circleCount);
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
                ActivateCircle(i);
                blockUIHelper.HighlightTargetBlock(spanQuestions[i]);
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
            _circleCount = update.CircleCount;
        }
        
        public override void EnableUIElements()
        {
            base.EnableUIElements();
            blockUIHelper.EnableGrid();
            blockUIHelper.gameObject.SetActive(true);
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
        
        public override void OnDestroy()
        {
            spanEventBus.Unregister<BlockSpanGridSizeEvent>(UpdateHelperIndex);
        }
    }
}

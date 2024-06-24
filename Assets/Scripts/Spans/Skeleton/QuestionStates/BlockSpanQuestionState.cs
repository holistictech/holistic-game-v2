using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.BlockSpan;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Utilities;

namespace Spans.Skeleton.QuestionStates
{
    public class BlockSpanQuestionState : SpanQuestionState
    {
        [SerializeField] private BlockSpanUIHelper blockUIHelper;
        private List<Question> _spanObjects = new List<Question>();
        private List<Question> _currentQuestions = new List<Question>();

        private IBlockSpanStrategy _config = new RegularMode();
        private BlockSpan.BlockSpan _blockSpanController; 
        private int _indexHelper = 2;
        private int _circleCount = 1;
        
        public override void Enter(SpanController controller)
        {
            blockUIHelper.DisableCorsiBlocks();
            EnableUIElements();
            if (spanController == null)
            {
                _blockSpanController = controller.GetComponent<BlockSpan.BlockSpan>();
                base.Enter(controller);
                spanController.SetHelperObject(blockUIHelper.gameObject);
                spanEventBus.Register<BlockSpanGridSizeEvent>(UpdateHelperIndex);
            }

            _config = _blockSpanController.GetCurrentStrategy();
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
            blockUIHelper.SetStrategy(_config);
            blockUIHelper.AssignQuestions(_spanObjects);
            blockUIHelper.SetConstraintsCount(_indexHelper);
        }

        private IEnumerator IterateQuestions()
        {
            var spanQuestions = spanController.GetCurrentSpanQuestions();
            _currentQuestions = new List<Question>();
            for (int i = 0; i < spanQuestions.Count; i++)
            {
                ConfigureQuestionField(i, spanQuestions[i]);
                if (_blockSpanController.GetCombineStatus() && spanQuestions.Count > 1)
                {
                    yield return FadeBlocksOutAndIn();
                }
                else
                {
                    yield return new WaitForSeconds(0.01f);
                }
            }
            
            DOVirtual.DelayedCall(1f, SwitchNextState);
        }

        private void ConfigureQuestionField(int index, Question question)
        {
            if (_config is ColorChooserMode)
            {
                ActivateCircle(index, 1f, (Color)question.GetQuestionItem());
            }
            else
            {
                ActivateCircle(index, 1f);
            }
                
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
                blockUIHelper.RotateGrid(-90, () =>
                {
                    base.SwitchNextState();
                });
            }
            else
            {
                base.SwitchNextState();
            }
        }

        private IEnumerator FadeBlocksOutAndIn()
        {
            yield return new WaitForSeconds(1f);
            
            blockUIHelper.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f); // Wait for the block to be hidden

            blockUIHelper.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f); // Wait for the block to be shown and highlighted
        }
        
        private void ConfigureDisplayedQuestions()
        {
            spanController.SetCurrentDisplayedQuestions(_currentQuestions);
        }

        private void UpdateHelperIndex(BlockSpanGridSizeEvent update)
        {
            _config = update.StrategyClass;
            _indexHelper = update.NewConfig.GridSize.y;
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

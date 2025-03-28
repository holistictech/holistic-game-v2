using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using UI.CorsiBlockTypes;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Helpers;

namespace UI.Helpers
{
    public class CorsiBlockUIHelper : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup blockParent;
        protected List<CorsiBlock> spawnedBlocks = new List<CorsiBlock>();
        protected List<CorsiBlock> selectedBlocks = new List<CorsiBlock>();
        protected List<Question> selectedAnswers = new List<Question>();
        private List<UnitCircle> _activeCircles = new List<UnitCircle>();
        private int _answerIndex;

        public void SetActiveCircles(List<UnitCircle> circles)
        {
            _answerIndex = 0;
            _activeCircles = circles;
            _activeCircles[_answerIndex].AnimateCircle();
        }

        public void GetCorsiBlocks()
        {
            spawnedBlocks = new List<CorsiBlock>(blockParent.GetComponentsInChildren<CorsiBlock>(true));
        }

        public virtual void AssignQuestions(List<Question> spanQuestions)
        {
            selectedAnswers.Clear();
            selectedBlocks.Clear();
            blockParent.gameObject.SetActive(true);
            for (int i = 0; i < spanQuestions.Count; i++)
            {
                spawnedBlocks[i].ConfigureSelf(spanQuestions[i], this);
                spawnedBlocks[i].SetParentTransform(blockParent.GetComponent<RectTransform>());
                spawnedBlocks[i].MakeBlockMove();
            }
        }
        
        public void ConfigureInput(bool toggle)
        {
            foreach (var block in spawnedBlocks)
            {
                block.ConfigureInput(toggle);
            }
        }

        public List<Question> GetGivenAnswers()
        {
            return selectedAnswers;
        }

        public virtual void HighlightTargetBlock(Question target)
        {
            var block = GetBlockByQuestion(target);
            block.AnimateSelf();
        }

        public CorsiBlock GetBlockByQuestion(Question question)
        {
            foreach (var block in spawnedBlocks)
            {
                var assignedQuestion = block.GetAssignedQuestion();  

                if (question is ColorQuestion || question is BlockImageQuestion)
                {
                    if (assignedQuestion.GetType() == question.GetType() && 
                        assignedQuestion.GetQuestionItemByType(CommonFields.ButtonType.Color).Equals(question.GetQuestionItemByType(CommonFields.ButtonType.Color)))
                    {
                        return block;
                    }
                }
                else
                {
                    if (assignedQuestion.GetType() == question.GetType() && 
                        assignedQuestion.GetQuestionItem().Equals(question.GetQuestionItem()))
                    {
                        return block;
                    }
                }
            }
            throw new Exception("Could not find target question in blocks");
        }


        public void AppendSelectedAnswers(Question question, CorsiBlock selectedBlock)
        {
            if (selectedAnswers.Count < _activeCircles.Count)
            {
                selectedBlocks.Add(selectedBlock);
                selectedAnswers.Add(question);
                UpdateAndAnimateUnitCircle(true);
            }
            else
            {
                selectedBlock.ResetUI();
            }
        }

        public void RevokeLastSelection()
        {
            if (selectedAnswers.Count == 0) return;
            selectedAnswers.Remove(selectedAnswers[^1]);
            selectedBlocks[^1].ResetUI();
            selectedBlocks.Remove(selectedBlocks[^1]);
            UpdateAndAnimateUnitCircle(false);
        }
        
        private void UpdateAndAnimateUnitCircle(bool toggle)
        {
            if (toggle)
            {
                _activeCircles[_answerIndex].OnAnswerGiven();
                _answerIndex++;
                if (_answerIndex >= _activeCircles.Count) return;
                _activeCircles[_answerIndex].AnimateCircle();
            }
            else
            {
                if(_answerIndex < _activeCircles.Count)
                    _activeCircles[_answerIndex].OnAnswerRevoked();
                _answerIndex--;
                _activeCircles[_answerIndex].OnAnswerRevoked();
                _activeCircles[_answerIndex].AnimateCircle();
            }
        }

        public void ResetCorsiBlocks()
        {
            foreach (var block in spawnedBlocks)
            {
                block.ResetUI();
            }
        }

        public void DisableUnitCircles()
        {
            foreach (var circle in _activeCircles)
            {
                circle.DisableCircle();
            }

            _answerIndex = 0;
        }
    }
}

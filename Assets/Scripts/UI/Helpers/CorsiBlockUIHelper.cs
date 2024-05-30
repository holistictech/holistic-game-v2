using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using UI.CorsiBlockTypes;
using UnityEngine;
using UnityEngine.UI;

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
            _activeCircles = circles;
            _activeCircles[_answerIndex].AnimateCircle();
        }

        public void GetCorsiBlocks()
        {
            spawnedBlocks = new List<CorsiBlock>(blockParent.GetComponentsInChildren<CorsiBlock>());
        }

        public virtual void AssignQuestions(List<Question> spanQuestions)
        {
            selectedAnswers.Clear();
            selectedBlocks.Clear();
            blockParent.gameObject.SetActive(true);
            for (int i = 0; i < spanQuestions.Count; i++)
            {
                spawnedBlocks[i].ConfigureSelf(spanQuestions[i], this);
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

        public void HighlightTargetBlock(Question target)
        {
            var block = GetBlockByQuestion(target);
            block.AnimateSelf();
        }

        public CorsiBlock GetBlockByQuestion(Question question)
        {
            foreach (var block in spawnedBlocks)
            {
                if (block.GetAssignedQuestion() == question)
                    return block;
            }

            throw new Exception("Could not find target question in blocks");
        }

        public void AppendSelectedAnswers(Question question, CorsiBlock selectedBlock)
        {
            selectedBlocks.Add(selectedBlock);
            selectedAnswers.Add(question);
            UpdateAndAnimateUnitCircle(true);
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

using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Helpers
{
    public class CorsiBlockUIHelper : MonoBehaviour
    {
        [SerializeField] private LayoutGroup blockParent;
        [SerializeField] private CorsiBlock blockPrefab;

        private CorsiQuestionState _corsiQuestionState;
        private CorsiAnswerState _corsiAnswerState;
        private List<CorsiBlock> _spawnedBlocks = new List<CorsiBlock>();
        private List<CorsiBlock> _selectedBlocks = new List<CorsiBlock>();
        private List<Question> _selectedAnswers = new List<Question>();
        private List<UnitCircle> _activeCircles = new List<UnitCircle>();
        private const int _blockSpawnCount = 9;
        private int _answerIndex;

        public void InjectQuestionState(CorsiQuestionState questionState)
        {
            _corsiQuestionState = questionState;
        }

        public void InjectAnswerState(CorsiAnswerState answerState)
        {
            _corsiAnswerState = answerState;
        }

        public void SetActiveCircles(List<UnitCircle> circles)
        {
            _activeCircles = circles;
            _activeCircles[_answerIndex].AnimateCircle();
        }

        public void SpawnCorsiBlocks()
        {
            for (int i = 0; i < _blockSpawnCount; i++)
            {
                var tempBlock = Instantiate(blockPrefab, blockParent.transform);
                _spawnedBlocks.Add(tempBlock);
            }
        }

        public void GetCorsiBlocks()
        {
            _spawnedBlocks = new List<CorsiBlock>(blockParent.GetComponentsInChildren<CorsiBlock>());
        }

        public void AssignQuestions(List<Question> spanQuestions)
        {
            _selectedAnswers.Clear();
            _selectedBlocks.Clear();
            blockParent.gameObject.SetActive(true);
            for (int i = 0; i < spanQuestions.Count; i++)
            {
                _spawnedBlocks[i].ConfigureSelf(spanQuestions[i], this);
            }
        }

        public void EnableInput()
        {
            foreach (var block in _spawnedBlocks)
            {
                block.AcceptInput();
            }
        }

        public List<Question> GetGivenAnswers()
        {
            return _selectedAnswers;
        }

        public void HighlightTargetBlock(Question target)
        {
            var block = GetBlockByQuestion(target);
            block.AnimateSelf();
        }

        public CorsiBlock GetBlockByQuestion(Question question)
        {
            foreach (var block in _spawnedBlocks)
            {
                if (block.GetAssignedQuestion() == question)
                    return block;
            }

            throw new Exception("Could not find target question in corsi blocks");
        }

        public void AppendSelectedAnswers(Question question, CorsiBlock selectedBlock)
        {
            _selectedBlocks.Add(selectedBlock);
            _selectedAnswers.Add(question);
            UpdateAndAnimateUnitCircle(true);
        }

        public void RevokeLastSelection()
        {
            if (_selectedAnswers.Count == 0) return;
            _selectedAnswers.Remove(_selectedAnswers[^1]);
            _selectedBlocks[^1].ResetUI();
            _selectedBlocks.Remove(_selectedBlocks[^1]);
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
            foreach (var block in _spawnedBlocks)
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

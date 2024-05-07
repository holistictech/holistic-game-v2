using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
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
        private List<CorsiBlock> _spawnedBlocks = new List<CorsiBlock>();
        private List<CorsiBlock> _selectedBlocks = new List<CorsiBlock>();
        private List<Question> _selectedAnswers = new List<Question>();
        private const int _blockSpawnCount = 9;

        private void Start()
        {
            SpawnCorsiBlocks();
        }

        public void InjectQuestionState(CorsiQuestionState questionState)
        {
            _corsiQuestionState = questionState;
        }

        private void SpawnCorsiBlocks()
        {
            for (int i = 0; i < _blockSpawnCount; i++)
            {
                var tempBlock = Instantiate(blockPrefab, blockParent.transform);
                tempBlock.DisableSelf();
                _spawnedBlocks.Add(tempBlock);
            }
        }

        public void AssignQuestions(List<Question> spanQuestions)
        {
            for (int i = 0; i < spanQuestions.Count; i++)
            {
                _spawnedBlocks[i].ConfigureSelf(spanQuestions[i], this);
            }
        }

        public void HighlightSelectedBlock(int blockIndex)
        {
            var temp = _selectedBlocks[blockIndex];
            if (temp == null)
            {
                throw new Exception("block to be highlighted is null");
            }
            
            temp.AnimateSelf();
        }

        public void AppendSelectedAnswers(Question question, CorsiBlock selectedBlock)
        {
            _selectedBlocks.Add(selectedBlock);
            _selectedAnswers.Add(question);
        }

        public void RevokeLastSelection()
        {
            if (_selectedAnswers.Count == 0) return;
            _selectedAnswers.Remove(_selectedAnswers[^1]);
            _selectedBlocks[^1].ResetUI();
            _selectedBlocks.Remove(_selectedBlocks[^1]);
        }
    }
}

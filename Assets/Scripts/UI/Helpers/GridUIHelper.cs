using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Helpers
{
    public class GridUIHelper : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup gridParent;
        [SerializeField] private Choice choice;

        private List<Choice> _choicePool;
        private List<Question> _givenAnswers = new List<Question>();
        private List<Choice> _activeChoices = new List<Choice>();
        private List<Choice> _selectedChoices = new List<Choice>();
        
        public static event Action<int> OnChoiceSelected;

        private void Start()
        {
            SpawnChoicePool();
        }

        public void ConfigureChoices(List<Question> questions, MultipleChoiceAnswerState answerState)
        {
            foreach (var question in questions)
            {
                var available = GetAvailableChoice();
                available.ConfigureUI(question, answerState);
                _activeChoices.Add(available);
            }
        }

        private void SpawnChoicePool()
        {
            _choicePool = new List<Choice>();
            for (int i = 0; i < 9; i++)
            {
                var temp = Instantiate(choice, gridParent.transform);
                _choicePool.Add(temp);
            }
        }
        
        private void CalculateDynamicCellSize()
        {
            RectTransform gridRectTransform = gridParent.GetComponent<RectTransform>();

            var rect = gridRectTransform.rect;
            float width = rect.width;
            float height = rect.height;
    
            float availableWidth = width - (gridParent.padding.left + gridParent.padding.right) - ((gridParent.constraintCount - 1) * gridParent.spacing.x);
            float availableHeight = height - (gridParent.padding.top + gridParent.padding.bottom) - ((gridParent.constraintCount - 1) * gridParent.spacing.y);
    
            float cellWidth = availableWidth / gridParent.constraintCount;
            float cellHeight = availableHeight / gridParent.constraintCount;
            float cellSize = Mathf.Min(cellWidth, cellHeight);
            gridParent.cellSize = new Vector2(cellSize, cellSize);
        }

        public void SelectChoice(Question question, Choice selected)
        {
            _givenAnswers.Add(question);
            _selectedChoices.Add(selected);
        }

        public void RevokeLastSelection()
        {
            if (_givenAnswers.Count == 0) return;
            //OnChoiceSelected?.Invoke(-(_givenAnswers.Count));
            _givenAnswers.Remove(_givenAnswers[^1]);
            _selectedChoices[^1].ResetUI();
            _selectedChoices.Remove(_selectedChoices[^1]);
        }
        
        public void SetConstraintCount(int index, bool isCumulative)
        {
            if (isCumulative)
            {
                gridParent.constraintCount = 3;
            }
            else if (index == 2)
            {
                gridParent.constraintCount = 2;
            }else
            {
                gridParent.constraintCount = 3;
            }
        }
        
        private void DisableSpawnedChoices()
        {
            foreach (var spawnedChoice in _choicePool)
            {
                spawnedChoice.DisableSelf();
            }
            _activeChoices.Clear();
        }
        
        private Choice GetAvailableChoice()
        {
            for (int i = 0; i < _choicePool.Count; i++)
            {
                if (!_choicePool[i].isActiveAndEnabled)
                {
                    return _choicePool[i];
                }
            }

            throw new Exception("No available choice. Need to spawn");
            return null;
        }
    }
}

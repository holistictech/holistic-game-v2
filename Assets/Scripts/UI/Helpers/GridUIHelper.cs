using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Helpers
{
    public class GridUIHelper : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup gridParent;
        [SerializeField] private Choice choicePrefab;

        private List<Choice> _choicePool;
        private MultipleChoiceAnswerState _answerState;
        private List<Question> _givenAnswers = new List<Question>();
        private List<Choice> _activeChoices = new List<Choice>();
        private List<Choice> _selectedChoices = new List<Choice>();
        private List<UnitCircle> _activeUnitCircles;

        private int _answerIndex = 0;
        public static event Action OnChoiceSelected;

        private void Start()
        {
            SpawnChoicePool();
        }

        public void ConfigureChoices(List<Question> questions, MultipleChoiceAnswerState answerState)
        {
            _answerState = answerState;
            _givenAnswers.Clear();
            foreach (var question in questions)
            {
                var available = GetAvailableChoice();
                available.ConfigureUI(question, this);
                _activeChoices.Add(available);
            }
            
            CalculateDynamicCellSize();
            _activeUnitCircles[_answerIndex].AnimateCircle();
        }

        public void SetActiveCircles(List<UnitCircle> circles)
        {
            _activeUnitCircles = circles;
        }

        public List<Question> GetGivenAnswers()
        {
            return _givenAnswers;
        }

        private void SpawnChoicePool()
        {
            _choicePool = new List<Choice>();
            for (int i = 0; i < 9; i++)
            {
                var temp = Instantiate(choicePrefab, gridParent.transform);
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
            _answerState.OnAnswerGiven();
            UpdateAndAnimateUnitCircle(true);
        }

        public void RevokeLastSelection()
        {
            if (_givenAnswers.Count == 0) return;
            UpdateAndAnimateUnitCircle(false);
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

        private void UpdateAndAnimateUnitCircle(bool toggle)
        {
            if (toggle)
            {
                _activeUnitCircles[_answerIndex].OnAnswerGiven();
                _answerIndex++;
                if (_answerIndex >= _activeUnitCircles.Count) return;
                _activeUnitCircles[_answerIndex].AnimateCircle();
            }
            else
            {
                if(_answerIndex < _activeUnitCircles.Count)
                    _activeUnitCircles[_answerIndex].OnAnswerRevoked();
                _answerIndex--;
                _activeUnitCircles[_answerIndex].OnAnswerRevoked();
                _activeUnitCircles[_answerIndex].AnimateCircle();
            }
        }
        
        public RectTransform GetAppropriateChoice(Question question)
        {
            foreach (var choice in _activeChoices)
            {
                var config = choice.GetAssignedQuestionConfig();
                if (config.GetQuestionItem() == question.GetQuestionItem())
                {
                    return choice.GetComponent<RectTransform>();
                }
            }

            throw new ArgumentException("Could not find such question in spawned choices");
            return null;
        }
        
        public void DisableSpawnedChoices()
        {
            OnChoiceSelected?.Invoke();
            foreach (var spawnedChoice in _choicePool)
            {
                spawnedChoice.DisableSelf();
            }

            _answerIndex = 0;
            _activeChoices.Clear();
            _selectedChoices.Clear();
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

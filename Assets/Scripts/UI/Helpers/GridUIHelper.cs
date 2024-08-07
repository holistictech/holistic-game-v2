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
        [SerializeField] private ComplexChoice complexChoicePrefab;

        private List<Choice> _choicePool;
        private List<ComplexChoice> _complexChoicePool;
        private MultipleChoiceAnswerState _answerState;
        private List<Question> _givenAnswers = new List<Question>();
        private List<Choice> _activeChoices = new List<Choice>();
        private List<Choice> _selectedChoices = new List<Choice>();
        private List<UnitCircle> _activeUnitCircles;

        private int _answerIndex = 0;
        private int _requiredAnswerCount = 0;
        public static event Action OnRoundFinished;

        private void Start()
        {
            SpawnChoicePool();
        }

        public void ConfigureChoices(List<Question> questions, MultipleChoiceAnswerState answerState)
        {
            gridParent.gameObject.SetActive(true);
            _answerState = answerState;
            _givenAnswers.Clear();
            foreach (var question in questions)
            {
                Choice available = null;
                available = question is ComplexShapeQuestion ? GetAvailableComplexChoice() : GetAvailableChoice();
                available.ConfigureUI(question, this);
                _activeChoices.Add(available);
            }
            
            CalculateDynamicCellSize();
            _activeUnitCircles[_answerIndex].AnimateCircle();
        }

        public void SetActiveCircles(List<UnitCircle> circles)
        {
            _requiredAnswerCount = circles.Count;
            _activeUnitCircles = circles;
        }

        public void SetStartingIndex(int index)
        {
            _answerIndex = index;
        }

        public List<Question> GetGivenAnswers()
        {
            return _givenAnswers;
        }

        private void SpawnChoicePool()
        {
            _choicePool = new List<Choice>();
            _complexChoicePool = new List<ComplexChoice>();
            for (int i = 0; i < 35; i++)
            {
                var temp = Instantiate(choicePrefab, gridParent.transform);
                _choicePool.Add(temp);
            }
            
            for (int i = 0; i < 10; i++)
            {
                var temp = Instantiate(complexChoicePrefab, gridParent.transform);
                _complexChoicePool.Add(temp);
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
            if (_givenAnswers.Count < _requiredAnswerCount)
            {
                _givenAnswers.Add(question);
                _selectedChoices.Add(selected);
                _answerState.OnAnswerGiven();
                UpdateAndAnimateUnitCircle(true);
            }
            else
            {
                selected.ResetUI();
            }
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
                gridParent.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridParent.constraintCount = 3;
            }
            else if (index == 2)
            {
                gridParent.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                gridParent.constraintCount = 2;
            }else
            {
                gridParent.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridParent.constraintCount = 3;
            }
        }

        private void UpdateAndAnimateUnitCircle(bool toggle)
        {
            if (toggle)
            {
                _activeUnitCircles[_answerIndex].OnAnswerGiven();
                _answerIndex += _answerState.GetUnitIndexUpdateAmount();
                if (_answerIndex >= _activeUnitCircles.Count) return;
                if(_answerState.CanAnimateNextCircle())
                    _activeUnitCircles[_answerIndex].AnimateCircle();
            }
            else
            {
                if(_answerIndex < _activeUnitCircles.Count)
                    _activeUnitCircles[_answerIndex].OnAnswerRevoked();
                _answerIndex -= _answerState.GetUnitIndexUpdateAmount();;
                _activeUnitCircles[_answerIndex].OnAnswerRevoked();
                if(_answerState.CanAnimateNextCircle())
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
        }
        
        public void DisableSpawnedChoices()
        {
            OnRoundFinished?.Invoke();
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
                if (!_choicePool[i].gameObject.activeSelf)
                {
                    return _choicePool[i];
                }
            }

            throw new Exception("No available choice. Need to spawn");
        }

        private ComplexChoice GetAvailableComplexChoice()
        {
            for (int i = 0; i < _complexChoicePool.Count; i++)
            {
                if (!_complexChoicePool[i].gameObject.activeSelf)
                {
                    return _complexChoicePool[i];
                }
            }

            throw new Exception("No available complex choice. Need to spawn");
        }
    }
}

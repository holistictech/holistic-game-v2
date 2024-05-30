using System.Collections.Generic;
using System.Linq;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using Utilities;
using static Utilities.CommonFields;

namespace Spans.BlockSpan
{
    public class BlockSpan : SpanController
    {
        private List<GridConfiguration> _gridConfigs =
            new List<GridConfiguration>()
            {
                new (new Vector2Int(2, 2), BlockSpanModes.Regular),
                new (new Vector2Int(2, 3), BlockSpanModes.Regular),
                new (new Vector2Int(3, 3), BlockSpanModes.Regular),
                new (new Vector2Int(3, 3), BlockSpanModes.ColorChooser),
                new (new Vector2Int(3, 3), BlockSpanModes.ItemChooser),
                new (new Vector2Int(3, 4), BlockSpanModes.Regular),
                new (new Vector2Int(3, 4), BlockSpanModes.ColorChooser),
                new (new Vector2Int(3, 4), BlockSpanModes.ItemChooser),
                new (new Vector2Int(4, 4), BlockSpanModes.Regular),
                new (new Vector2Int(4, 4), BlockSpanModes.ColorChooser),
                new (new Vector2Int(4, 4), BlockSpanModes.ItemChooser),
                new (new Vector2Int(4, 5), BlockSpanModes.Regular),
                new (new Vector2Int(4, 5), BlockSpanModes.ColorChooser),
                new (new Vector2Int(4, 5), BlockSpanModes.ItemChooser),
                new (new Vector2Int(5, 5), BlockSpanModes.Regular),
                new (new Vector2Int(5, 5), BlockSpanModes.ColorChooser),
                new (new Vector2Int(5, 5), BlockSpanModes.ItemChooser),
            };
        
        private int _gridIndex;
        private Vector2Int _currentGrid;
        private int _questionCount = 1;

        protected override void Start()
        {
            base.Start();
            _currentGrid = _gridConfigs[0].GridSize;
            UpdateSpanConfig();
        }
        
        public override List<Question> GetSpanObjects()
        {
            var allQuestions = GetAllAvailableSpanObjects();
            List<Question> roundQuestions = new List<Question>();
            var iterations = _currentGrid.x * _currentGrid.y;
            for (int i = 0; i < iterations; i++)
            {
                var randomQuestionIndex = Random.Range(0, allQuestions.Length);
                var randomQuestion = allQuestions[randomQuestionIndex];
                while (roundQuestions.Contains(randomQuestion))
                {
                    randomQuestionIndex = Random.Range(0, allQuestions.Length);
                    randomQuestion = allQuestions[randomQuestionIndex];
                }
                
                roundQuestions.Add(randomQuestion);
            }
            
            currentSpanQuestions = roundQuestions;


            return currentSpanQuestions;
        }
        
        public override List<Question> GetCurrentSpanQuestions()
        {
            List<Question> corrects = new List<Question>();
            for (int i = 0; i < _questionCount; i++)
            {
                var randomIndex = Random.Range(0, currentSpanQuestions.Count);
                var tempQuestion = currentSpanQuestions[randomIndex];
                while (corrects.Contains(tempQuestion))
                {
                    randomIndex = Random.Range(0, currentSpanQuestions.Count);
                    tempQuestion = currentSpanQuestions[randomIndex];
                }
                
                corrects.Add(tempQuestion);
            }
            
            return corrects;
        }
        
        public override int GetRoundTime()
        {
            return _questionCount * 3;
        }

        public override bool IsAnswerCorrect()
        {
            if (currentGivenAnswers.Count == 0 || currentGivenAnswers.Count != currentDisplayedQuestions.Count)
            {
                IncrementFailStreak();
                return false;
            }
            
            for (int i = 0; i < currentDisplayedQuestions.Count; i++)
            {
                if ((int)currentDisplayedQuestions[i].GetQuestionItem() != (int)currentGivenAnswers[i].GetQuestionItem())
                {
                    IncrementFailStreak();
                    return false;
                }
            }
            IncrementSuccessStreak();
            return true;
        }

        protected override void IncrementSuccessStreak()
        {
            StatisticsHelper.IncrementTrueCount();
            currentSuccessStreak++;
            currentFailStreak = 0;
            if (currentSuccessStreak == 4)
            {
                currentSuccessStreak = 0;
                IncrementGridSize();
            }
        }

        protected override void IncrementFailStreak()
        {
            currentFailStreak++;
            currentSuccessStreak = 0;
            if (currentFailStreak == 4)
            {
                currentFailStreak = 0;
                DecrementGridSize();
            }
        }

        private void IncrementGridSize()
        {
            _gridIndex++;
            _currentGrid = _gridIndex >= _gridConfigs.Count ? _gridConfigs[^1].GridSize : _gridConfigs[_gridIndex].GridSize;
            UpdateSpanConfig();
        }

        private void DecrementGridSize()
        {
            _gridIndex--;
            _currentGrid = _gridIndex <= 0 ? _gridConfigs[0].GridSize : _gridConfigs[_gridIndex].GridSize;
            UpdateSpanConfig();
        }
        
        private void UpdateSpanConfig()
        {
            var size = _currentGrid.x * _currentGrid.y;
            _questionCount = (int)Mathf.Floor(size/3);
            spanEventBus.Trigger(new BlockSpanGridSizeEvent(_currentGrid, _questionCount));
        }
    }
}

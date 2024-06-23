using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using Utilities;
using Utilities.Helpers;
using static Utilities.Helpers.CommonFields;

namespace Spans.BlockSpan
{
    public class BlockSpan : SpanController
    {
        [SerializeField] private Question[] colorQuestions;
        [SerializeField] private Question[] imageQuestions;
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
        private GridConfiguration _currentConfig;
        private int _questionCount = 1;
        private IBlockSpanStrategy _gameMode;

        protected override void Start()
        {
            base.Start();
            _currentConfig = _gridConfigs[0];
            _gameMode = new RegularMode();
            UpdateSpanConfig();
        }
        
        public override List<Question> GetSpanObjects()
        {
            var allQuestions = GetAllAvailableSpanObjects();
            List<Question> roundQuestions = new List<Question>();
            HashSet<int> usedQuestionIndices = new HashSet<int>();
            var iterations = _currentConfig.GridSize.x * _currentConfig.GridSize.y;

            for (int i = 0; i < iterations; i++)
            {
                int randomQuestionIndex;
                do
                {
                    randomQuestionIndex = Random.Range(0, allQuestions.Length);
                } while (usedQuestionIndices.Contains(randomQuestionIndex));

                usedQuestionIndices.Add(randomQuestionIndex);
                roundQuestions.Add(allQuestions[randomQuestionIndex]);
            }

            currentSpanQuestions = roundQuestions;
            return currentSpanQuestions;
        }
        
        protected override Question[] GetAllAvailableSpanObjects()
        {
            switch (_currentConfig.Mode)
            {
                case BlockSpanModes.Regular:
                    return spanQuestions;
                case BlockSpanModes.ColorChooser:
                    foreach (var question in colorQuestions)
                    {
                        question.ResetSelf();
                    }
                    return colorQuestions;
                case BlockSpanModes.ItemChooser:
                    return imageQuestions;
            }

            return new Question[]{};
        }
        
        public override List<Question> GetCurrentSpanQuestions()
        {
            List<Question> currentQuestions = new List<Question>();
            switch (_currentConfig.Mode)
            {
                case BlockSpanModes.Regular:
                    currentQuestions = _gameMode.GetCorrectQuestions(currentSpanQuestions, _questionCount);
                    break;
                case BlockSpanModes.ColorChooser: case BlockSpanModes.ItemChooser:
                    currentQuestions = _gameMode.GetCorrectQuestions(currentSpanQuestions, _questionCount);
                    break;
            }

            currentDisplayedQuestions = currentQuestions;
            return currentQuestions;
        }
        
        public override float GetRoundTime()
        {
            return _questionCount * 3;
        }

        public override bool IsAnswerCorrect()
        {
            var result = _gameMode.CheckAnswer(currentDisplayedQuestions, currentGivenAnswers);
            if (result)
            {
                IncrementSuccessStreak();
            }
            else
            {
                IncrementFailStreak();
            }

            return result;
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
            _currentConfig = _gridIndex >= _gridConfigs.Count ? _gridConfigs[^1] : _gridConfigs[_gridIndex]; 
            SetGameMode(_currentConfig.Mode);
            UpdateSpanConfig();
        }

        private void DecrementGridSize()
        {
            _gridIndex--;
            _currentConfig = _gridIndex <= 0 ? _gridConfigs[0] : _gridConfigs[_gridIndex]; 
            SetGameMode(_currentConfig.Mode);
            UpdateSpanConfig();
        }
        
        private void UpdateSpanConfig()
        {
            var size = _currentConfig.GridSize.x * _currentConfig.GridSize.y;
            _questionCount = (int)Mathf.Floor(size/3);
            spanEventBus.Trigger(new BlockSpanGridSizeEvent(_currentConfig, _questionCount, _gameMode));
        }

        private void SetGameMode(BlockSpanModes mode)
        {
            switch (mode)
            {
                case BlockSpanModes.Regular:
                    _gameMode = new RegularMode();
                    break;
                case BlockSpanModes.ColorChooser:
                    _gameMode = new ColorChooserMode();
                    break;
                case BlockSpanModes.ItemChooser:
                    _gameMode = new ItemChooserMode();
                    break;
            }
        }

        public override BlockSpanModes GetCurrentMode()
        {
            return _currentConfig.Mode;
        }

        public IBlockSpanStrategy GetCurrentStrategy()
        {
            return _gameMode;
        }
    }
}

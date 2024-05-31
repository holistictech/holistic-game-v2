using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using Utilities;
using static Utilities.CommonFields;

namespace Spans.BlockSpan
{
    public class BlockSpan : SpanController
    {
        [SerializeField] private Question[] colorQuestions;
        [SerializeField] private Question[] imageQuestions;
        private List<GridConfiguration> _gridConfigs =
            new List<GridConfiguration>()
            {
                //new (new Vector2Int(2, 2), BlockSpanModes.Regular),
                //new (new Vector2Int(2, 3), BlockSpanModes.Regular),
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
            var iterations = _currentConfig.GridSize.x * _currentConfig.GridSize.y;
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
        
        protected override Question[] GetAllAvailableSpanObjects()
        {
            switch (_currentConfig.Mode)
            {
                case BlockSpanModes.Regular:
                    return spanQuestions;
                case BlockSpanModes.ColorChooser:
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
                    currentQuestions = GetCurrentRegularQuestions();
                    break;
                case BlockSpanModes.ColorChooser: case BlockSpanModes.ItemChooser:
                    currentQuestions = _gameMode.GetCorrectQuestions(currentSpanQuestions);
                    break;
            }

            currentDisplayedQuestions = currentQuestions;
            return currentQuestions;
        }

        private List<Question> GetCurrentRegularQuestions()
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
    }
}

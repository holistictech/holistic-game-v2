using System;
using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using Scriptables.QuestionSystem.Images;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using Utilities.Helpers;
using Random = UnityEngine.Random;

namespace Spans.ComplexSpan
{
    public class PerceptionRecognitionStrategy : IComplexSpanStrategy
    {
        private ComplexSpan _controller;
        private List<Question> _modeQuestions = new List<Question>();
        private List<Question> _currentQuestions = new List<Question>();
        private List<Question> _choices = new List<Question>();
        private List<CommonFields.ButtonType> _chosenButtonTypes = new List<CommonFields.ButtonType>();

        private ComplexQuestionState _questionState;
        private ComplexAnswerState _answerState;
        
        private int _round = 0;
        public void InjectController(ComplexSpan controller)
        {
            _controller = controller;
        }

        public int GetStartingRoundIndex()
        {
            return 3;
        }

        public void InjectModeQuestions(List<Question> mainQuestions, List<Question> helperQuestions)
        {
            _modeQuestions.AddRange(mainQuestions);
            _modeQuestions.AddRange(helperQuestions);
        }

        public void EnableRequiredModeElements(ComplexQuestionState questionState)
        {
            questionState.GetQuestionField().gameObject.SetActive(true);
        }
        
        public void InjectAnswerState(ComplexAnswerState answerState)
        {
            _answerState = answerState;
        }
        
        
        public void ShowQuestion(Questioner questioner, Action onComplete)
        {
            if (_choices == null || _choices.Count == 0)
            {
                GetModeChoices();
            }
            var itemToDisplay = _choices[_round];
            questioner.ShowQuestion(itemToDisplay, () =>
            {
                var buttons = _answerState.GetButtons();
                buttons.ForEach(x => x.gameObject.SetActive(true));
                onComplete?.Invoke();
            });
        }

        public int GetCircleCount()
        {
            return _currentQuestions.Count;
        }

        public List<Question> GetCorrectQuestions(int iterations)
        {
            _currentQuestions = new List<Question>();
            
            for (int i = 0; i < iterations; i++)
            {
                var question = _modeQuestions[Random.Range(0, _modeQuestions.Count)];
                while (_currentQuestions.Contains(question))
                {
                    question = _modeQuestions[Random.Range(0, _modeQuestions.Count)];
                }
                
                _currentQuestions.Add(question);
            }
            
            _currentQuestions.Shuffle();
            return _currentQuestions;
        }

        public List<Question> GetModeChoices()
        {
            var questionCount = _currentQuestions.Count;
            var count = questionCount == 3 ? 5 : questionCount * 2;
            _choices = new List<Question>(count);
            for (int i = 0; i < questionCount; i++)
            {
                if (IsLucky())
                {
                    _choices.Add(_currentQuestions[i]);
                }
            }

            var iterations = _choices.Capacity - _choices.Count;
            for (int i = 0; i < iterations; i++)
            {
                var question = _modeQuestions[Random.Range(0, _modeQuestions.Count)];
                while (_choices.Contains(question))
                {
                    question = _modeQuestions[Random.Range(0, _modeQuestions.Count)];
                }
                
                _choices.Add(question);
            }

            return _choices;
        }

        public void AppendChoice(CommonFields.ButtonType type)
        {
            _chosenButtonTypes.Add(type);
            
        }

        private bool IsLucky()
        {
            return Random.Range(0, 2) == 0;
        }
        
        public bool CheckAnswer(List<Question> given)
        {
            var displayed = _choices[_round];
            var choice = _chosenButtonTypes[_round];
            _round++;

            if (_round == _choices.Count)
            {
                _controller.SetMainSpanNeeded(true);
                _round = 0;
            }
            
            if (_currentQuestions.Contains(displayed))
            {
                if ((displayed is ImageQuestion && choice == CommonFields.ButtonType.Saw) ||
                    (displayed is ClipQuestion && choice == CommonFields.ButtonType.Heard))
                {
                    return true;
                }
            }
            else
            {
                if (choice == CommonFields.ButtonType.SawNorHeard)
                {
                    return true;
                }
            }

            return false;
        }
        
        public int GetUnitIndex()
        {
            return 0;
        }
    }
}

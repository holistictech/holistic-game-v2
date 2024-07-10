using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using Scriptables.QuestionSystem.Images;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using UnityEngine;
using Utilities.Helpers;

namespace Spans.ComplexSpan
{
    public class PerceptionRecognitionStrategy : IComplexSpanStrategy
    {
        private ComplexSpan _controller;
        private List<Question> _modeQuestions = new List<Question>();
        private List<Question> _currentQuestions = new List<Question>();
        private List<Question> _choices = new List<Question>();
        private List<CommonFields.ButtonType> _chosenButtonTypes = new List<CommonFields.ButtonType>();
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
        
        public void EnableRequiredModeElements(ComplexAnswerState answerState)
        {
            var buttons = answerState.GetButtons();
            buttons.ForEach(x => x.gameObject.SetActive(true));
        }

        public int GetCircleCount()
        {
            return _controller.GetCurrentDisplayedQuestions().Count;
        }

        public List<Question> GetCorrectQuestions(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                var question = _modeQuestions[Random.Range(0, _modeQuestions.Count)];
                while (_modeQuestions.Contains(question))
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

        private int _round = 0;

        public bool CheckAnswer(List<Question> given)
        {
            if (_round >= _choices.Count || _round >= _chosenButtonTypes.Count)
            {
                return false;
            }

            var displayed = _choices[_round];
            var choice = _chosenButtonTypes[_round];
            _round++;
            
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

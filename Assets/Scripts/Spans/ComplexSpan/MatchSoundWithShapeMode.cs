using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Helpers;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using UnityEngine;
using Utilities.Helpers;
using Random = UnityEngine.Random;

namespace Spans.ComplexSpan
{
    public class MatchSoundWithShapeMode : IComplexSpanStrategy
    {
        private ComplexSpan _controller;
        private ComplexQuestionState _questionState;
        private ComplexAnswerState _answerState;
        private SpanHintHelper _hintHelper;

        private List<Question> _mainQuestions;
        private List<Question> _helperQuestions;
        private List<Question> _currentQuestions;
        private Dictionary<Question, Question> _classifiedQuestions = new Dictionary<Question, Question>();
        
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
            if (mainQuestions.Count != helperQuestions.Count)
            {
                Debug.LogError("Mode question lists does not match!");
                return;
            }
            
            _mainQuestions.AddRange(mainQuestions);
            helperQuestions.AddRange(_helperQuestions);
            for (int i = 0; i < _mainQuestions.Count; i++)
            {
                var mainQuestion = _mainQuestions[i];
                var category = _helperQuestions[i];
                if (_classifiedQuestions.TryGetValue(mainQuestion, out Question value))
                {
                    Debug.LogError($"Duplicate may be exist in dictionary! value: {value}");
                }
                
                _classifiedQuestions.Add(mainQuestion, category);
            }
        }
        
        public void InjectQuestionState(ComplexQuestionState questionState)
        {
            _questionState = questionState;
            _hintHelper = _questionState.GetHintHelper();
        }

        public void InjectAnswerState(ComplexAnswerState answerState)
        {
            _answerState = answerState;
        }

        private bool _hintNeeded = true;
        public void ShowQuestionStateQuestion(Questioner questioner)
        {
            questioner.InjectQuestionState(_questionState);
            if (_hintNeeded)
            {
                _hintNeeded = false;
                _hintHelper.PoolImagesOnNeed();
                _hintHelper.PopulateHintGrid(_classifiedQuestions.Take(_currentQuestions.Count).ToDictionary(pair => pair.Key, pair => pair.Value),
                    () =>
                    {
                        questioner.PlayCoroutine(_currentQuestions, this, _questionState);
                    });
            }
            else
            {
                questioner.PlayCoroutine(_currentQuestions, this, _questionState);
            }
        }

        public void HandleOnComplete()
        {
            _controller.SwitchState();
        }

        public void ShowAnswerStateQuestion(Questioner questioner, Action onComplete)
        {
            _answerState.ConfigureUnitCircles();
            _answerState.EnableButtons();
            onComplete?.Invoke();
        }

        public int GetCircleCount()
        {
            return _currentQuestions.Count;
        }

        private int _tempIndex = 0;
        private int _prevRoundIndex = 0;
        public List<Question> GetCorrectQuestions(int iterations)
        {
            _currentQuestions.Clear();

            if (iterations != _prevRoundIndex)
            {
                _hintNeeded = true;
            }
            
            switch (_tempIndex)
            {
                case 0:
                    var index = Random.Range(0, _mainQuestions.Count);
                    var question = _mainQuestions[index];
                    _currentQuestions.Add(question);
                    _currentQuestions.Add(question);
                    _currentQuestions.Add(question);
                    _tempIndex++;
                    break;
                case 1:
                    index = Random.Range(0, _mainQuestions.Count);
                    question = _mainQuestions[index];
                    var next = _mainQuestions[^1];
                    _currentQuestions.Add(question);
                    _currentQuestions.Add(question);
                    _currentQuestions.Add(next);
                    _tempIndex++;
                    break;
                default:
                    _currentQuestions.AddRange(GetRandomQuestions(iterations));
                    _controller.SetMainSpanNeeded(true);
                    break;
            }

            _prevRoundIndex = _currentQuestions.Count;
            _currentQuestions.Shuffle();
            return _currentQuestions;
        }

        private List<Question> GetRandomQuestions(int count)
        {
            var limit = count >= _mainQuestions.Count ? _mainQuestions.Count : count;
            List<Question> selected = new List<Question>();
            for (int i = 0; i < count; i++)
            {
                var index = Random.Range(0, limit);
                var question = _mainQuestions[index];
                while (selected.Contains(question))
                {
                    index = Random.Range(0, limit);
                    question = _mainQuestions[index];
                }

                selected.Add(question);
            }
            
            return selected;
        }

        public List<Question> GetModeChoices()
        {
            return _helperQuestions.GetRange(0, _currentQuestions.Count);
        }

        public void AppendChoice(CommonFields.ButtonType type)
        {
            Debug.Log("Not related with this mode. Exist for the sake of strategy pattern");
        }

        public bool CheckAnswer(List<Question> given)
        {
            if (given.Count != _currentQuestions.Count)
            {
                return false;
            }

            for (int i = 0; i < given.Count; i++)
            {
                var tempGiven = given[i];
                if (_classifiedQuestions.TryGetValue(_currentQuestions[i], out Question value))
                {
                    if (!value.IsEqual(tempGiven)) return false;
                }
                else
                {
                    Debug.LogError("This is some weird behaviour");
                    return false;
                }
            }

            return true;
        }

        public int GetUnitIndex()
        {
            return 0;
        }
    }
}

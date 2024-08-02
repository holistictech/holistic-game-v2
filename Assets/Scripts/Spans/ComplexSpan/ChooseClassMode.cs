using System;
using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using UnityEngine;
using Utilities.Helpers;
using Random = UnityEngine.Random;

namespace Spans.ComplexSpan
{
    public class ChooseClassMode : IComplexSpanStrategy
    {
        private ComplexSpan _controller;
        private ComplexQuestionState _questionState;
        private ComplexAnswerState _answerState;

        private List<Question> _modeQuestions = new List<Question>();
        private List<Question> _helperChoices = new List<Question>();
        private Dictionary<Question, Question> _classifiedQuestionDictionary = new Dictionary<Question, Question>();

        private List<Question> _currentQuestions = new List<Question>();
        private Dictionary<Question, Question> _currentRoundDictionary = new Dictionary<Question, Question>();
        private int _currentRoundIndex = 0;
        public void InjectController(ComplexSpan controller)
        {
            _controller = controller;
        }

        public int GetStartingRoundIndex()
        {
            return 2;
        }

        public void InjectModeQuestions(List<Question> mainQuestions, List<Question> helperQuestions)
        {
            _modeQuestions = mainQuestions;
            _helperChoices = helperQuestions;
            
            for (int i = 0; i < mainQuestions.Count; i++)
            {
                if (_classifiedQuestionDictionary.ContainsKey(mainQuestions[i]))
                {
                    Debug.LogError($"Duplicate key found: {mainQuestions[i].name}");
                }
                _classifiedQuestionDictionary.Add(mainQuestions[i] , helperQuestions[i%3]);
            }
        }

        public void InjectQuestionState(ComplexQuestionState questionState)
        {
            _questionState = questionState;
        }

        public void InjectAnswerState(ComplexAnswerState answerState)
        {
            _answerState = answerState;
            _answerState.EnableButtons();
        }

        private bool _isInitial = true;
        public void ShowQuestionStateQuestion(Questioner questioner)
        {
            questioner.InjectQuestionState(_questionState);
            if (_currentRoundIndex >= _currentQuestions.Count)
            {
                if (_isInitial)
                {
                    _controller.SetMainSpanNeeded(true);
                    _isInitial = false;
                    Debug.LogError("Main span is needed");
                    HandleOnComplete();
                    return;
                }

                if (_isInitial) return;
                _isInitial = true;
                _controller.GetSpanObjects();
                ShowQuestions(questioner);
            }
            else
            {
                ShowQuestions(questioner);
            }
        }
        
        private void ShowQuestions(Questioner questioner)
        {
            questioner.PlayCoroutine(new List<Question>{_currentQuestions[_currentRoundIndex]}, this, _questionState);
            _currentRoundIndex++;
        }

        public void HandleOnComplete()
        {
            _questionState.SwitchNextState();
        }

        public void ShowAnswerStateQuestion(Questioner questioner, Action onComplete)
        {
            _answerState.SetChoiceUI();
            if (_controller.GetIsMainSpanNeeded())
            {
                _answerState.TriggerHintHelper("Sırasıyla hangi resimleri görmüştün?", () =>
                {
                    onComplete?.Invoke();
                });
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        public int GetCircleCount()
        {
            return _currentRoundIndex >= _currentQuestions.Count ? _controller.GetRoundIndex() : 1;
        }

        public List<Question> GetCorrectQuestions(int iterations)
        {
            Debug.Log("Getting mode questions");
            _currentQuestions.Clear();
            _currentRoundDictionary.Clear();
            _currentRoundIndex = 0;
            
            List<Question> iterationQuestions = new List<Question>();
            for (int i = 0; i < iterations; i++)
            {
                var randomIndex = Random.Range(0, _modeQuestions.Count);
                var randomQuestion = _modeQuestions[randomIndex];
                while (iterationQuestions.Contains(randomQuestion))
                {
                    randomIndex = Random.Range(0, _modeQuestions.Count);
                    randomQuestion = _modeQuestions[randomIndex];
                }
                
                iterationQuestions.Add(randomQuestion);
                _currentQuestions.Add(randomQuestion);
                if (_classifiedQuestionDictionary.TryGetValue(randomQuestion, out Question questionClass))
                {
                    _currentRoundDictionary.Add(randomQuestion, questionClass);
                }
                else
                {
                    Debug.LogError("selected random question does not exist in main dictionary");
                }
            }
            Debug.Log($"mode questions ready {_currentQuestions.Count}");
            return _currentQuestions;
        }

        public List<Question> GetModeChoices()
        {
            if (_controller.GetIsMainSpanNeeded())
            {
                List<Question> choices = new List<Question>(_currentQuestions);
                for (int i = 0; i < _currentQuestions.Count; i++)
                {
                    var randomIndex = Random.Range(0, _modeQuestions.Count);
                    var randomQuestion = _modeQuestions[randomIndex];
                    while (choices.Contains(randomQuestion))
                    {
                        randomIndex = Random.Range(0, _modeQuestions.Count);
                        randomQuestion = _modeQuestions[randomIndex];
                    }
                    
                    choices.Add(randomQuestion);
                }

                return choices;
            }
            else
            {
                return _helperChoices;
            }
        }

        public void AppendChoice(CommonFields.ButtonType type)
        {
            
        }

        public bool CheckAnswer(List<Question> given)
        {
            List<Question> listToBeChecked = new List<Question>();
            if (_controller.GetIsMainSpanNeeded())
            {
                listToBeChecked = _currentQuestions;
            }
            else
            {
                listToBeChecked = GetComparerQuestions();
            }

            if (listToBeChecked.Count != given.Count)
            {
                return false;
            }

            for (int i = 0; i < listToBeChecked.Count; i++)
            {
                if (!listToBeChecked[i].IsEqual(given[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private List<Question> GetComparerQuestions()
        {
            List<Question> comparer = new List<Question>();
            if (_currentRoundDictionary.TryGetValue(_currentQuestions[_currentRoundIndex], out Question correctChoice))
            {
                comparer.Add(correctChoice);
            }

            return comparer;
        }

        public int GetUnitIndex()
        {
            return 0;
        }
    }
}
